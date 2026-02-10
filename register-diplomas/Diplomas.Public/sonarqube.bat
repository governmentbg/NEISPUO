REM SonarQube key  7df15e9ec63ffa76733b3c3b3d0c3957f308aade

REM dotnet tool install --global dotnet-sonarscanner
dotnet sonarscanner begin /k:"rd" /d:sonar.host.url="http://20.82.147.222"  /d:sonar.login="7df15e9ec63ffa76733b3c3b3d0c3957f308aade"
dotnet build
dotnet sonarscanner end /d:sonar.login="7df15e9ec63ffa76733b3c3b3d0c3957f308aade"