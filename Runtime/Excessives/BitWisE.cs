using System;


namespace SOSXR.SeaShark.Excessives
{
    /// <summary>
    /// Collection of lightweight bitwise utilities used by the project.
    /// Includes packing/unpacking booleans, bitstring/string conversions, and simple bit operations.
    /// </summary>
    internal static class BitWisE
    {
        #region Bit Play

        /// <summary>Encodes up to eight booleans into a single byte.</summary>
        /// <param name="boolArray">Boolean array to encode. If exactly eight elements, they map to bits 7 through 0.</param>
        /// <returns>A byte representing the input booleans as bits.</returns>
        public static byte BoolArrayToSingleBinaryByte(bool[] boolArray)
        {
            if (boolArray.Length == 8)
            {
                return (byte) (
                    ((boolArray[0] ? 1 : 0) << 7) |
                    ((boolArray[1] ? 1 : 0) << 6) |
                    ((boolArray[2] ? 1 : 0) << 5) |
                    ((boolArray[3] ? 1 : 0) << 4) |
                    ((boolArray[4] ? 1 : 0) << 3) |
                    ((boolArray[5] ? 1 : 0) << 2) |
                    ((boolArray[6] ? 1 : 0) << 1) |
                    (boolArray[7] ? 1 : 0)
                );
            }

            byte rtnByte = 0;

            for (var i = 0; i < boolArray.Length; i++)
            {
                if (i >= 8)
                {
                    break;
                }

                rtnByte = (byte) (
                    rtnByte | ((boolArray[i] ? 1 : 0) << (7 - i))
                );
            }

            return rtnByte;
        }


        /// <summary>Unpacks a single byte into an array of eight booleans.</summary>
        /// <param name="binary">The byte to unpack.</param>
        /// <returns>An array of eight booleans corresponding to the bits of the input byte, from least-significant to most-significant.</returns>
        public static bool[] SingleBinaryByteToBool(byte binary)
        {
            return new[]
            {
                (binary & 1) > 0,
                (binary & 2) > 0,
                (binary & 4) > 0,
                (binary & 8) > 0,
                (binary & 16) > 0,
                (binary & 32) > 0,
                (binary & 64) > 0,
                (binary & 128) > 0
            };
        }


        /// <summary>Returns a string representation of a single byte's bits.</summary>
        /// <param name="_byte">The byte to convert to a string of bits.</param>
        /// <returns>A string consisting of eight characters '0' or '1' representing the bits from most-significant to least-significant.</returns>
        public static string BinaryToString(byte _byte)
        {
            var byteString = "";

            byteString += (_byte & 128) > 0 ? "1" : "0";
            byteString += (_byte & 64) > 0 ? "1" : "0";
            byteString += (_byte & 32) > 0 ? "1" : "0";
            byteString += (_byte & 16) > 0 ? "1" : "0";
            byteString += (_byte & 8) > 0 ? "1" : "0";
            byteString += (_byte & 4) > 0 ? "1" : "0";
            byteString += (_byte & 2) > 0 ? "1" : "0";
            byteString += (_byte & 1) > 0 ? "1" : "0";


            return byteString;
        }


        /// <summary>Converts an array of bytes to a concatenated bit string.</summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>A string representing all bytes as bits, with the most-significant bit of each byte first.</returns>
        public static string BinaryToString(byte[] bytes)
        {
            var byteString = "";

            //Quite a handy extension method I'd say
            bytes.ForEachBack(n => byteString += BinaryToString(n));

            return byteString;
        }


        /// <summary>Parses an 8-character string of '0' and '1' into a single byte.</summary>
        /// <param name="bitString">A string of length 8 representing bits, where the first character is the most-significant bit.</param>
        /// <returns>The corresponding byte value.</returns>
        public static byte StringToBinary(string bitString)
        {
            byte returnbyte = 0;

            for (byte i = 0; i < 8; i++)
            {
                if (bitString[i] == '1')
                {
                    returnbyte =
                        (byte) (
                            returnbyte |
                            (128 >> i)
                        );
                }
            }

            return returnbyte;
        }


        //Get a bit at any point
        /// <summary>Gets the value of the bit at the specified position within a single byte.</summary>
        /// <param name="bitList">The byte containing the bit field.</param>
        /// <param name="position">Zero-based position of the bit within the byte (0 = least-significant bit).</param>
        /// <returns>True if the bit at the given position is set; otherwise false.</returns>
        public static bool GetBit(byte bitList, int position)
        {
            return (bitList & (1 << position)) > 0;
        }


        //Get a bit at any point
        /// <summary>Gets the value of a bit at a global position across a byte array.</summary>
        /// <param name="bitList">The array of bytes containing the bit field.</param>
        /// <param name="position">Zero-based bit index across the concatenated bytes (LSB first within each byte).</param>
        /// <returns>True if the bit at the given position is set; otherwise false.</returns>
        public static bool GetBit(byte[] bitList, ulong position)
        {
            return
                GetBit(
                    bitList[(int) Math.Floor((decimal) position / 8)],
                    (int) position % 8
                );
        }

        #endregion

        #region Operations

        #region Crossover

        /// <summary>Applies a bitwise crossover between two bytes using a mask.</summary>
        /// <param name="byte1">First source byte.</param>
        /// <param name="byte2">Second source byte.</param>
        /// <param name="mask">Mask indicating which bits to take from the first source.</param>
        /// <returns>The result of combining the two bytes according to the mask.</returns>
        public static byte Crossover(byte byte1, byte byte2, byte mask)
        {
            return (byte) (
                (byte1 & mask)
                |
                (byte2 & ~mask)
            );
        }


