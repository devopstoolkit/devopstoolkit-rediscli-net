using CommandLine;

namespace DevOpsToolkit.RedisCli.Models
{
    [Verb("list", HelpText = "List and filter Redis keys.")]
    public class ListKeys
    {
        [Option('p', "pattern", Required = false, HelpText = "Find keys by name or wildcard.")]
        public string Pattern { get; set; }
    }
}