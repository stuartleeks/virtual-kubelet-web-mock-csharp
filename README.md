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

## Running the API via docker

To run the Mock API via docker run

```bash
docker run -p 5000:80 stuartleeks/vk-web-mock
```

This will expose the API on http://localhost:5000.

## Connecting the API with Virtual Kubelet locally

To connect Virtual Kubelet to the API, set the `WEB_ENDPOINT_URL` environment variable to `http://localhost:5000/` (or whatever you have exposed the API as)

```bash
export WEB_ENDPOINT_URL=http://localhost:5000/
```

Then run `virtual-kubelet` with the `--provider web` switch. This will run Virtual Kubelet on your local machine. It will connect to Kubernetes based on the kubectl config, and connect to the API defined in the `WEB_ENDPOINT_URL` environment variable

## Visualising the API state

You can use `kubectl` commands to query running pods, but you can also run [Virtual Kubelet Web UI](https://github.com/stuartleeks/virtual-kubelet-web-ui) to connect to the API directly and show details of the running pods and their status.


