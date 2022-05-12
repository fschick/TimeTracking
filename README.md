# FS.TimeTracking

Time tracking for freelancers

## Demo

https://demo.timetracking.schick-software.de/

## Status

Version 1.0.0 released

Better documentation will follow in the next days

Docker images will follow in the next days , see docker build script `Build/make_publish_docker.docker`until then

## Roadmap

### Reports

Activity reports as PDF for customers are in development and will follow. Maybe it will become a paid feature. 

### User management

Currently no user management / login exists. It's a long time since I've done this things and my knowledge isn't up to date. If you like to help please contact me.

## Configuration files

##### Application / Service / Kestrel configuration

./config/FS.TimeTracking.config.json

##### Logging configuration

./config/FS.TimeTracking.config.nlog

##### OpenAPI specification

./FS.TimeTracking.openapi.json

## Run from command line

```shell
# Windows
.\FS.TimeTracking.exe

# Linux
chmod +x ./FS.TimeTracking
./FS.TimeTracking
```

## Install as service on Windows

```bash
# Copy the content of publish folder to suitable location
mkdir C:\services\FS.TimeTracking
copy . C:\Services\FS.TimeTracking

# When the service runs from program folder adjust path to log files (programm folder isn't writeable!)
notepad "C:\services\FS.TimeTracking\FS.TimeTracking.config.nlog"

# Install and run as windows service
C:\Services\FS.TimeTracking\FS.TimeTracking.WindowsService.Install.bat

# Uninstall service
C:\Services\FS.TimeTracking\FS.TimeTracking.WindowsService.Uninstall.bat
```

## Install as service on Linux

```bash
# Copy files
cp -R opt/FS.TimeTracking /opt/
cp etc/systemd/system/FS.TimeTracking.service /etc/systemd/system/
chmod +x /opt/FS.TimeTracking/bin/FS.TimeTracking

# Create system user/group
useradd -m dotnetuser --user-group
# Adjust user/group
vi /etc/systemd/system/FS.TimeTracking.service
chown -R dotnetuser:dotnetuser /opt/FS.TimeTracking/
chmod -R o= /opt/FS.TimeTracking/

# Test service
/opt/FS.TimeTracking/bin/FS.TimeTracking

# Start service
systemctl daemon-reload
systemctl start FS.TimeTracking.service

# Get service status
systemctl status FS.TimeTracking.service
journalctl -u FS.TimeTracking.service

# If you want your service to start when the machine does then you can use
systemctl enable FS.TimeTracking.service
```

## Development

### Pre requirements

[.NET 6 SDK](https://dotnet.microsoft.com/en-us/download)

[Node.js 16.x](https://nodejs.org/en/)

[OpenJDK](https://docs.microsoft.com/en-us/java/openjdk/download) (required to generate Angular REST client via OpenAPI Generator)

### Run

Download or clone repository

```bash
git clone https://github.com/fschick/TimeTracking.git
cd TimeTracking
```

Install NPM packages

```bash
cd FS.TimeTracking.UI/FS.TimeTracking.UI.Angular
npm install
cd ../..
```

Run server

```
dotnet run --project FS.TimeTracking/FS.TimeTracking/FS.TimeTracking.csproj
```

Run UI

```
cd FS.TimeTracking.UI/FS.TimeTracking.UI.Angular
npm run start
```

Open web browser:

http://localhost:4200/

### Publish

See publish script `Build/make_publish.ps1`
