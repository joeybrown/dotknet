# dotknet
`/dɒtkeɪ-ˈnɛt/`

## ⚠️ Under Active Development

Dotknet is a tool for building simple dotnet docker images (locally or in a CI/CD pipeline) without a docker daemon or dockerfiles. Reference this [article](https://github.com/ImJasonH/ImJasonH/tree/main/articles/moving-and-building-images) for reasons why building images without the docker daemon might be desirable.

Dotknet can be compared to (and is inspired by) [ko](https://github.com/google/ko) & [jib](https://github.com/GoogleContainerTools/jib).

## Images Implementations

Images are an abstract resource that a container runtime uses to run a container. There are a few implementations of containers:
- [remote](https://github.com/google/go-containerregistry/blob/main/pkg/v1/remote/README.md) (registry image)
- [tarball](https://github.com/google/go-containerregistry/tree/main/pkg/v1/tarball)
- [daemon](https://docs.docker.com/engine/reference/commandline/save/) (`docker save`)
