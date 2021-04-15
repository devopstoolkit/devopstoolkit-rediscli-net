# DevOpsToolkit.RedisCli

Access to Redis server and apply some of the functions:

- List all keys / List keys with pattern
- Show key value (with decompression if used)
- Delete a key / Delete keys with pattern
- Flush all keys (removes all keys from Redis)

## Configuration

Change `config.json` file to you needs before you run the tool:

```json
{
    "RedisInstance": "localhost:6380",
    "RedisConnectionString": "localhost:6380,password=pass,ssl=True,abortConnect=False,connectTimeout=60000,asyncTimeout=60000,syncTimeout=60000"
}
```