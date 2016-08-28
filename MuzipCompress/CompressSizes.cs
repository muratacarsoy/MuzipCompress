using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuzipCompress
{
    class CompressSizes
    {
        public static byte[] CompressMethods()
        {
            return new byte[12] { 255, 0, 1, 2, 3, 4, 5, 20, 30, 31, 32, 40 };
        }

        public static int CompressSizeOfData(byte[] _bytes, byte Module, int compressedOthers)
        {
            int ret = -1;
            switch (Module)
            {
                case 0: ret = CSModule0(_bytes); break;
                case 1: ret = CSModule1(_bytes); break;
                case 2: ret = CSModule2(_bytes); break;
                case 3: ret = CSModule3(_bytes); break;
                case 4: ret = CSModule4(_bytes); break;
                case 5: ret = CSModule5(_bytes); break;
                case 20: ret = CSHuffman(_bytes); break;
                case 21: ret = CSHuffman2(_bytes); break;
                case 22: ret = CSHuffman3(_bytes); break;
                case 30: ret = CSRunLength0(_bytes); break;
                case 31: ret = CSRunLength1(_bytes); break;
                case 32: ret = CSRunLength2(_bytes); break;
                case 40: ret = CSLZ78(_bytes, compressedOthers); break;
                case 41: ret = CSLZ78_2(_bytes, compressedOthers); break;
                case 42: ret = CSLZ78_3(_bytes, compressedOthers); break;
                case 255: ret = CSModuleNon(_bytes); break;
            }
            return ret;
        }

        private static int CSModuleNon(byte[] _bytes)
        {
            return _bytes.Length;
        }

        private static int CSModule0(byte[] _bytes)
        {
            int i = 0, c = _bytes.Length;
            byte b = _bytes[0];
            while (i < c)
            {
                if (_bytes[i] != b) break;
                i++;
            }
            if (i != c) return -1;
            return 5;
        }

        private static int CSModule1(byte[] _bytes)
        {
            int i = 0, c = _bytes.Length;
            byte min = 255, max = 0, delta;
            while (i < c) { if (min > _bytes[i]) min = _bytes[i]; if (max < _bytes[i]) max = _bytes[i]; i++; }
            delta = (byte)(max - min);
            int cap = bitCapacity(delta);
            if ((cap * c) % 8 != 0 || cap == 8) return -1;
            return cap * c / 8 + 2;
        }

        private static int CSModule2(byte[] _bytes)
        {
            int i = 0, c = _bytes.Length;
            if (c % 2 != 0) return -1;
            ushort min = 65535, max = 0, delta;
            ushort[] shorts = new ushort[c / 2];
            c = shorts.Length;
            while (i < c) { shorts[i] = BitConverter.ToUInt16(_bytes, 2 * i); i++; }
            i = 0;
            while (i < c) { if (min > shorts[i]) min = shorts[i]; if (max < shorts[i]) max = shorts[i]; i++; }
            delta = (ushort)(max - min);
            int cap = bitCapacity(delta);
            if ((cap * c) % 16 != 0 || cap == 16) return -1;
            return cap * c / 8 + 4;
        }

        private static int CSModule3(byte[] _bytes)
        {
            int i = 0, c = _bytes.Length;
            if (c % 4 != 0) return -1;
            uint min = 0xFFFFFFFF, max = 0, delta;
            uint[] ints = new uint[c / 4];
            c = ints.Length;
            while (i < c) { ints[i] = BitConverter.ToUInt32(_bytes, 4 * i); i++; }
            i = 0;
            while (i < c) { if (min > ints[i]) min = ints[i]; if (max < ints[i]) max = ints[i]; i++; }
            delta = (uint)(max - min);
            int cap = bitCapacity(delta);
            if ((cap * c) % 32 != 0 || cap == 32) return -1;
            return (cap * c) / 8 + 8;
        }

        private static int CSModule4(byte[] _bytes)
        {
            int i = 0, c = _bytes.Length;
            if (c % 8 != 0) return -1;
            ulong min = 0xFFFFFFFFFFFFFFFF, max = 0, delta;
            ulong[] longs = new ulong[c / 8];
            c = longs.Length;
            while (i < c) { longs[i] = BitConverter.ToUInt64(_bytes, 8 * i); i++; }
            i = 0;
            while (i < c) { if (min > longs[i]) min = longs[i]; if (max < longs[i]) max = longs[i]; i++; }
            delta = (ulong)(max - min);
            int cap = bitCapacity(delta);
            if ((cap * c) % 64 != 0 || cap == 64) return -1;
            return (cap * c) / 8 + 16;
        }

        private static int CSModule5(byte[] _bytes)
        {
            byte[] bytes = (byte[])_bytes.Clone();
            int i = 0, c = bytes.Length; byte maxA = 0, minB = 255, b = 0;
            while (i < c)
            {
                b = bytes[i];
                if (b < 128) { if (maxA < b) maxA = b; }
                else { if (minB > b) minB = b; }
                i++;
            }
            i = 0; maxA = (byte)(127 - maxA); minB = (byte)(minB - 128);
            while (i < c)
            {
                b = bytes[i];
                if (b < 128) { bytes[i] += maxA; } else { bytes[i] -= minB; }
                i++;
            }
            int md1 = CSModule1(bytes);
            if (md1 == -1) return -1;
            return md1 + 2;
        }

        private static int CSHuffman(byte[] _bytes)
        {
            return UnSafeHuffman.MuzipSize(_bytes);
        }

        private static int CSHuffman2(byte[] _bytes)
        {
            byte[] bytes = (byte[])_bytes.Clone();
            int i = 0, c = bytes.Length;
            while (i < c) { if (i % 2 == 0) bytes[i] = (byte)~bytes[i]; i++; }
            return UnSafeHuffman.MuzipSize(bytes);
        }

        private static int CSHuffman3(byte[] _bytes)
        {
            byte[] bytes = (byte[])_bytes.Clone();
            int i = 0, c = bytes.Length;
            while (i < c) { if (i % 2 == 0) bytes[i] = (byte)~bytes[i]; i++; }
            return UnSafeHuffman.MuzipSize(bytes);
        }

        private static int CSRunLength0(byte[] _bytes)
        {
            int i = 1, c = _bytes.Length, again = 0, ret = 0; byte b = _bytes[0], key = 0xFF;
            while (i < c)
            {
                if (b == _bytes[i]) again++;
                else
                {
                    if (again == 0)
                    { if (b == key) { ret += 3; } else ret++; }
                    else { ret++; while (again >= 255) { ret++; again -= 255; } ret += 2; again = 0; }
                }
                b = _bytes[i];
                i++;
            }
            if (again == 0) { if (b == key) { ret += 3; } else ret++; }
            else { ret++; while (again >= 255) { ret++; again -= 255; } ret += 2; again = 0; }
            return ret;
        }

        private static int CSRunLength1(byte[] _bytes)
        {
            int i = 1, c = _bytes.Length, again = 0, ret = 0; byte b = _bytes[0], key = 0;
            while (i < c)
            {
                if (b == _bytes[i]) again++;
                else
                {
                    if (again == 0)
                    { if (b == key) { ret += 3; } else ret++; }
                    else { ret++; while (again >= 255) { ret++; again -= 255; } ret += 2; again = 0; }
                }
                b = _bytes[i];
                i++;
            }
            if (again == 0) { if (b == key) { ret += 3; } else ret++; }
            else { ret++; while (again >= 255) { ret++; again -= 255; } ret += 2; again = 0; }
            return ret;
        }

        private static int CSRunLength2(byte[] _bytes)
        {
            int i = 1, c = _bytes.Length, again = 0, ret = 0; byte b = _bytes[0], key = 128;
            while (i < c)
            {
                if (b == _bytes[i]) again++;
                else
                {
                    if (again == 0)
                    { if (b == key) { ret += 3; } else ret++; }
                    else { ret++; while (again >= 255) { ret++; again -= 255; } ret += 2; again = 0; }
                }
                b = _bytes[i];
                i++;
            }
            if (again == 0) { if (b == key) { ret += 3; } else ret++; }
            else { ret++; while (again >= 255) { ret++; again -= 255; } ret += 2; again = 0; }
            return ret;
        }

        private static int CSLZ78(byte[] _bytes, int compressedOthers)
        {
            return UnSafeLZ78.MuzipSize(_bytes, compressedOthers);
        }

        private static int CSLZ78_2(byte[] _bytes, int compressedOthers)
        {
            byte[] bytes = (byte[])_bytes.Clone();
            int i = 0, c = bytes.Length;
            while (i < c) { if (i % 2 == 0) bytes[i] = (byte)~bytes[i]; i++; }
            return UnSafeLZ78.MuzipSize(bytes, compressedOthers);
        }

        private static int CSLZ78_3(byte[] _bytes, int compressedOthers)
        {
            byte[] bytes = (byte[])_bytes.Clone();
            int i = 0, c = bytes.Length;
            while (i < c) { if (i % 3 == 0) bytes[i] = (byte)~bytes[i]; i++; }
            return UnSafeLZ78.MuzipSize(bytes, compressedOthers);
        }

        private static int bitCapacity(byte dl)
        {
            int ret = 8;
            if (dl >= 128) ret = 8;
            else if (dl >= 64 && dl < 128) ret = 7;
            else if (dl >= 32 && dl < 64) ret = 6;
            else if (dl >= 16 && dl < 32) ret = 5;
            else if (dl >= 8 && dl < 16) ret = 4;
            else if (dl >= 4 && dl < 8) ret = 3;
            else if (dl >= 2 && dl < 4) ret = 2;
            else if (dl >= 0 && dl < 2) ret = 1;
            return ret;
        }

        private static int bitCapacity(ushort dl)
        {
            int ret = 0;
            ushort i = 0, c = 16, line = 1;
            while (i < c)
            {
                if (dl >= line) ret++; else break; line = (ushort)(line << 1); i++;
            }
            if (ret == 0) ret = 1;
            return ret;
        }

        private static int bitCapacity(uint dl)
        {
            int ret = 0;
            uint i = 0, c = 32, line = 1;
            while (i < c)
            {
                if (dl >= line) ret++; else break; line = (uint)(line << 1); i++;
            }
            if (ret == 0) ret = 1;
            return ret;
        }
        private static int bitCapacity(ulong dl)
        {
            int ret = 0;
            ulong i = 0, c = 64, line = 1;
            while (i < c)
            {
                if (dl >= line) ret++; else break; line = (ulong)(line << 1); i++;
            }
            if (ret == 0) ret = 1;
            return ret;
        }
    }
}
