#!/bin/bash
/opt/mssql/bin/sqlservr &

until /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P EventLogger123! -Q "SELECT 1" > /dev/null 2>&1; do
  echo "Waiting for SQL Server to be ready..."
  sleep 2
done

echo "SQL Server is up - running init script"
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P EventLogger123! -i /init.sql

wait