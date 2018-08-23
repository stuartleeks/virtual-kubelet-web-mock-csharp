using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace vk_web_mock.Services
{
    public class Pod
    {
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("spec")]
        public Spec Spec { get; set; }

        [JsonProperty("status")]
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
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("selfLink")]
        public string SelfLink { get; set; }

        [JsonProperty("uid")]
        public Guid Uid { get; set; }

        [JsonProperty("resourceVersion")]
        public string ResourceVersion { get; set; }

        [JsonProperty("creationTimestamp")]
        public DateTimeOffset CreationTimestamp { get; set; }

        [JsonProperty("annotations")]
        public Annotations Annotations { get; set; }
    }

    public partial class Annotations
    {
        [JsonProperty("kubectl.kubernetes.io/last-applied-configuration")]
        public string KubectlKubernetesIoLastAppliedConfiguration { get; set; }
    }

    public partial class Spec
    {
        [JsonProperty("volumes")]
        public Volume[] Volumes { get; set; }

        [JsonProperty("containers")]
        public Container[] Containers { get; set; }

        [JsonProperty("restartPolicy")]
        public string RestartPolicy { get; set; }

        [JsonProperty("terminationGracePeriodSeconds")]
        public long TerminationGracePeriodSeconds { get; set; }

        [JsonProperty("dnsPolicy")]
        public string DnsPolicy { get; set; }

        [JsonProperty("serviceAccountName")]
        public string ServiceAccountName { get; set; }

        [JsonProperty("serviceAccount")]
        public string ServiceAccount { get; set; }

        [JsonProperty("nodeName")]
        public string NodeName { get; set; }

        [JsonProperty("securityContext")]
        public SecurityContext SecurityContext { get; set; }

        [JsonProperty("schedulerName")]
        public string SchedulerName { get; set; }

        [JsonProperty("tolerations")]
        public Toleration[] Tolerations { get; set; }
    }

    public partial class Container
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("ports")]
        public Port[] Ports { get; set; }

        [JsonProperty("resources")]
        public Resources Resources { get; set; }

        [JsonProperty("volumeMounts")]
        public VolumeMount[] VolumeMounts { get; set; }

        [JsonProperty("terminationMessagePath")]
        public string TerminationMessagePath { get; set; }

        [JsonProperty("terminationMessagePolicy")]
        public string TerminationMessagePolicy { get; set; }

        [JsonProperty("imagePullPolicy")]
        public string ImagePullPolicy { get; set; }
    }

    public partial class Port
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("containerPort")]
        public long ContainerPort { get; set; }

        [JsonProperty("protocol")]
        public string Protocol { get; set; }
    }

    public partial class Resources
    {
        [JsonProperty("requests")]
        public Requests Requests { get; set; }
    }

    public class Requests
    {
        [JsonProperty("cpu")]
        public string Cpu { get; set; }

        [JsonProperty("memory")]
        public string Memory { get; set; }
    }

    public class VolumeMount
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }

        [JsonProperty("mountPath")]
        public string MountPath { get; set; }
    }

    public class SecurityContext
    {
    }

    public class Toleration
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty("effect")]
        public string Effect { get; set; }

        [JsonProperty("operator", NullValueHandling = NullValueHandling.Ignore)]
        public string Operator { get; set; }

        [JsonProperty("tolerationSeconds", NullValueHandling = NullValueHandling.Ignore)]
        public long? TolerationSeconds { get; set; }
    }

    public class Volume
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("secret")]
        public Secret Secret { get; set; }
    }

    public class Secret
    {
        [JsonProperty("secretName")]
        public string SecretName { get; set; }

        [JsonProperty("defaultMode")]
        public long DefaultMode { get; set; }
    }

    public class Status
    {
        [JsonProperty("phase")]
        public string Phase { get; set; }

        [JsonProperty("qosClass")]
        public string QosClass { get; set; }

        [JsonProperty("conditions", NullValueHandling = NullValueHandling.Ignore)]
        public PodCondition[] Conditions { get; set; }
        [JsonProperty("containerStatuses", NullValueHandling = NullValueHandling.Ignore)]
        public ContainerStatus[] ContainerStatuses { get; set; }
    }

    public class PodCondition
    {
        // partial class implementation!!

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class ContainerStatus
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("ready")]
        public bool Ready { get; set; }

        [JsonProperty("restartCount")]
        public int RestartCount { get; set; }

        [JsonProperty("state")]
        public ContainerState State { get; set; }
    }

    public class ContainerState
    {
        [JsonProperty("waiting", NullValueHandling = NullValueHandling.Ignore)]
        public ContainerStateWaiting Waiting { get; set; }

        [JsonProperty("running", NullValueHandling = NullValueHandling.Ignore)]
        public ContainerStateRunning Running { get; set; }

        [JsonProperty("terminated", NullValueHandling = NullValueHandling.Ignore)]
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

