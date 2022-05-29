# dotknet
`/dɒtkeɪ-ˈnɛt/`

## ⚠️ Development Status - Proof of Concept

Dotknet is a tool for building simple dotnet docker images (locally or in a CI/CD pipeline) without a docker daemon or dockerfiles. Reference this [article](https://github.com/ImJasonH/ImJasonH/tree/main/articles/moving-and-building-images) for reasons why building images without the docker daemon might be desirable.

Dotknet can be compared to (and is inspired by) [ko](https://github.com/google/ko) & [jib](https://github.com/GoogleContainerTools/jib).

## Proof of Concept
This project is under active development. It should be used for demonstration only at this point.

In a controlled development environment, we're able to do the following:
- Build a dotnet application from source.
- Package that source code into a tarball layer format.
- Add the layer to a base image to make a new image.
- Upload the new image to a local registry.

Areas to improve:
- Only supports linux amd64
  - Should be pretty straight-forward to build for other environments.
- Config is not modified at all
  - This is the next step I'm interested in so we can have something useable.
  - Once we're generating a usable image, the project will move to from proof of concept to alpha.
- No caching
  - There are lots of places to cache that will *significantly* improve the experience
- No image maintenance (rebase)
