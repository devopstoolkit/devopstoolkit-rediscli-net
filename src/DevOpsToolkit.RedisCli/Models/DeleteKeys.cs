using CommandLine;

namespace DevOpsToolkit.RedisCli.Models
{
    [Verb("delete", HelpText = "Delete Redis keys from the server.")]
    public class DeleteKeys
    {
        [Option('p', "pattern", Required = true, HelpText = "Redis key you want to delete.")]
        public string Pattern { get; set; }
    }
}