## Server cluster management tool

Without some customisation, this is **not** production-ready.

#### Getting started
```
docker build -t webnetworkmanager .
docker run -d \ 
    -p 8080:8080 \
    -e "ConnectionStrings__Postgres=Host=postgres.local;Database=postgres;Username=postgres;Password=hunter2" \
    webnetworkmanager
``` 

Default credentials are `Administrator` / `Adm1nistrator!` 