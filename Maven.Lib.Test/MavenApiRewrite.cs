using System;
using Maven.News;
using MavenProtocol.Apis;
using MavenProtocol.News;
using NUnit.Framework;

namespace MavenProtocol
{
    [TestFixture]
    public class MavenApiRewrite
    {
        [Test]
        public void ISPToHandleArtifactLevelMetadata()
        {
            
            //IReleaseArtifactRepository artifactVersionsRepository = null;
            /*IMetadataApi target = new MetadataApi(
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
            MavenMetadataXml result = target.Retrieve(mi);*/
        }

        [Test]
        public void ISPToCreateDummyMetadata()
        {
            /*IMetadataRepository metadataRepository = null;
            IDummyGenerator target = new DummyGenerator(
                metadataRepository
                );
            var result = target.GetArtifactMetadata(new string[] { "org", "kendar" }, "test");

            Assert.AreEqual(result.Group,"org.kendar");
            Assert.AreEqual(result.ArtifactId, "test");
            Assert.IsFalse(result.Initialized);*/
        }
    }
}
