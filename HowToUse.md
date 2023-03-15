## Basic usage

### JSON configuration (Adjust this to your needs)
```json
{
    "AllowedHosts": "*",
    "PingRService": {
        "ServiceDelayInMilliSeconds": 30000,
        "HeartbeatIntervalInMilliSeconds": 30000,
        "DomainsToCheck": [
            {
                "Domain": "https://www.google.com",
                "LogMessage": "Google is down.",
                "InstanceKey": "Google",
                "CheckCertificateExpiry": true,
                "CertificateExpiryCheckInterval": "01:00:00"
            }
        ],
        "TelegramBotToken": "111111111:AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
        "TelegramChatId": "2222222"
    }
}
```

### Run this project in Docker from the command line (Examples for Powershell, but should work in other shells as well):

1. Change the directory
    ```bash
    cd ..\src\PingRService
    ```

2. Publish the project
    ```bash
    dotnet publish -c Release --output publish/
    ```

3. Build the docker file:
    * `dockerhubuser` is a placeholder for your docker hub username, if you want to build locally, just name the container `simplemqttserver`
    * `1.0.2` is an example version tag, use it as you like
    * `-f Dockerfile .` (Mind the `.`) is used to specify the dockerfile to use

    ```bash
    docker build --tag dockerhubuser/pingrservice:1.0.2 -f Dockerfile .
    ```

4. Push the project to docker hub (If you like)
    * `dockerhubuser` is a placeholder for your docker hub username, if you want to build locally, just name the container `pingrservice`
    * `1.0.2` is an example version tag, use it as you like

    ```bash
    docker push dockerhubuser/pingrservice:1.0.2
    ```

5. Run the container:
    * `-d` runs the docker container detached (e.g. no logs shown on the console, is needed if run as service)
    * `--name="pingrservice"` gives the container a certain name
    * `-v "/home/config.json:/app/appsettings.json"` sets the path to the external configuration file (In the example located under `/home/appsettings.json`) to the container internally
    
    ```bash
    docker run -d --name="pingrservice" -v "/home/appsettings.json:/app/appsettings.json" --restart=always dockerhubuser/pingrservice:1.0.2
    ```

6. Check the status of all containers running (Must be root)
    ```bash
    docker ps -a
    ```

7. Stop a container
    * `containerid` is the id of the container obtained from the `docker ps -a` command
    ```bash
    docker stop containerid
    ```

8. Remove a container
    * `containerid` is the id of the container obtained from the `docker ps -a` command
    ```bash
    docker rm containerid
    ```