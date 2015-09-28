using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.NLogEtw
{
    internal static class SourceName
    {

        // used for generating GUID from eventsource name
        private static readonly byte[] namespaceBytes = new byte[] {
            0x48, 0x2C, 0x2D, 0xB2, 0xC3, 0x90, 0x47, 0xC8,
            0x87, 0xF8, 0x1A, 0x15, 0xBF, 0xC1, 0x30, 0xFB,
        };

        internal static Guid GenerateGuidFromName(string name)
        {
            byte[] bytes = Encoding.BigEndianUnicode.GetBytes(name);
            var hash = new Sha1ForNonSecretPurposes();
            hash.Start();
            hash.Append(namespaceBytes);
            hash.Append(bytes);
            Array.Resize(ref bytes, 16);
            hash.Finish(bytes);

            bytes[7] = unchecked((byte)((bytes[7] & 0x0F) | 0x50));    // Set high 4 bits of octet 7 to 5, as per RFC 4122
            return new Guid(bytes);
        }

    }
}
