using System;


namespace SOSXR.SeaShark.Excessives
{
    /// <summary>
    /// Lightweight bit-level accessor for a binary stream.
    /// Provides indexed access to individual bits within the underlying byte array.
    /// </summary>
    internal struct BitString
    {
        public byte[] stream;

        /// <summary>
        /// Gets the total number of bits represented by the underlying stream.
        /// </summary>
        public long Length => stream.LongLength * 8;

        /// <summary>
        /// Initializes a new BitString with the specified byte stream.
        /// </summary>
        /// <param name="stream">The byte array representing the bit stream.</param>
        public BitString(byte[] stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Gets or sets the bit at the specified index within the stream.
        /// Bit 0 corresponds to the least-significant bit of the first byte.
        /// </summary>
        /// <param name="index">The zero-based bit index within the stream.</param>
        /// <returns>True if the bit is set; otherwise false.</returns>
        public bool this[ulong index]
        {
            get => //This - 'appears to work'...
                (stream[(ulong) Math.Floor(index / 8.0)]
                 & (1 << (int) (index % 8))
                ) > 0;
            //...and so does this
            set
            {
                var byteIndex = (ulong) Math.Floor(index / 8.0);
                var bitIndex = (int) (index % 8);
                
                if (value)
                {
                    // Turn the bit on
                    stream[byteIndex] |= (byte) (1 << bitIndex);
                }
                else
                {
                    // Turn the bit off
                    stream[byteIndex] &= (byte) ~(1 << bitIndex);
                }
            }
        }
    }
}
