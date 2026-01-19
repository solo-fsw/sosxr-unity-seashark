using System;


namespace SOSXR.SeaShark.Excessives
{
    internal struct BitString
    {
        public byte[] stream;

        public long Length => stream.LongLength * 8;

        public BitString(byte[] stream)
        {
            this.stream = stream;
        }

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