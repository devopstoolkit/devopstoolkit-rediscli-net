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
            Console.WriteLine(@"                   .___.__               __  .__   ");
            Console.WriteLine(@"_______   ____   __| _/|__| ______ _____/  |_|  |  ");
            Console.WriteLine(@"\_  __ \_/ __ \ / __ | |  |/  ___// ___\   __\  |  ");
            Console.WriteLine(@" |  | \/\  ___// /_/ | |  |\___ \\  \___|  | |  |__");
            Console.WriteLine(@" |__|    \___  >____ | |__/____  >\___  >__| |____/");
            Console.WriteLine(@"             \/     \/         \/     \/           ");
            Console.WriteLine(@"");

            Parser.Default.ParseArguments<ListKeys, ShowKey, FlushAllKeys, DeleteKeys>(args)
                .WithParsed<ListKeys>(options => RunListKeys(options))
                .WithParsed<ShowKey>(async options => await RunShowKey(options))
                .WithParsed<FlushAllKeys>(async options => await RunFlushAllKeys(options))
                .WithParsed<DeleteKeys>(async options => await RunDeleteKeys(options))
                .WithNotParsed(Errors);
        }

        private static void Errors(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine($"Error: {error.ToString()}");
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

            Console.WriteLine("");
            Console.WriteLine("Press key to exit.");
            Console.ReadLine();
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

            Console.WriteLine("");
            Console.WriteLine("Press key to exit.");
            Console.ReadLine();
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

            Console.WriteLine("");
            Console.WriteLine("Press key to exit.");
            Console.ReadLine();
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

            Console.WriteLine("");
            Console.WriteLine("Press key to exit.");
            Console.ReadLine();
        }
    }
}