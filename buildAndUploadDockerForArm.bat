cd src\PingRService
dotnet publish -c Release --output publish/ -r linux-arm --no-self-contained
docker build --tag sepppenner/pingrservice-arm:1.0.4 -f Dockerfile.armv7 .
docker login -u sepppenner -p "%DOCKERHUB_CLI_TOKEN%"
docker push sepppenner/pingrservice-arm:1.0.4
@ECHO.Build successful. Press any key to exit.
pause