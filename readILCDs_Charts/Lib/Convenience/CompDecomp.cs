using System.IO;
using System.IO.Compression;

namespace Greet.ConvenienceLib
{
    /// <summary>
    /// Static class that contains methods to compress and decompress a stream using the deflate algorithm
    /// </summary>
    public static class CompDecomp
    {
        /// <summary>
        /// Decompresses a stream using the deflate algorithm
        /// </summary>
        /// <param name="inp">Compressed stream that we desire to decompress</param>
        /// <param name="outp">Decompressed output</param>
        /// <returns>Lengtyh of the decompressed stream in bytes</returns>
        public static long Decompress(Stream inp, Stream outp)
        {
            byte[] buf = new byte[1000];
            long nBytes = 0;
            inp.Position = 0;

            // Decompress the contents of the input file
            using (inp = new DeflateStream(inp, CompressionMode.Decompress))
            {
                int len;
                while ((len = inp.Read(buf, 0, buf.Length)) > 0)
                {
                    // Write the data block to the decompressed output stream
                    outp.Write(buf, 0, len);
                    nBytes += len;
                }
            }
            // Done
            return nBytes;
        }

        /// <summary>
        /// Compresses a stream using the deflate algorithm
        /// </summary>
        /// <param name="inp">Stream that we desire to compress</param>
        /// <param name="outp">Compressed stream</param>
        /// <returns>Length of the compressed stream in bytes</returns>
        public static long Compress(Stream inp, Stream outp)
        {
            byte[] buf = new byte[1000];
            long nBytes = 0;
            inp.Position = 0;

            // Compress the contents of the input file
            using (outp = new DeflateStream(outp, CompressionMode.Compress, true))
            {
                int len;
                while ((len = inp.Read(buf, 0, buf.Length)) > 0)
                {
                    // Write the data block to the compressed output stream
                    outp.Write(buf, 0, len);
                    nBytes += len;
                }
            }
            // Done
            return nBytes;
        }
    }
}
