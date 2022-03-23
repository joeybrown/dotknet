#!/bin/bash

# Requirements:
# - dotnet (in order to publish app)
# - docker (in order to run a local image registry
#           & inspect/run resulting image.
#           docker is *not* required for building the image)
# - crane  (for appending & mutating docker image)

export BASE_IMAGE="gcr.io/distroless/base-debian11"
export TARGET_IMAGE="localhost:5000/foo"
export PROJECT_PATH="../src/Dotknet.Cli"
export PUBLISH_PATH="./output/app"
export ARTIFACT_PATH="./output/"
export OS="linux"

# spin up local registry if not already running

# [[ $(docker ps -f "name=registry" --format '{{.Names}}') == registry ]] || docker run -d -p 5000:5000 --name registry registry:2


# # dotnet publish
# dotnet publish $PROJECT_PATH -c Release -o $PUBLISH_PATH --os $OS --arch x64 --self-contained true /p:PublishSingleFile=true



# # crane append
TARGET_IMAGE_REF=$(crane append -f <(tar -f - -c $ARTIFACT_PATH) -t $TARGET_IMAGE -b $BASE_IMAGE --insecure)

# crane mutate
crane mutate $TARGET_IMAGE_REF --entrypoint=./output/entrypoint.sh
