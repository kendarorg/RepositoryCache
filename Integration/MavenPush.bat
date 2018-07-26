@echo off
REM maven_exes\lib\slf4j-api-1.7.25.pom
SET APIKEY=E7446C12-BD80-4272-3332-09914BE6EBC8
SET NUROOT=http://localhost:9088/maven

cd maven_exes
SET CUDIR=%CD%
cd bin

echo Maven.3.5.4, Upload of simple file

mvn deploy:deploy-file -DgroupId=org -DartifactId=slf4j -Dversion=1.7.25 -DgeneratePom=true -Dpackaging=jar -DrepositoryId=local -Durl=%NUROOT% -DpomFile=%CUDIR%\lib\slf4j-api-1.7.25.pom -Dfile=%CUDIR%\lib\slf4j-api-1.7.25.jar

cd ..

cd ..

PUT /maven/log4j/project/1.2.17/project-1.2.17.jar

PUT http://localhost:9088/maven/log4j/project/1.2.17/project-1.2.17.jar
PUT http://localhost:9088/maven/log4j/project/1.2.17/project-1.2.17.pom