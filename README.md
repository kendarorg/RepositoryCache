# RepositoryCache
Nuget, Maven, NPM local cache server

* it is possible to push on remote repos
* tree view only for maven
* maven and npm search similar to nuget

## Common API

* _GET /v1/index.json_ Retrieve all the existing databases

## Maven API

* _PUT /{repo}/{path}/{packageId}/{version}/{packageId}.{version}.\[jar|pom\](.\[md5|asc|sha1\]))_	To upload package, pom and the signatures
* _PUT /{repo}/{path}/{packageId}/maven-metadata.xml(.\[md5|asc|sha1\]))_	To upload the maven metadata for the uploaded package

## Nuget API

* _GET /{repo}/v3/index.json_	List of avaliable APIs
* _POST /{repo}/v2/publish_	Publish package
* _DELETE /{repo}/v2/publish_	Delist package
* _PUT /{repo}/v2/publish_	Relist package
* _GET /{repo}/v3/registration/{semver}/{packageid}/index.json_ Registration pages for the specific package and semver
* _GET /{repo}/v3/registration/{semver}/{packageid}/page/{from}/{to}.json_ List of registrations paged by version
* _GET /{repo}/v3/registration/{semver}/{packageid}/{version}.json_ Registration for the specific package version and semver
* _GET /{repo}/v3/query_ Search the packages
* _GET /{repo}/v3/container/{idLower}/{versionLower}/{fullversion}.nupkg_ Download the package
* _GET /{repo}/v3/container/{packageid}/index.json_ List the versions for the package
* _GET /{repo}/v3/catalog/data/{date}/{fullPackage}.json_ Retrieve alla data included dependencies for package
* _GET /{repo}/custom/load_ Load nupkg from an existing directory



