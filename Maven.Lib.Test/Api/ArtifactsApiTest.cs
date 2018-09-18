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

        [TestInitialize()]
         public void MyTestInitialize() {

            _artifactVersionsRepositoryMock = new Mock<IArtifactsRepository>();
            IArtifactsStorage artifactsStorage = null;
            IRepositoryEntitiesRepository repositoriesRepository = null;
            IServicesMapper servicesMapper = null;
            IHashCalculator hashCalculator = null;
            ITransactionManager transactionManager = null;
            IReleaseArtifactRepository releaseArtifactRepository = null;
            IPomApi pomApi = null;
            IMetadataApi metadataApi = null;
            _target = new ArtifactsApi(
                _artifactVersionsRepository, artifactsStorage, repositoriesRepository, servicesMapper,
                hashCalculator, transactionManager, releaseArtifactRepository,
                pomApi, metadataApi);

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
                Meta="metadata.xml"
            };
            Assert.IsFalse(_target.CanHandle(mi));
        }
        #endregion


        [TestMethod]
        public void ISPTRetrieveFile()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar"
            };
            var result = _target.Retrieve(mi);
            Assert.IsNotNull(result);
        }
    }
}
