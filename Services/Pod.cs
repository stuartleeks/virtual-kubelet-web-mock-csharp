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

    public partial class Requests
    {
        [JsonProperty("cpu")]
        public string Cpu { get; set; }

        [JsonProperty("memory")]
        public string Memory { get; set; }
    }

    public partial class VolumeMount
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }

        [JsonProperty("mountPath")]
        public string MountPath { get; set; }
    }

    public partial class SecurityContext
    {
    }

    public partial class Toleration
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

    public partial class Volume
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("secret")]
        public Secret Secret { get; set; }
    }

    public partial class Secret
    {
        [JsonProperty("secretName")]
        public string SecretName { get; set; }

        [JsonProperty("defaultMode")]
        public long DefaultMode { get; set; }
    }

    public partial class Status
    {
        [JsonProperty("phase")]
        public string Phase { get; set; }

        [JsonProperty("qosClass")]
        public string QosClass { get; set; }
    }
}

