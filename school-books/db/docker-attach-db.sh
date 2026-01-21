sleep 15s

tail -f /var/opt/mssql/log/errorlog | grep -q "Service Broker manager has started" && \
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -No -i /docker-attach-db.sql
