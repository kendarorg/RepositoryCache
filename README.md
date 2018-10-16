# RepositoryCache
Nuget, Maven local cache server

* it is possible to push on remote repos
* tree view only for maven
* maven search similar to nuget

## Common API

* _GET /global/index.json_ Retrieve all the existing databases and entry points

## Maven API

Support for timestamp based and single snapshot

* _PUT /{repo}/{path}/{packageId}/{version}/{packageId}.{version}(-{classifier}.\[{extension}|pom\](.\[md5|sha1\]))_	To upload package, pom and the signatures
* _PUT /{repo}/{path}/{packageId}/{version}/maven-metadata.xml(.\[md5|sha1\]))_	To upload the maven metadata for the specific version
* _PUT /{repo}/{path}/{packageId}/maven-metadata.xml(.\[md5|sha1\]))_	To upload the maven metadata for the uploaded package

* _PUT /{repo}/{path}/{packageId}/{version}/{packageId}.{version}/tags To upload the tags
* _GET /{repo}/{path}/{packageId}/{version}/{packageId}.{version}/tags To retrieve the tags

* _GET /{repo}/..._	Standard maven browse

* _GET /{repo}/search Search according to [https://search.maven.org/classic/] on the [https://search.maven.org/solrsearch/select] api
	* ?rows=1 number of rows
	* ?skip=1 number of rows to skip
	* ?core=gav "Get All Versions"
	* ?wt=json Xml Version not supported
	* ?q= (mandatory)
		* g:text group exact (e.g. com.google.api)
		* v:text version exact (e.g. 1.2) 
		* l:text classifier exact (e.g. javadoc)
		* p:text packaging exact (e.g. jar)
		* a:text artifactId exact (e.g. api-common)
		* c:text className partial with package (e.g. est.A to search for com.test.Advance)
		* 1:text sha1 exact
		* 5:text md5 exact
		* tags:text tag exact (e.g. core)
		* timestamp:yyyyMMddHHmmss exact (e.g. 20181231235959)
		* can put a free text and will search for all parts

## Nuget API

* _GET /{repo}/v3/index.json_	List of avaliable APIs
* _POST /{repo}/v2/publish_	Publish package
* _DELETE /{repo}/v2/publish_	Delist package
* _PUT /{repo}/v2/publish_	Relist package
* _GET /{repo}/v3/registration/{semver}/{packageid}/index.json_ Registration pages for the specific package and semver
* _GET /{repo}/v3/registration/{semver}/{packageid}/page/{from}/{to}.json_ List of registrations paged by version
* _GET /{repo}/v3/registration/{semver}/{packageid}/{version}.json_ Registration for the specific package version and semver
* _GET /{repo}/v3/query Search the packages
	* ?q=... the query
		* id:text  partial (e.g. Json)
		* packageid:text exact (e.g. Newtonsoft.Json)
		* title:text partial
		* version:text exact (e.g. 1.2-alpha)
		* tags:json exact
		* author:text exact
		* owner:text exact
		* description:text partial
		* summary:text partial
		* free text 
	* ?prerelease=true if should consider prereleases
	* ?semverlevel=[2.0.0|1.0.0] the semantic version level to consider
	* ?skip=1 the rows to skip
	* ?take=1 the rows to return
	* ?supportedFramework=.NetFramework,Version=v5.2.1 the  framework for which the user is searching package
* _GET /{repo}/v3/container/{idLower}/{versionLower}/{fullversion}.nupkg_ Download the package
* _GET /{repo}/v3/container/{packageid}/index.json_ List the versions for the package
* _GET /{repo}/v3/catalog/data/{date}/{fullPackage}.json_ Retrieve alla data included dependencies for package

* _GET /{repo}/custom/load Load nupkg from an existing directory on the server

## Thanks to:

* https://docs.microsoft.com/en-us/nuget/reference/target-frameworks
* https://github.com/joelverhagen/NuGetTools