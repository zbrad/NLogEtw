using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBrad.NLogEtw
{
    /// <summary>
    /// Implements the SHA1 hashing algorithm. Note that this
    /// implementation is for hashing public information. Do not
    /// use this code to hash private data, as this implementation does
    /// not take any steps to avoid information disclosure.
    /// 
    /// Source: http://referencesource.microsoft.com/#mscorlib/system/diagnostics/eventing/eventsource.cs,1545
    /// </summary>
    internal struct Sha1ForNonSecretPurposes
    {
        private long length; // Total message length in bits
        private uint[] w; // Workspace
        private int pos; // Length of current chunk in bytes

        /// <summary>
        /// Call Start() to initialize the hash object.
        /// </summary>
        public void Start()
        {
            if (this.w == null)
            {
                this.w = new uint[85];
            }

            this.length = 0;
            this.pos = 0;
            this.w[80] = 0x67452301;
            this.w[81] = 0xEFCDAB89;
            this.w[82] = 0x98BADCFE;
            this.w[83] = 0x10325476;
            this.w[84] = 0xC3D2E1F0;
        }

        /// <summary>
        /// Adds an input byte to the hash.
        /// </summary>
        /// <param name="input">Data to include in the hash.</param>
        public void Append(byte input)
        {
            this.w[this.pos / 4] = (this.w[this.pos / 4] << 8) | input;
            if (64 == ++this.pos)
            {
                this.Drain();
            }
        }

        /// <summary>
        /// Adds input bytes to the hash.
        /// </summary>
        /// <param name="input">
        /// Data to include in the hash. Must not be null.
        /// </param>
        public void Append(byte[] input)
        {
            foreach (var b in input)
            {
                this.Append(b);
            }
        }

        /// <summary>
        /// Retrieves the hash value.
        /// Note that after calling this function, the hash object should
        /// be considered uninitialized. Subsequent calls to Append or
        /// Finish will produce useless results. Call Start() to
        /// reinitialize.
        /// </summary>
        /// <param name="output">
        /// Buffer to receive the hash value. Must not be null.
        /// Up to 20 bytes of hash will be written to the output buffer.
        /// If the buffer is smaller than 20 bytes, the remaining hash
        /// bytes will be lost. If the buffer is larger than 20 bytes, the
        /// rest of the buffer is left unmodified.
        /// </param>
        public void Finish(byte[] output)
        {
            long l = this.length + 8 * this.pos;
            this.Append(0x80);
            while (this.pos != 56)
            {
                this.Append(0x00);
            }

            unchecked
            {
                this.Append((byte)(l >> 56));
                this.Append((byte)(l >> 48));
                this.Append((byte)(l >> 40));
                this.Append((byte)(l >> 32));
                this.Append((byte)(l >> 24));
                this.Append((byte)(l >> 16));
                this.Append((byte)(l >> 8));
                this.Append((byte)l);

                int end = output.Length < 20 ? output.Length : 20;
                for (int i = 0; i != end; i++)
                {
                    uint temp = this.w[80 + i / 4];
                    output[i] = (byte)(temp >> 24);
                    this.w[80 + i / 4] = temp << 8;
                }
            }
        }

        /// <summary>
        /// Called when this.pos reaches 64.
        /// </summary>
        private void Drain()
        {
            for (int i = 16; i != 80; i++)
            {
                this.w[i] = Rol1((this.w[i - 3] ^ this.w[i - 8] ^ this.w[i - 14] ^ this.w[i - 16]));
            }

            unchecked
            {
                uint a = this.w[80];
                uint b = this.w[81];
                uint c = this.w[82];
                uint d = this.w[83];
                uint e = this.w[84];

                for (int i = 0; i != 20; i++)
                {
                    const uint k = 0x5A827999;
                    uint f = (b & c) | ((~b) & d);
                    uint temp = Rol5(a) + f + e + k + this.w[i]; e = d; d = c; c = Rol30(b); b = a; a = temp;
                }

                for (int i = 20; i != 40; i++)
                {
                    uint f = b ^ c ^ d;
                    const uint k = 0x6ED9EBA1;
                    uint temp = Rol5(a) + f + e + k + this.w[i]; e = d; d = c; c = Rol30(b); b = a; a = temp;
                }

                for (int i = 40; i != 60; i++)
                {
                    uint f = (b & c) | (b & d) | (c & d);
                    const uint k = 0x8F1BBCDC;
                    uint temp = Rol5(a) + f + e + k + this.w[i]; e = d; d = c; c = Rol30(b); b = a; a = temp;
                }

                for (int i = 60; i != 80; i++)
                {
                    uint f = b ^ c ^ d;
                    const uint k = 0xCA62C1D6;
                    uint temp = Rol5(a) + f + e + k + this.w[i]; e = d; d = c; c = Rol30(b); b = a; a = temp;
                }

                this.w[80] += a;
                this.w[81] += b;
                this.w[82] += c;
                this.w[83] += d;
                this.w[84] += e;
            }

            this.length += 512; // 64 bytes == 512 bits
            this.pos = 0;
        }

        private static uint Rol1(uint input)
        {
            return (input << 1) | (input >> 31);
        }

        private static uint Rol5(uint input)
        {
            return (input << 5) | (input >> 27);
        }

        private static uint Rol30(uint input)
        {
            return (input << 30) | (input >> 2);
        }
    }
}
