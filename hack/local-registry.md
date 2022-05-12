[reference](https://docs.docker.com/registry/)

```
$ docker run -d -p 5000:5000 --name registry registry:2
```

```
$ docker start registry
```

```
$ docker container stop registry && docker container rm -v registry
```
