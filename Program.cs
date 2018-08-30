using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using vk_web_mock.Services;

namespace vk_web_mock
{
    public class Program
    {
        private static PodStore _podStore;
        private static ILogger _logger;
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                   .UseUrls("http://+:5000")
                   .ConfigureServices(services =>
                   {
                       services.AddCors();
                   })
                    .Configure(app =>
                    {
                        app.UseCors(options =>
                        {
                            options
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowAnyOrigin();
                        });

                        _podStore = new PodStore();
                        _logger = app.ApplicationServices.GetService<ILogger<Program>>();

                        app.HandlePath("/capacity", GetCapacity);
                        app.HandlePath("/nodeConditions", GetNodeConditions);
                        app.HandlePath("/nodeAddresses", GetNodeAddresses);
                        app.HandlePath("/getPods", GetPods);
                        app.HandlePath("/getPodStatus", GetPodStatus);
                        app.HandlePath("/createPod", CreatePod);
                        app.HandlePath("/updatePod", UpdatePod);
                        app.HandlePath("/deletePod", DeletePod);
                        app.HandlePath("/getContainerLogs", GetContainerLogs);

                        app.UseDefaultFiles();
                        app.UseStaticFiles(); // homepage, CSS etc

                        app.Run(context =>
                        {
                            _logger.LogWarning($"Unmatched: {context.Request.Path}");
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            return Task.CompletedTask;
                        });
                    });
        }
        private static async Task GetCapacity(HttpContext context)
        {
            if (!context.ShouldProcessFor(HttpMethods.Get))
                return;
            await context.WriteJsonResponseAsync(new
            {
                cpu = "20",
                memory = "100Gi",
                pods = "20"
            });
        }
        private static async Task GetNodeConditions(HttpContext context)
        {
            if (!context.ShouldProcessFor(HttpMethods.Get))
                return;

            DateTime utcNow = DateTime.UtcNow;
            await context.WriteJsonResponseAsync(new[] {
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
        private static async Task GetNodeAddresses(HttpContext context)
        {
            if (!context.ShouldProcessFor(HttpMethods.Get))
                return;

            var kubeletPort = Environment.GetEnvironmentVariable("KUBELET_PORT");
            var kubeletPodIp = Environment.GetEnvironmentVariable("VKUBELET_POD_IP");
            if (string.IsNullOrEmpty(kubeletPodIp))
            {
                _logger.LogInformation("NodeAddresses - returning empty list");
                await context.WriteJsonResponseAsync(new NodeAddress[] { });
            }
            else
            {
                var address = new NodeAddress { Type = NodeAddressType.InternalIP, Address = kubeletPodIp };
                _logger.LogInformation($"NodeAddresses - returning {address.Type} {address.Address}");
                await context.WriteJsonResponseAsync(new[] { address });
            }
        }

        private static async Task GetPods(HttpContext context)
        {
            if (!context.ShouldProcessFor(HttpMethods.Get))
                return;

            _logger.LogInformation($"GetPods");
            var pods = _podStore.GetPods()
                .OrderBy(p => p.Metadata.Namespace)
                .ThenBy(p => p.Metadata.Name); // sort for consistent output :-)
            await context.WriteJsonResponseAsync(pods);
        }

        private static async Task GetPodStatus(HttpContext context)
        {
            if (!context.ShouldProcessFor(HttpMethods.Get))
                return;

            var @namespace = context.GetQueryStringValue("namespace");
            var name = context.GetQueryStringValue("name");
            var pod = _podStore.GetPod(@namespace, name);
            if (pod == null)
            {
                _logger.LogInformation($"GetPodStatus: returning NotFound for {@namespace}:{name}");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            _logger.LogInformation($"GetPodStatus: returning {pod.Status} for {@namespace}:{name}");
            await context.WriteJsonResponseAsync(pod.Status);
        }


        private static async Task CreatePod(HttpContext context)
        {
            if (!context.ShouldProcessFor(HttpMethods.Post))
                return;

            var pod = await context.GetJsonBody<Pod>();
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
        }

        private static async Task UpdatePod(HttpContext context)
        {
            if (!context.ShouldProcessFor(HttpMethods.Post))
                return;

            var pod = await context.GetJsonBody<Pod>();
            _logger.LogInformation($"Update: {pod.Metadata.Namespace}:{pod.Metadata.Name}");

            // Delete and re-add
            // TODO check that we know about the pod in future?
            _podStore.DeletePod(pod.Metadata.Namespace, pod.Metadata.Name);
            _podStore.AddPod(pod);
        }

        private static async Task DeletePod(HttpContext context)
        {
            if (!context.ShouldProcessFor(HttpMethods.Post))
                return;

            var pod = await context.GetJsonBody<Pod>();
            _logger.LogInformation($"DeletePod: {pod.Metadata.Namespace}:{pod.Metadata.Name}");
            if (!_podStore.DeletePod(pod.Metadata.Namespace, pod.Metadata.Name))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }

        private static async Task GetContainerLogs(HttpContext context)
        {
            var @namespace = context.GetQueryStringValue("namespace");
            var podName = context.GetQueryStringValue("podName");
            var containerName = context.GetQueryStringValue("containerName");
            var tailString = context.GetQueryStringValue("tail");

            _logger.LogInformation($"GetContainerLogs: {@namespace}:{podName}:{containerName}");
            var pod = _podStore.GetPod(@namespace, podName);
            if (pod == null)
            {
                await context.Response.WriteAsync("No such pod");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            if (!pod.Spec.Containers.Any(c => c.Name == containerName))
            {
                await context.Response.WriteAsync("No such container");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            var logContent = $"Simulated log content for {@namespace}, {podName}, {containerName}\nIf this provider actually ran the containers then the logs would appear here ;-)\n";
            await context.Response.WriteAsync(logContent);
        }
    }
}