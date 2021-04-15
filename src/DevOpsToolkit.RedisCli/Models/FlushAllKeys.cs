using CommandLine;

namespace DevOpsToolkit.RedisCli.Models
{
    [Verb("flushall", HelpText = "Flush all Redis keys from the server.")]
    public class FlushAllKeys
    {
    }
}