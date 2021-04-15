using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System;

namespace DevOpsToolkit.RedisCli.Helpers
{
    /// <summary>
    /// Decompress functionality to decompress the zipped Redis value
    /// </summary>
    public static class Decompressor
    {
        /// <summary>
        /// Decompress byte array
        /// </summary>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static async Task<byte[]> Decompress(byte[] gzip)
        {
            if (gzip == null)
            {
                return null;
            }

            await using (var stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int Size = 4096;
                var buffer = new byte[Size];
                await using (var memory = new MemoryStream())
                {
                    var count = 0;
                    do
                    {
                        count = await stream.ReadAsync(buffer.AsMemory(0, Size));
                        if (count > 0)
                        {
                            await memory.WriteAsync(buffer.AsMemory(0, count));
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }
}