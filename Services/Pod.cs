using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace vk_web_mock.Services
{
    public class Pod
    {
        public Metadata Metadata { get; set; }
        public Spec Spec { get; set; }
        public Status Status { get; set; }
    }

    public static class PodPhase
    {

        // PodPending means the pod has been accepted by the system, but one or more of the containers
        // has not been started. This includes time before being bound to a node, as well as time spent
        // pulling images onto the host.
        public const string Pending = "Pending";
        // PodRunning means the pod has been bound to a node and all of the containers have been started.
        // At least one container is still running or is in the process of being restarted.
        public const string Running = "Running";
        // PodSucceeded means that all containers in the pod have voluntarily terminated
        // with a container exit code of 0, and the system is not going to restart any of these containers.
        public const string Succeeded = "Succeeded";
        // PodFailed means that all containers in the pod have terminated, and at least one container has
        // terminated in a failure (exited with a non-zero exit code or was stopped by the system).
        public const string Failed = "Failed";
        // PodUnknown means that for some reason the state of the pod could not be obtained, typically due
        // to an error in communicating with the host of the pod.
        public const string Unknown = "Unknown";
    }


    public static class PodConditionType
    {
        // PodScheduled represents status of the scheduling process for this pod.
        public const string Scheduled = "PodScheduled";
        // PodReady means the pod is able to service requests and should be added to the
        // load balancing pools of all matching services.
        public const string Ready = "Ready";
        // PodInitialized means that all init containers in the pod have started successfully.
        public const string Initialized = "Initialized";
        // PodReasonUnschedulable reason in PodScheduled PodCondition means that the scheduler
        // can't schedule the pod right now, for example due to insufficient resources in the cluster.
        public const string Unschedulable = "Unschedulable";
    }
    public class Metadata
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string SelfLink { get; set; }
        public Guid Uid { get; set; }
        public string ResourceVersion { get; set; }
        public DateTimeOffset CreationTimestamp { get; set; }
        public Annotations Annotations { get; set; }
    }

    public partial class Annotations
    {
        [JsonProperty("kubectl.kubernetes.io/last-applied-configuration")]
        public string KubectlKubernetesIoLastAppliedConfiguration { get; set; }
    }

    public partial class Spec
    {
        public Volume[] Volumes { get; set; }
        public Container[] Containers { get; set; }
        public string RestartPolicy { get; set; }
        public long TerminationGracePeriodSeconds { get; set; }
        public string DnsPolicy { get; set; }
        public string ServiceAccountName { get; set; }
        public string ServiceAccount { get; set; }
        public string NodeName { get; set; }
        public SecurityContext SecurityContext { get; set; }
        public string SchedulerName { get; set; }
        public Toleration[] Tolerations { get; set; }
    }

    public partial class Container
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public Port[] Ports { get; set; }
        public Resources Resources { get; set; }
        public VolumeMount[] VolumeMounts { get; set; }
        public string TerminationMessagePath { get; set; }
        public string TerminationMessagePolicy { get; set; }
        public string ImagePullPolicy { get; set; }
    }

    public partial class Port
    {
        public string Name { get; set; }
        public long ContainerPort { get; set; }
        public string Protocol { get; set; }
    }

    public partial class Resources
    {
        public Requests Requests { get; set; }
    }

    public class Requests
    {
        public string Cpu { get; set; }
        public string Memory { get; set; }
    }

    public class VolumeMount
    {
        public string Name { get; set; }
        public bool ReadOnly { get; set; }
        public string MountPath { get; set; }
    }

    public class SecurityContext
    {
    }

    public class Toleration
    {
        public string Key { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
        public string Effect { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Operator { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? TolerationSeconds { get; set; }
    }

    public class Volume
    {
        public string Name { get; set; }
        public Secret Secret { get; set; }
    }

    public class Secret
    {
        public string SecretName { get; set; }
        public long DefaultMode { get; set; }
    }

    public class Status
    {
        public string Phase { get; set; }
        public string QosClass { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PodCondition[] Conditions { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ContainerStatus[] ContainerStatuses { get; set; }
    }

    public class PodCondition
    {
        // partial class implementation!!
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class ContainerStatus
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public bool Ready { get; set; }
        public int RestartCount { get; set; }
        public ContainerState State { get; set; }
    }

    public class ContainerState
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ContainerStateWaiting Waiting { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ContainerStateRunning Running { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ContainerStateTerminated Terminated { get; set; }
    }
    public class ContainerStateWaiting
    {
        public string Reason { get; set; }
        public string Message { get; set; }
    }
    public class ContainerStateRunning
    {
        public DateTime StartedAt { get; set; }
    }
    public class ContainerStateTerminated
    {
        public int ExitCode { get; set; }
        public int Signal { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set;}
        public string ContainerID { get;set;}
    }
}

