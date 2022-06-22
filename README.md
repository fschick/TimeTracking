# FS.TimeTracking

Time tracking for freelancers

## Demo

https://demo.timetracking.schick-software.de/

## Supported operating systems

Supported operation systems are Windows 10 + / Windows Server 2016 + / Debian based Linux

Other Operating systems might worked but are untested

## Supported databases

**SQL Server 2019+**: Fully tested

**MySQL / MariaDB**: Fully tested

**SQLite**: Partially tested, demo runs with it

**PostgreSQL**: Mainly untested

## Installation

### Run as docker container

```bash
docker run -d -p <port_on_host>:5000 \
	--name timetracking \
	-e TimeTracking__Database__Type='<MySql|SqlServer|Sqlite|PostgreSql>' \
	-e TimeTracking__Database__ConnectionString=<connection string> \
	schicksoftware/timetracking
```

### Run from command line

```shell
# Windows
.\FS.TimeTracking.exe

# Linux
chmod +x ./FS.TimeTracking
./FS.TimeTracking
```

### Install as Windows service

```bash
# Copy the content of publish folder to suitable location
mkdir C:\services\TimeTracking
robocopy /E timetracking.1.0.1.windows\ C:\Services\TimeTracking

# When the service runs from program folder adjust path to log files (programm folder isn't writeable!)
cd C:\services\TimeTracking
notepad config\FS.TimeTracking.config.nlog

# Install and run as windows service
# The service will be installed as "FS.TimeTracking". You can change the name in the .bat file
cd C:\services\TimeTracking
FS.TimeTracking.WindowsService.Install.bat

# Uninstall service
cd C:\services\TimeTracking
FS.TimeTracking.WindowsService.Uninstall.bat
```

### Install as Linux daemon

```bash
# Copy files
cp -r timetracking.1.0.1.linux/opt/FS.TimeTracking/ /opt/
cp timetracking.1.0.1.linux/etc/systemd/system/FS.TimeTracking.service /etc/systemd/system
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

## Configuration

##### Application / Service / Kestrel configuration

./config/FS.TimeTracking.config.json

```json
"TimeTracking": {
  "Database": {
    // SQL Server
    "Type": "SqlServer",
    "ConnectionString": "Data Source=<server>;Initial Catalog=<database>;User ID=<user>;Password=<password>;Persist Security Info=True"

    // MariaDB / MySql
    "Type": "MySql",
    "ConnectionString": "Server=<server>;Database=<database>;User=<user>;Password=<password>;"

    // Sqlite
    "Type": "Sqlite",
    "ConnectionString": "Data Source=<database file>;"

    // PostgreSQL
    "Type": "PostgreSql",
    "ConnectionString": "Server=<server>;Database=<database>;User Id=<user>;Password=<password>;"
  }
}
```

##### Logging configuration

./config/FS.TimeTracking.config.nlog

See [official documentation](https://github.com/nlog/nlog/wiki/Configuration-file) for details

## Roadmap

### Reports

PDF activity reports for customers are in development and will follow

### User management

Currently no user management / login exists. It's a long time since I've done this things and my knowledge isn't up to date. If you like to help please contact me.

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

See publish script `Build/make_publish.ps1
