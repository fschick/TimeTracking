# FS.TimeTracking

### Configuration files

##### Application / Service / Kestrel configuration

./FS.TimeTracking.config.json

##### Logging configuration

./FS.TimeTracking.config.nlog

##### OpenAPI specification

./FS.TimeTracking.openapi.json



### Run from command line

```shell
# Windows
.\FS.TimeTracking.exe

# Linux
chmod +x ./FS.TimeTracking
./FS.TimeTracking
```



### Install as service on Windows

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



### Install as service on Linux

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

