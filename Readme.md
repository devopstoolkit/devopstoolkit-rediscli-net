# DevOpsToolkit.RedisCli

Access to Redis server and apply some of the functions:

```powershell
# List all keys / List keys with pattern
rscli list
rscli list -p <partialname>*
# Show key value (with decompression if used)
rscli show -k <keyname>
rscli show -k <keyname> -d
# Delete a key / Delete keys with pattern
rscli delete -p <keyname>
rscli delete -p <partialname>*
# Flush all keys (removes all keys from Redis)
rscli flushall
# Check connection key (check if connection to Redis is OK)
rscli check
# Get number of client connections to the Redis server
rscli clients
```


## Configuration

Change `config.json` file to you needs before you run the tool:

```json
{
    "RedisInstance": "localhost:6380",
    "RedisConnectionString": "localhost:6380,password=pass,ssl=True,abortConnect=False,connectTimeout=60000,asyncTimeout=60000,syncTimeout=60000"
}
```

## Build it yourself

```powershell
dotnet.exe publish --configuration Release --framework net5.0 --output Publish --self-contained True --runtime win-x64 --verbosity Normal /property:PublishTrimmed=True /property:PublishSingleFile=True /property:IncludeNativeLibrariesForSelfExtract=True /property:DebugType=None /property:DebugSymbols=False
```