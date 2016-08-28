using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuzipCompress
{
    class RunLength
    {
        public static byte[] Module0(byte[] _bytes)
        {
            byte[] ret = null;
            int i = 0, c = _bytes.Length;
            byte b = _bytes[0];
            while (i < c)
            {
                if (_bytes[i] != b) break;
                i++;
            }
            if (i != c) return null;
            byte[] bLen = BitConverter.GetBytes(c);
            List<byte> byteArray = new List<byte>();
            byteArray.Add(bLen[0]);
            byteArray.Add(bLen[1]);
            byteArray.Add(bLen[2]);
            byteArray.Add(bLen[3]);
            byteArray.Add(b);
            ret = new byte[byteArray.Count]; i = 0;
            while (i < ret.Length) { ret[i] = byteArray[i]; i++; }
            return ret;
        }

        public static byte[] UnModule0(byte[] _bytes)
        {
            byte[] ret = null;
            int i = 0, c = BitConverter.ToInt32(_bytes, 0);
            byte b = _bytes[4];
            ret = new byte[c];
            while (i < c) { ret[i] = b; i++; }
            return ret;
        }

        private static byte[] ModuleRunLength(byte[] _bytes, byte _key)
        {
            byte[] ret = null; List<byte> byteArray = new List<byte>();
            int i = 1, c = _bytes.Length, again = 0; byte b = _bytes[0], key = _key;
            while (i < c)
            {
                if (b == _bytes[i]) again++;
                else
                {
                    if (again == 0)
                    {
                        if (b == key) { byteArray.Add(key); byteArray.Add((byte)again); byteArray.Add(b); }
                        else byteArray.Add(b);
                    }
                    else
                    {
                        byteArray.Add(key);
                        while (again >= 255) { byteArray.Add(255); again -= 255; }
                        byteArray.Add((byte)again); byteArray.Add(b); again = 0;
                    }
                }
                b = _bytes[i];
                i++;
            }
            if (again == 0)
            {
                if (b == key) { byteArray.Add(key); byteArray.Add((byte)again); byteArray.Add(b); }
                else byteArray.Add(b);
            }
            else
            {
                byteArray.Add(key);
                while (again >= 255) { byteArray.Add(255); again -= 255; }
                byteArray.Add((byte)again); byteArray.Add(b); again = 0;
            }
            i = 0; c = byteArray.Count;
            ret = new byte[c];
            while (i < c) { ret[i] = byteArray[i]; i++; }
            return ret;
        }

        public static byte[] ModuleRunLength0(byte[] _bytes)
        {
            byte[] ret = ModuleRunLength(_bytes, 0xFF); return ret;
        }

        public static byte[] ModuleRunLength1(byte[] _bytes)
        {
            byte[] ret = ModuleRunLength(_bytes, 0); return ret;
        }

        public static byte[] ModuleRunLength2(byte[] _bytes)
        {
            byte[] ret = ModuleRunLength(_bytes, 128); return ret;
        }

        private static byte[] UnModuleRunLength(byte[] _bytes, byte _key)
        {
            byte[] ret = null; List<byte> byteArray = new List<byte>();
            int i = 0, c = _bytes.Length, again = 0; byte b, key = _key;
            while (i < c)
            {
                b = _bytes[i];
                if (b == key)
                {
                    byte ag = 255; while (ag == 255) { i++; ag = _bytes[i]; again += ag; }
                    i++; b = _bytes[i]; byteArray.Add(b);
                    int j = 0; while (j < again) { byteArray.Add(b); j++; } again = 0;
                }
                else byteArray.Add(b);
                i++;
            }
            i = 0; c = byteArray.Count;
            ret = new byte[c];
            while (i < c) { ret[i] = byteArray[i]; i++; }
            return ret;
        }

        public static byte[] UnModuleRunLength0(byte[] _bytes)
        {
            byte[] ret = UnModuleRunLength(_bytes, 0xFF); return ret;
        }

        public static byte[] UnModuleRunLength1(byte[] _bytes)
        {
            byte[] ret = UnModuleRunLength(_bytes, 0); return ret;
        }

        public static byte[] UnModuleRunLength2(byte[] _bytes)
        {
            byte[] ret = UnModuleRunLength(_bytes, 128); return ret;
        }
    }
}
