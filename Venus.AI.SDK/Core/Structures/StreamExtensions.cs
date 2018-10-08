using System.IO;

namespace Venus.AI.SDK.Core.Structures
{
    /// <summary>
    /// Extensions for Stream class
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Converts stream to byew array
        /// </summary>
        /// <param name="stream">input stream</param>
        /// <returns>byte array</returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
