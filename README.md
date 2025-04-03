![Static Badge](https://img.shields.io/badge/Umbraco-214BAF?logo=Umbraco&labelColor=214BAF&color=000000)
![Static Badge](https://img.shields.io/badge/.Net-512BD4?logo=dotnet&labelColor=512BD4&color=000000)
![Static Badge](https://img.shields.io/badge/Docker-512BD4?logo=docker&labelColor=FFFFFF&color=000000)
# Umbraco-Remote-Storage
Remote Storage example for Umbraco

For the example to work please setup a local FTP with the following Docker Compose file:
```yaml
services:
  ftp:
    image: bogem/ftp
    container_name: ftp
    ports:
    - 21:21
    - 47400-47470:47400-47470
    environment:
    - FTP_USER=umbraco 
    - FTP_PASS=Abc123!@
    - PASV_ADDRESS=127.0.0.1
    volumes:
    - ftpdata:/home/vsftpd 

volumes:
  ftpdata:
```
