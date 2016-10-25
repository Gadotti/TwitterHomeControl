using System;
using Microsoft.SPOT;


    public static class HexDisplay
    {
        static readonly char[] hchars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };


        public static string byteToHex(byte input)
        {
            return new string(new char[] { hchars[input >> 4], hchars[input & 0x0F] });
        }

        public static string bytesToHex(byte[] input)
        {
            string r = "";
            foreach (byte b in input)
            {
                r += byteToHex(b);
            }
            return r;
        }

    }
