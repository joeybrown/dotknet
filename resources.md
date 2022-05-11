## Image Formats

- [Remote](https://github.com/google/go-containerregistry/blob/main/pkg/v1/remote/README.md) (lives on a registry)
  - Based on the [OCI distribution Spec](https://github.com/opencontainers/distribution-spec/blob/main/spec.md)
- [Tarball](https://github.com/google/go-containerregistry/tree/main/pkg/v1/tarball) (`docker load`)
  - Useful for backup, daemon
- [daemon](https://docs.docker.com/engine/reference/commandline/save/) (`docker save`)
  - Legacy format no dotknet support