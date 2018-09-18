using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maven.News;
using Maven.Services;
using MultiRepositories.Repositories;
using MavenProtocol;
using Repositories;
using MavenProtocol.News;
using System.Text.RegularExpressions;
using MavenProtocol.Apis;
using Moq;

namespace Maven.Lib.Test.Api
{
    /// <summary>
    /// Summary description for ArtifactsApiTest
    /// </summary>
    [TestClass]
    public class ArtifactsApiTest
    {
        public ArtifactsApiTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;
        private ArtifactsApi _target;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private Mock<IArtifactsRepository> _artifactVersionsRepositoryMock;

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 

        IArtifactsRepository _artifactVersionsRepository = null;
        private Mock<IRepositoryEntitiesRepository> _repositoriesRepositoryMock;
        IRepositoryEntitiesRepository _repositoriesRepository = null;
        IArtifactsStorage _artifactsStorage = null;
        Mock<IArtifactsStorage> _artifactsStorageMock = null;
        IHashCalculator _hashCalculator = null;
        Mock<IHashCalculator> _hashCalculatorMock = null;
        ITransactionManager _transactionManager = null;

        IReleaseArtifactRepository _releaseArtifactRepository = null;
        Mock<IReleaseArtifactRepository> _releaseArtifactRepositoryMock = null;
        IPomApi _pomApi = null;
        Mock<IPomApi> _pomApiMock = null;
        IMetadataApi _metadataApi = null;
        Mock<IMetadataApi> _metadataApiMock = null;
        [TestInitialize()]
        public void MyTestInitialize()
        {

            _artifactVersionsRepositoryMock = new Mock<IArtifactsRepository>();
            _artifactVersionsRepository = _artifactVersionsRepositoryMock.Object;
            _repositoriesRepositoryMock = new Mock<IRepositoryEntitiesRepository>();
            _repositoriesRepository = _repositoriesRepositoryMock.Object;
            _artifactsStorageMock = new Mock<IArtifactsStorage>();
            _artifactsStorage = _artifactsStorageMock.Object;

            _hashCalculatorMock = new Mock<IHashCalculator>();
            _hashCalculator = _hashCalculatorMock.Object;
            var transactionManagerMock = new Mock<ITransactionManager>();
            transactionManagerMock.Setup(a => a.BeginTransaction()).Returns(new Mock<ITransaction>().Object);
            _transactionManager = transactionManagerMock.Object;
            _releaseArtifactRepositoryMock = new Mock<IReleaseArtifactRepository>();
            _releaseArtifactRepository = _releaseArtifactRepositoryMock.Object;
            _pomApiMock = new Mock<IPomApi>();
            _pomApi = _pomApiMock.Object;
            _metadataApiMock = new Mock<IMetadataApi>();
            _metadataApi = _metadataApiMock.Object;

            IServicesMapper servicesMapper = null;

            _target = new ArtifactsApi(
                _artifactVersionsRepository, _artifactsStorage, _repositoriesRepository, servicesMapper,
                _hashCalculator, _transactionManager, _releaseArtifactRepository,
                _pomApi, _metadataApi);

        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        #region CAN_HANDLE
        [TestMethod]
        public void ITSPToHandleStandardRequests()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "ear"
            };
            Assert.IsTrue(_target.CanHandle(mi));
        }
        [TestMethod]
        public void ITSNPToHandlePomRequests()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "pom",
            };
            Assert.IsFalse(_target.CanHandle(mi));
        }
        [TestMethod]
        public void ITSNPToHandleMEtaRequests()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "war",
                Meta = "metadata.xml"
            };
            Assert.IsFalse(_target.CanHandle(mi));
        }
        #endregion


        [TestMethod]
        public void ISNBPToRetrieveNotExistingPackage()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar"
            };
            var result = _target.Retrieve(mi);
            Assert.IsNull(result);
        }
        [TestMethod]
        public void ISBPToRetrievePackage()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid()
            };

            var artifactVersion = new ArtifactEntity();
            _artifactVersionsRepositoryMock.Setup(a => a.GetSingleArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(),
                It.Is<string>(b => b == mi.Version), It.IsAny<string>(), It.Is<string>(b => b == mi.Extension),
                It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>())).
                Returns(artifactVersion);

            var result = _target.Retrieve(mi);

            _artifactsStorageMock.Verify(a => a.Load(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>()), Times.Exactly(1));
            _artifactVersionsRepositoryMock.Verify(a => a.GetSingleArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(),
                It.Is<string>(b => b == mi.Version), It.IsAny<string>(), It.Is<string>(b => b == mi.Extension),
                It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void ISBPToGeneratePackageLocally()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };



            var result = _target.Generate(mi, false);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(1));
            _artifactVersionsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(1));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(1));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ISBPToGeneratePackageLocallyChecksum()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                Checksum  = "md5",
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };



            var result = _target.Generate(mi, false);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(1));
            _artifactVersionsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(1));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(1));

            Assert.IsNotNull(result);
        }



        [TestMethod]
        public void ISBPToGenerateSnapshotPackageLocally()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                IsSnapshot = true,
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };



            var result = _target.Generate(mi, false);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(1));
            _artifactVersionsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(1));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(0));

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void ISBPToGeneratePackageRemote()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };



            var result = _target.Generate(mi, true);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(1));
            _artifactVersionsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(1));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(1));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ISBPToUpdateRemotePackage()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ISBPToUpdateTimestampedSnapshot()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ISBNotPToUpdateFixedSnapshot()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ISBPToGeneratePomWhenRemote()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ISBPToChangeTheReleaseArtifactsWhenAddingMoreRecentVersion()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ISBPToChangeTheTimestampedSnapshotArtifactsWhenAddingMoreRecentVersion()
        {
            Assert.Inconclusive();
        }
    }



}
