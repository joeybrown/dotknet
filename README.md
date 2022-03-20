# dotknet
`/dɒtkə-ˈnɛt/` or `/dɒtkeɪ-ˈnɛt/`?

Dotknet is a tool for building simple dotnet container images without docker or dockerfiles.

It's basically `dotnet publish && crane append`.[^1] It is inspired by [crane](https://github.com/google/go-containerregistry/blob/main/cmd/crane/doc/crane_append.md), [ko](https://github.com/google/ko), [jib](https://github.com/GoogleContainerTools/jib), & [buildpacks](https://buildpacks.io/).

It is ideal for environments where installing docker or maintaining dockerfiles is unnecessary, undesired, or untenable.

# Current Status
Unstable, not working at all, under active development.

[^1]: https://github.com/ImJasonH/ImJasonH/tree/main/articles/moving-and-building-images#what-to-do-instead-1