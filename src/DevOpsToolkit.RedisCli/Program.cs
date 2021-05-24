using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using DevOpsToolkit.RedisCli.Helpers;
using DevOpsToolkit.RedisCli.Models;
using StackExchange.Redis;

namespace DevOpsToolkit.RedisCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"");
            Console.WriteLine(@"                   .___.__              .__  .__ ");
            Console.WriteLine(@"_______   ____   __| _/|__| ______ ____ |  | |__|");
            Console.WriteLine(@"\_  __ \_/ __ \ / __ | |  |/  ___// ___\|  | |  |");
            Console.WriteLine(@" |  | \/\  ___// /_/ | |  |\___ \\  \___|  |_|  |");
            Console.WriteLine(@" |__|    \___  >____ | |__/____  >\___  >____/__|");
            Console.WriteLine(@"             \/     \/         \/     \/         ");
            Console.WriteLine(@"");

            Parser.Default
                .ParseArguments<ListKeys, ShowKey, FlushAllKeys, DeleteKeys, CheckConnectionKeys, ClientKeys>(args)
                .WithParsed<ListKeys>(RunListKeys)
                .WithParsed<ShowKey>(async options => await RunShowKey(options))
                .WithParsed<FlushAllKeys>(async options => await RunFlushAllKeys(options))
                .WithParsed<DeleteKeys>(async options => await RunDeleteKeys(options))
                .WithParsed<CheckConnectionKeys>(async options => await CheckConnectionKeys(options))
                .WithParsed<ClientKeys>(async options => await ClientKeys(options))
                .WithNotParsed(Errors);
        }

        private static void Errors(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine($"Error: {error}");
            }
        }

        private static void RunListKeys(ListKeys options)
        {
            Console.WriteLine($"List Redis Keys...");
            Console.WriteLine(@"");

            var configuration = Configuration.GetConfiguration();

            var redis = ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]);
            var server = redis.GetServer(configuration["RedisInstance"]);
            var database = redis.GetDatabase();

            var pattern = string.IsNullOrEmpty(options.Pattern) ? "*" : options.Pattern;

            var keys = server.Keys(database: database.Database, pattern: pattern);

            if (keys == null)
            {
                Console.WriteLine("No keys found.");
            }
            else
            {
                foreach (var key in keys)
                {
                    Console.WriteLine($"{key}");
                }
            }
        }

        private static async Task RunShowKey(ShowKey options)
        {
            Console.WriteLine($"Showing value of the key...");
            Console.WriteLine(@"");

            var configuration = Configuration.GetConfiguration();

            var redis = ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]);
            var server = redis.GetServer(configuration["RedisInstance"]);
            var database = redis.GetDatabase();

            var hash = database.HashGetAll(options.Key);
            var value = hash.FirstOrDefault(h => h.Name == "data");
            var absexp = hash.FirstOrDefault(h => h.Name == "absexp");
            byte[] data = value.Value;

            if (hash.Length == 0)
            {
                Console.WriteLine($"Key {options.Key} does not exist.");
                return;
            }

            Console.WriteLine($"Key: {options.Key}");

            Console.WriteLine(
                options.Decompress
                    ? $"Value: {Encoding.UTF8.GetString(await Decompressor.Decompress(data))}"
                    : $"Value: {Encoding.UTF8.GetString(data)}");

            Console.WriteLine($"Expiry: {new DateTimeOffset((long) absexp.Value, TimeSpan.Zero) - DateTimeOffset.Now}");
        }

        private static async Task RunFlushAllKeys(FlushAllKeys options)
        {
            Console.WriteLine("Type 'yes' to flush the database.");
            if (Console.ReadLine() != "yes")
            {
                Console.WriteLine("");
                Console.WriteLine("Press key to exit.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Flushing Redis database...");
            Console.WriteLine(@"");

            var configuration = Configuration.GetConfiguration();

            var redis = ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]);
            var server = redis.GetServer(configuration["RedisInstance"]);
            var database = redis.GetDatabase();

            await server.FlushDatabaseAsync(database.Database);
        }

        private static async Task RunDeleteKeys(DeleteKeys options)
        {
            Console.WriteLine($"Type 'yes' to delete the key {options.Pattern}.");
            if (Console.ReadLine() != "yes")
            {
                Console.WriteLine("");
                Console.WriteLine("Press key to exit.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(@"");

            var configuration = Configuration.GetConfiguration();

            var redis = ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]);
            var server = redis.GetServer(configuration["RedisInstance"]);
            var database = redis.GetDatabase();

            foreach (var key in server.Keys(database: database.Database, pattern: options.Pattern))
            {
                Console.WriteLine($"Delete Redis Key: {key}");
                await database.KeyDeleteAsync(key);
            }
        }

        private static async Task CheckConnectionKeys(CheckConnectionKeys options)
        {
            var configuration = Configuration.GetConfiguration();

            try
            {
                var redis = ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]);
                Console.WriteLine(redis.IsConnected ? "Redis connected OK." : "Redis not connected.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Redis not connected {e.Message}.");
            }
        }

        private static async Task ClientKeys(ClientKeys options)
        {
            var configuration = Configuration.GetConfiguration();

            var redis = ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]);
            var server = redis.GetServer(configuration["RedisInstance"]);
            var clients = server.ClientList();

            Console.WriteLine("Redis Connections");
            Console.WriteLine("");
            Console.WriteLine("{0,-50}{1,-20}", "Client Name", "Connection count");
            Console.WriteLine("{0,-50}{1,-20}", "-------------------------------------", "----------------------");
            foreach (var client in clients
                .GroupBy(g => g.Name, (key, g) => new {Name = key, Count = g.Count()})
                .OrderByDescending(o => o.Count))
            {
                Console.WriteLine("{0,-50}{1,-20}", client.Name, client.Count);
            }
        }
    }
}