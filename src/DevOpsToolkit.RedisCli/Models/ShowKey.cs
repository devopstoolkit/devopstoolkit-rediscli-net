using CommandLine;

namespace DevOpsToolkit.RedisCli.Models
{
        [Verb("show", HelpText = "Show value of a Redis key.")]
        public class ShowKey
        {
            [Option('k', "key", Required = true, HelpText = "Redis key you want to show.")]
            public string Key { get; set; }
            
            [Option('d', "decompress", Required = false, Default = false, HelpText = "Unzip (decompress) Redis key value.")]
            public bool Decompress { get; set; }
        }
}