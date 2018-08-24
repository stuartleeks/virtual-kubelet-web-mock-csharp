using System;

namespace vk_web_mock.Services
{
    public class NodeCondition
    {
        public NodeConditionType Type { get; set; }
        public NodeConditionStatus Status { get; set; }
        public DateTime LastHeartbeatTime { get; set; }
        public DateTime LastTransitionTime { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
    }

    public enum NodeConditionType
    {
        // NodeReady means kubelet is healthy and ready to accept pods.
        Ready,
        // NodeOutOfDisk means the kubelet will not accept new pods due to insufficient free disk
        // space on the node.
        OutOfDisk,
        // NodeMemoryPressure means the kubelet is under pressure due to insufficient available memory.
        MemoryPressure,
        // NodeDiskPressure means the kubelet is under pressure due to insufficient available disk.
        DiskPressure,
        // NodePIDPressure means the kubelet is under pressure due to insufficient available PID.
        PIDPressure,
        // NodeNetworkUnavailable means that network for the node is not correctly configured.
        NetworkUnavailable,
        // NodeKubeletConfigOk indicates whether the kubelet is correctly configured
        KubeletConfigOk,
    }
    public enum NodeConditionStatus
    {
        True,
        False,
        Unknown
    }

    public class NodeAddress
    {
        public NodeAddressType Type { get; set; }
        public string Address { get; set; }
    }
    public enum NodeAddressType
    {
        HostName,
        ExternalIP,
        InternalIP,
        ExternalDNS,
        InternalDNS
    }
}