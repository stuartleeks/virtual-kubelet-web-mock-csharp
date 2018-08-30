using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using vk_web_mock.Services;

namespace vk_web_mock.Controllers
{
    [ApiController]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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
                new NodeCondition {
                    LastHeartbeatTime = utcNow,
                    LastTransitionTime = utcNow,
                    Message= "At your service",
                    Reason = "KubeletReady",
                    Status = NodeConditionStatus.True,
                    Type= NodeConditionType.Ready
                },
                new NodeCondition {
                    LastHeartbeatTime = utcNow,
                    LastTransitionTime = utcNow,
                    Message= "Plenty of disk space here",
                    Reason = "KubeletHasSufficientDisk",
                    Status = NodeConditionStatus.False,
                    Type= NodeConditionType.OutOfDisk
                },
                new NodeCondition {
                    LastHeartbeatTime = utcNow,
                    LastTransitionTime = utcNow,
                    Message= "Plenty of memory here",
                    Reason = "KubeletHasSufficientMemory",
                    Status = NodeConditionStatus.False,
                    Type= NodeConditionType.MemoryPressure
                },
                new NodeCondition {
                    LastHeartbeatTime = utcNow,
                    LastTransitionTime = utcNow,
                    Message= "At your service",
                    Reason = "KubeletHasNoDiskPressure",
                    Status = NodeConditionStatus.False,
                    Type= NodeConditionType.DiskPressure
                },
                new NodeCondition {
                    LastHeartbeatTime = utcNow,
                    LastTransitionTime = utcNow,
                    Message= "Cables all intact",
                    Reason = "RouteCreated",
                    Status = NodeConditionStatus.False,
                    Type= NodeConditionType.NetworkUnavailable
                },
            });
        }


        [HttpGet("nodeAddresses")]
        public IActionResult GetNodeAddresses()
        {
            var kubeletPort = Environment.GetEnvironmentVariable("KUBELET_PORT");
            var kubeletPodIp = Environment.GetEnvironmentVariable("VKUBELET_POD_IP");
            if (string.IsNullOrEmpty(kubeletPodIp))
            {
                _logger.LogInformation("NodeAddresses - returning empty list");
                return base.Json(new NodeAddress[] { });
            }
            else
            {
                var address = new NodeAddress { Type = NodeAddressType.InternalIP, Address = kubeletPodIp };
                _logger.LogInformation($"NodeAddresses - returning {address.Type} {address.Address}");
                return base.Json(new[] { address });
            }
        }

        [HttpGet("getPods")]
        public IActionResult GetPods()
        {
            _logger.LogInformation($"GetPods");
            var pods = _podStore.GetPods()
                .OrderBy(p=>p.Metadata.Namespace)
                .ThenBy(p=>p.Metadata.Name); // sort for consistent output :-)
            return base.Json(pods);
        }

        [HttpGet("getPodStatus")]
        public IActionResult GetPodStatus([FromQuery] string @namespace, [FromQuery] string name)
        {
            var pod = _podStore.GetPod(@namespace, name);
            if (pod == null)
            {
            _logger.LogInformation($"GetPodStatus: returning NotFound for {@namespace}:{name}");
                return NotFound();
            }
            _logger.LogInformation($"GetPodStatus: returning {pod.Status} for {@namespace}:{name}");
            return Json(pod.Status);
        }

        [HttpPost("createPod")]
        public IActionResult CreatePod(Pod pod)
        {
            _logger.LogInformation($"CreatePod: {pod.Metadata.Namespace}:{pod.Metadata.Name}");
            // update state so that we show as running!
            pod.Status.Phase = PodPhase.Running;
            pod.Status.Conditions = new[] {
                new PodCondition{ Type = PodConditionType.PodScheduled, Status = PodConditionStatus.True},
                new PodCondition{ Type = PodConditionType.Initialized, Status = PodConditionStatus.True},
                new PodCondition{ Type = PodConditionType.Ready, Status = PodConditionStatus.True},
            };
            pod.Status.ContainerStatuses = pod.Spec.Containers
                .Select(container => new ContainerStatus
                {
                    Name = container.Name,
                    Image = container.Image,
                    Ready = true,
                    State = new ContainerState
                    {
                        Running = new ContainerStateRunning
                        {
                            StartedAt = DateTime.UtcNow
                        }
                    }
                })
                .ToArray();

            _podStore.AddPod(pod);

            return Ok();
        }
        
        [HttpPost("updatePod")]
        public IActionResult UpdatePod(Pod pod)
        {
            _logger.LogInformation($"Update: {pod.Metadata.Namespace}:{pod.Metadata.Name}");

            // Delete and re-add
            // TODO check that we know about the pod in future?
            _podStore.DeletePod(pod.Metadata.Namespace, pod.Metadata.Name);
            _podStore.AddPod(pod);

            return Ok();
        }
        [HttpDelete("deletePod")]
        public IActionResult DeletePod(Pod pod)
        {
            _logger.LogInformation($"DeletePod: {pod.Metadata.Namespace}:{pod.Metadata.Name}");
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
            _logger.LogInformation($"GetContainerLogs: {@namespace}:{podName}:{containerName}");
            var pod = _podStore.GetPod(@namespace, podName);
            if (pod == null)
                return NotFound("No such pod");

            if (!pod.Spec.Containers.Any(c => c.Name == containerName))
                return NotFound("No such container");

            var logContent = $"Simulated log content for {@namespace}, {podName}, {containerName}\nIf this provider actually ran the containers then the logs would appear here ;-)\n";
            return Content(logContent);
        }

        [HttpGet("{*unmatched}")]
        public IActionResult Catchall(string unmatched)
        {
            _logger.LogWarning($"Unmatched: {unmatched}");
            return NotFound();
        }
    }
}