        /// <summary>Applies per-byte crossover across two byte arrays using a per-byte mask.</summary>
        /// <param name="byte1">First source array.</param>
        /// <param name="byte2">Second source array.</param>
        /// <param name="mask">Mask array indicating per-byte crossover masks.</param>
        /// <returns>The resulting array after crossover. The input array is modified in place and returned.</returns>
        public static byte[] Crossover
            (byte[] byte1, byte[] byte2, byte[] mask)
        {
            byte1.For((n, i) =>
                byte1[i] = Crossover(byte1[i], byte2[i], mask[i])
            );

            return byte1;
        }

        #endregion


        /// <summary>Adds two byte arrays element-wise.</summary>
        /// <param name="a">First addend array.</param>
        /// <param name="b">Second addend array.</param>
        /// <returns>A new array containing the element-wise sums, or default if lengths do not match.</returns>
        public static byte[] Add(byte[] a, byte[] b)
        {
            if (a.LongLength != b.LongLength)
            {
                return default;
            }

            var added = new byte[a.LongLength];

            return (byte[]) added.For((n, i) => added[i] = (byte) (a[i] + b[i]));
        }

        #endregion

        #region Byte Conversion Extension Methods

        #region ToBytes

        /// <summary>Converts a string to a byte array by encoding each character to two bytes.</summary>
        /// <param name="v">The string to convert.</param>
        /// <returns>Byte array representing the characters of the string.</returns>
        public static byte[] ToBytes(this string v)
        {
            var cArray = v.ToCharArray();

            var data = new byte[cArray.LongLength * 2];

            for (var i = 0; i < cArray.Length; i++)
            {
                Array.Copy(
                    BitConverter.GetBytes(cArray[i]), 0,
                    data, i * 2,
                    2
                );
            }

            //v.For((n, i) => data[i] = BitConverter.GetBytes(n)[0]);


            return data;
        }


        /// <summary>Converts a single byte to a byte array.</summary>
        /// <param name="v">The byte to convert.</param>
        /// <returns>A single-element byte array containing the value.</returns>
        public static byte[] ToBytes(this byte v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts a signed byte to a byte array.</summary>
        /// <param name="v">The sbyte to convert.</param>
        /// <returns>A two's-complement representation as a byte array.</returns>
        public static byte[] ToBytes(this sbyte v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts a character to a two-byte array.</summary>
        /// <param name="v">The character to convert.</param>
        /// <returns>Byte array representation of the character.</returns>
        public static byte[] ToBytes(this char v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts an unsigned short to a byte array.</summary>
        /// <param name="v">The ushort to convert.</param>
        /// <returns>Byte array representation of the value.</returns>
        public static byte[] ToBytes(this ushort v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts a short to a byte array.</summary>
        /// <param name="v">The short value to convert.</param>
        /// <returns>Byte array representation of the value.</returns>
        public static byte[] ToBytes(this short v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts an unsigned int to a byte array.</summary>
        /// <param name="v">The uint value to convert.</param>
        /// <returns>Byte array representation of the value.</returns>
        public static byte[] ToBytes(this uint v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts a signed int to a byte array.</summary>
        /// <param name="v">The int value to convert.</param>
        /// <returns>Byte array representation of the value.</returns>
        public static byte[] ToBytes(this int v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts an unsigned long to a byte array.</summary>
        /// <param name="v">The ulong value to convert.</param>
        /// <returns>Byte array representation of the value.</returns>
        public static byte[] ToBytes(this ulong v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts a long to a byte array.</summary>
        /// <param name="v">The long value to convert.</param>
        /// <returns>Byte array representation of the value.</returns>
        public static byte[] ToBytes(this long v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts a float to a byte array.</summary>
        /// <param name="v">The float value to convert.</param>
        /// <returns>Byte array representation of the value.</returns>
        public static byte[] ToBytes(this float v)
        {
            return BitConverter.GetBytes(v);
        }


        /// <summary>Converts a double to a byte array.</summary>
        /// <param name="v">The double value to convert.</param>
        /// <returns>Byte array representation of the value.</returns>
        public static byte[] ToBytes(this double v)
        {
            return BitConverter.GetBytes(v);
        }

        #endregion

        #region FromBytes

        public static string DecStr(this byte[] v)
        {
            var str = "";

            for (var i = 0; i < v.Length; i += 2)
            {
                str += BitConverter.ToChar(v, i);
            }

            return str;
        }


        public static string DecHex(this byte[] v)
        {
            return BitConverter.ToString(v);
        }


        public static char DecChar(this byte[] v)
        {
            return BitConverter.ToChar(v, 0);
        }


        public static ushort DecUShort(this byte[] v)
        {
            return BitConverter.ToUInt16(v, 0);
        }


        public static short DecShort(this byte[] v)
        {
            return BitConverter.ToInt16(v, 0);
        }


        public static uint DecUInt(this byte[] v)
        {
            return BitConverter.ToUInt32(v, 0);
        }


        public static int DecInt(this byte[] v)
        {
            return BitConverter.ToInt32(v, 0);
        }


        public static ulong DecULong(this byte[] v)
        {
            return BitConverter.ToUInt64(v, 0);
        }


        public static long DecLong(this byte[] v)
        {
            return BitConverter.ToInt64(v, 0);
        }


        public static float DecSingle(this byte[] v)
        {
            return BitConverter.ToSingle(v, 0);
        }


        public static double DecDouble(this byte[] v)
        {
            return BitConverter.ToDouble(v, 0);
        }

        #endregion

        #endregion
    }
}
