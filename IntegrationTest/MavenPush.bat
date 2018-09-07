@echo off
REM maven_exes\lib\slf4j-api-1.7.25.pom
SET APIKEY=E7446C12-BD80-4272-3332-09914BE6EBC8
SET NUROOT=http://localhost:9088/maven.local

cd maven_exes

SET GROUPID=com.google.api
SET ARTIFACTID=api-common
SET AVERSION=1.6.0
SET ARTIPATH=%CD%\upload\%ARTIFACTID%-%AVERSION%

SET CUDIR=%CD%
cd bin

echo Maven.3.5.4, Upload of simple file

REM mvn deploy:deploy-file --fn -DgroupId=%GROUPID% -DartifactId=%ARTIFACTID% -Dversion=%AVERSION% -DgeneratePom=true -Dpackaging=jar -DrepositoryId=local -Durl=%NUROOT% -Dfile=%ARTIPATH%.jar 

start /WAIT /B mvn deploy:deploy-file -DpomFile=%ARTIPATH%.pom  -DgroupId=%GROUPID% -DartifactId=%ARTIFACTID% -Dversion=%AVERSION% -DgeneratePom=false -Dpackaging=jar -DrepositoryId=local -Durl=%NUROOT% -Dfile=%ARTIPATH%.jar -Dfiles=%ARTIPATH%-javadoc.jar,%ARTIPATH%-sources.jar -Dclassifiers=javadoc,sources -Dtypes=jar,jar


pause

cd ..

cd ..
pause
REM PUT /maven/log4j/project/1.2.17/project-1.2.17.jar

REM PUT http://localhost:9088/maven/log4j/project/1.2.17/project-1.2.17.jar
REM PUT http://localhost:9088/maven/log4j/project/1.2.17/project-1.2.17.pom

REM https://repo.maven.apache.org/maven2/     org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.jar
REM http://localhost:9088/maven.local/publish/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.jar