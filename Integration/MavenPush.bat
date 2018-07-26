@echo off

SET APIKEY=E7446C12-BD80-4272-3332-09914BE6EBC8
SET NUROOT=http://localhost:9088/maven

cd maven_exes
SET CUDIR=%CD%
cd bin

echo Maven.3.5.4, Upload of simple file

mvn deploy:deploy-file -DgroupId=log4j -DartifactId=project -Dversion=1.2.17 -DgeneratePom=true -Dpackaging=jar -DrepositoryId=nexus -Durl=%NUROOT% -DpomFile=%CUDIR%\log4j-1.2.17.pom -Dfile=%CUDIR%\log4j-1.2.17.jar

cd ..

cd ..

PUT /maven/log4j/project/1.2.17/project-1.2.17.jar

PUT http://localhost:9088/maven/log4j/project/1.2.17/project-1.2.17.jar
PUT http://localhost:9088/maven/log4j/project/1.2.17/project-1.2.17.pom