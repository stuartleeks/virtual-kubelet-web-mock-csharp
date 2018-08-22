using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using vk_web_mock.Models;
using vk_web_mock.Services;

namespace vk_web_mock.Controllers
{
    [ApiController]
    public class VirtualKubeletWebController : Controller
    {
        private readonly PodStore _podStore;
        private readonly ILogger _logger;
        public VirtualKubeletWebController(
            PodStore podStore,
            ILogger<VirtualKubeletWebController> logger)
        {
            _podStore = podStore;
            _logger = logger;
        }

        [HttpGet("capacity")]
        public IActionResult GetCapacity()
        {
            return Json(new
            {
                cpu = "20",
                memory = "100Gi",
                pods = "20"
            });
        }

        [HttpGet("nodeConditions")]
        public IActionResult GetNodeConditions()
        {
            DateTime utcNow = DateTime.UtcNow;

            return base.Json(new[] {
                new {
                    lastHeartbeatTime = utcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    lastTransitionTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message= "At your service",
                    reason = "KubeletReady",
                    status = "True",
                    type="Ready"
                }

        //         {
		// 	Type:               "Ready",
		// 	Status:             v1.ConditionTrue,
		// 	LastHeartbeatTime:  metav1.Now(),
		// 	LastTransitionTime: metav1.Now(),
		// 	Reason:             "KubeletReady",
		// 	Message:            "kubelet is ready.",
		// },
		// {
		// 	Type:               "OutOfDisk",
		// 	Status:             v1.ConditionFalse,
		// 	LastHeartbeatTime:  metav1.Now(),
		// 	LastTransitionTime: metav1.Now(),
		// 	Reason:             "KubeletHasSufficientDisk",
		// 	Message:            "kubelet has sufficient disk space available",
		// },
		// {
		// 	Type:               "MemoryPressure",
		// 	Status:             v1.ConditionFalse,
		// 	LastHeartbeatTime:  metav1.Now(),
		// 	LastTransitionTime: metav1.Now(),
		// 	Reason:             "KubeletHasSufficientMemory",
		// 	Message:            "kubelet has sufficient memory available",
		// },
		// {
		// 	Type:               "DiskPressure",
		// 	Status:             v1.ConditionFalse,
		// 	LastHeartbeatTime:  metav1.Now(),
		// 	LastTransitionTime: metav1.Now(),
		// 	Reason:             "KubeletHasNoDiskPressure",
		// 	Message:            "kubelet has no disk pressure",
		// },
		// {
		// 	Type:               "NetworkUnavailable",
		// 	Status:             v1.ConditionFalse,
		// 	LastHeartbeatTime:  metav1.Now(),
		// 	LastTransitionTime: metav1.Now(),
		// 	Reason:             "RouteCreated",
		// 	Message:            "RouteController created a route",
		// },
            });
        }


        [HttpGet("nodeAddresses")]
        public IActionResult GetNodeAddresses()
        {
            return base.Json(new object[] { });
            // func (p *MockProvider) NodeAddresses() []v1.NodeAddress {
            // 	return []v1.NodeAddress{
            // 		{
            // 			Type:    "InternalIP",
            // 			Address: p.internalIP,
            // 		},
            // 	}
            // }
        }

        [HttpGet("getPods")]
        public IActionResult GetPods()
        {
            var pods = _podStore.GetPods();
            return base.Json(pods);
        }

        [HttpGet("getPodStatus")]
        public IActionResult GetPodStatus([FromQuery] string @namespace, [FromQuery] string name)
        {
            var pod = _podStore.GetPod(@namespace, name);
            if (pod == null)
            {
                return NotFound();
            }
            return Json(pod.Status);
        }

        [HttpPost("createPod")]
        public IActionResult CreatePod(Pod pod)
        {
            _podStore.AddPod(pod);
            return Ok();
        }
        [HttpDelete("deletePod")]
        public IActionResult DeletePod(Pod pod)
        {
            if (_podStore.DeletePod(pod.Metadata.Namespace, pod.Metadata.Name))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("getContainerLogs")]
        public IActionResult GetContainerLogs([FromQuery]string @namespace, [FromQuery]string podName, [FromQuery]string containerName, [FromQuery]int tail)
        {
            var pod = _podStore.GetPod(@namespace, podName);
            if (pod == null)
                return NotFound("No such pod");

            if (!pod.Spec.Containers.Any(c=> c.Name == containerName))
                return NotFound("No such container");

            return Content($"TODO: implement container logs: {@namespace}, {podName}, {containerName}"); // TODO implement container logs
        }

        [HttpGet("{*unmatched}")]
        public IActionResult Catchall(string unmatched)
        {
            _logger.LogWarning($"Unmatched: {unmatched}");
            return NotFound();
        }
    }


}
