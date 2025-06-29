@echo off
REM Check if network exists
docker network inspect eventlogger-network >nul 2>&1
IF %ERRORLEVEL% NEQ 0 (
    echo Creating Docker network 'eventlogger-network'...
    docker network create eventlogger-network
) ELSE (
    echo Docker network 'eventlogger-network' already exists.
)

REM Start Docker Compose
docker-compose up -d

pause