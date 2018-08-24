# Virtual Kubelet Web Provider Mock Implementation
This project is a sample API implementation for the [Virtual Kubelet](https://github.com/virtual-kubelet/virtual-kubelet) [web provider](https://github.com/virtual-kubelet/virtual-kubelet/tree/master/providers/web)

This API simply stores the a list of the pods that it has been requested to create, marks them as started and serves up their status

```
+----------------+         +---------------------------+          +------------------------------+
|                |         |                           |   HTTP   |                              |
|   Kubernetes   | <-----> |   Virtual Kubelet: Web    | <------> |   This sample/mock API       |
|                |         |                           |          |                              |
+----------------+         +---------------------------+          +------------------------------+
```



TODO - instructions for building/running