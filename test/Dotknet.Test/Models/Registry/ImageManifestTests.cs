using System;
using Xunit;

namespace Dotknet.Test.Models.Registry;

public class ImageManifestTests {

  [Fact]
  public void Create_Manifest_From_Docker_Manifest_2_2() {
    const string manifestJson = "{\n   \"schemaVersion\": 2,\n   \"mediaType\": \"application/vnd.docker.distribution.manifest.v2+json\",\n   \"config\": {\n      \"mediaType\": \"application/vnd.docker.container.image.v1+json\",\n      \"size\": 2793,\n      \"digest\": \"sha256:54fe74496e3810bb0cbb73fb041873654680ce46038fdf3e9ae7e625cf26530e\"\n   },\n   \"layers\": [\n      {\n         \"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\n         \"size\": 31379476,\n         \"digest\": \"sha256:214ca5fb90323fe769c63a12af092f2572bf1c6b300263e09883909fc865d260\"\n      },\n      {\n         \"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\n         \"size\": 14966772,\n         \"digest\": \"sha256:562f2b48c06c589de5b8b2f72eb9a0040bc6eab0d479295085ad96036c1da020\"\n      },\n      {\n         \"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\n         \"size\": 31620678,\n         \"digest\": \"sha256:bdd7874d464602a566b7f5087754c80ca1f5d01d279b251aceec9836d66e717e\"\n      },\n      {\n         \"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\n         \"size\": 154,\n         \"digest\": \"sha256:8aa9b64f5310895753358c2676ade6be98179e6b05dadd87adf51f8571250802\"\n      }\n   ]\n}";
    // var manifest = ImageManifest.FromContent(manifestJson);
  }
}