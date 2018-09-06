using System;
using Maven.News;
using MavenProtocol.Apis;
using MavenProtocol.News;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MavenProtocol
{
    [TestClass]
    public class MavenApiRewrite
    {
        [TestMethod]
        public void ISPToHandleArtifactLevelMetadata()
        {
            IDummyGenerator dummyGenerator = null;
            IReleaseArtifactRepository artifactVersionsRepository = null;
            IMetadataApi target = new MetadataApi(
                dummyGenerator,
                artifactVersionsRepository
                );
            var mi = new MavenIndex
            {
                Content = null,
                RepoId = Guid.NewGuid(),
                ArtifactId = "test",
                Group = new string[] { "org", "kendar" },
                Meta = "maven-metadata.xml",
            };

            Assert.IsTrue(target.CanHandle(mi));
            MavenMetadataXml result = target.Retrieve(mi);
        }

        [TestMethod]
        public void ISPToCreateDummyMetadata()
        {
            IMetadataRepository metadataRepository = null;
            IDummyGenerator target = new DummyGenerator(
                metadataRepository
                );
            var result = target.GetArtifactMetadata(new string[] { "org", "kendar" }, "test");

            Assert.AreEqual(result.Group,"org.kendar");
            Assert.AreEqual(result.ArtifactId, "test");
            Assert.IsFalse(result.Initialized);
        }
    }
}
