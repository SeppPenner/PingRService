cd src\PingRService
dotnet publish -c Release --output publish/ -r linux-x64 --no-self-contained
docker build --tag sepppenner/pingrservice:1.0.2 -f Dockerfile .
docker login -u sepppenner -p "%DOCKERHUB_CLI_TOKEN%"
docker push sepppenner/pingrservice:1.0.2
@ECHO.Build successful. Press any key to exit.
pause