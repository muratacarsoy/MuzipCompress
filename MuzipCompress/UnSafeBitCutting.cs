using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuzipCompress
{
    class UnSafeBitCutting
    {
        public static unsafe byte[] Module1(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone(); int i = 0, c = bytes.Length;
            byte min = 255, max = 0, parameter, delta;
            while (i < c) { if (min > bytes[i]) min = bytes[i]; if (max < bytes[i]) max = bytes[i]; i++; }
            parameter = min; delta = (byte)(max - min);
            i = 0;
            while (i < c) { bytes[i] -= min; i++; }
            int cap = bitCapacity(delta), k = 8;
            if (cap == 8) return null;
            if ((cap * c) % 8 != 0) return null;
            int alloc = cap * c / 8 + 2;
            byte _byte = 0, _addByte = 0;
            ret = new byte[alloc];
            fixed (byte* _ptrBytes = bytes)
            {
                byte* ptrBytes = _ptrBytes;
                fixed (byte* _ptrRet = ret)
                {
                    byte* ptrRet = _ptrRet;
                    *ptrRet = parameter; ptrRet++; *ptrRet = delta; ptrRet++;
                    i = 0;
                    while (i < c)
                    {
                        _byte = *ptrBytes;
                        if (k < cap)
                        {
                            k = cap - k; _byte = (byte)(_byte >> k); _addByte += _byte;
                            *ptrRet = _addByte; ptrRet++;
                            _addByte = 0; _byte = *ptrBytes;
                            k = 8 - k; _byte = (byte)(_byte << k); _addByte += _byte;
                        }
                        else if (k > cap) { k = k - cap; _byte = (byte)(_byte << k); _addByte += _byte; }
                        else if (k == cap)
                        {
                            _addByte += _byte;
                            *ptrRet = _addByte; ptrRet++;
                            _addByte = 0; k = 8;
                        }
                        ptrBytes++;
                        i++;
                    }
                }
            }
            return ret;
        }

        public static unsafe byte[] Module2(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone(); int i = 0, c = bytes.Length;
            if (c % 2 != 0) return null;
            ushort min = 65535, max = 0, parameter, delta;
            ushort[] shorts = new ushort[c / 2];
            c = shorts.Length;
            while (i < c) { shorts[i] = BitConverter.ToUInt16(bytes, 2 * i); i++; }
            i = 0;
            while (i < c) { if (min > shorts[i]) min = shorts[i]; if (max < shorts[i]) max = shorts[i]; i++; }
            parameter = min; delta = (ushort)(max - min);
            i = 0;
            while (i < c) { shorts[i] -= min; i++; }
            int cap = bitCapacity(delta), k = 16;
            if (cap == 16) return null;
            if ((cap * c) % 16 != 0) return null;
            ushort[] retShorts = new ushort[cap * c / 16 + 2];
            ushort _short = 0, _addShort = 0;
            fixed (ushort* _ptrShorts = shorts)
            {
                ushort* ptrShorts = _ptrShorts;
                fixed (ushort* _ptrRetShorts = retShorts)
                {
                    ushort* ptrRetShorts = _ptrRetShorts;
                    *ptrRetShorts = parameter; ptrRetShorts++; *ptrRetShorts = delta; ptrRetShorts++;
                    i = 0;
                    while (i < c)
                    {
                        _short = *ptrShorts;
                        if (k < cap)
                        {
                            k = cap - k; _short = (ushort)(_short >> k); _addShort += _short;
                            *ptrRetShorts = _addShort; ptrRetShorts++;
                            _addShort = 0; _short = *ptrShorts;
                            k = 16 - k; _short = (ushort)(_short << k); _addShort += _short;
                        }
                        else if (k > cap) { k = k - cap; _short = (ushort)(_short << k); _addShort += _short; }
                        else if (k == cap)
                        {
                            _addShort += _short;
                            *ptrRetShorts = _addShort; ptrRetShorts++;
                            _addShort = 0; k = 16;
                        }
                        i++; ptrShorts++;
                    }
                }
            }
            c = retShorts.Length;
            ret = new byte[2 * c]; i = 0;
            while (i < c)
            {
                byte[] __shortBytes = BitConverter.GetBytes(retShorts[i]);
                ret[2 * i] = __shortBytes[0]; ret[2 * i + 1] = __shortBytes[1];
                i++;
            }
            return ret;
        }

        public static unsafe byte[] Module3(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone(); int i = 0, c = bytes.Length;
            if (c % 4 != 0) return null;
            System.Collections.ObjectModel.Collection<uint> intArray = new System.Collections.ObjectModel.Collection<uint>();
            uint min = 0xFFFFFFFF, max = 0, parameter, delta;
            uint[] ints = new uint[c / 4];
            c = ints.Length;
            while (i < c) { ints[i] = BitConverter.ToUInt32(bytes, 4 * i); i++; }
            i = 0;
            while (i < c) { if (min > ints[i]) min = ints[i]; if (max < ints[i]) max = ints[i]; i++; }
            parameter = min; delta = (uint)(max - min);
            i = 0;
            while (i < c) { ints[i] -= min; i++; }
            intArray.Add(parameter); intArray.Add(delta);
            int cap = bitCapacity(delta), k = 32;
            if (cap == 32) return null;
            if ((cap * c) % 32 != 0) return null;
            uint[] retInts = new uint[(cap * c) / 32 + 2];
            uint _int = 0, _addInt = 0;
            fixed (uint* _ptrInts = ints)
            {
                uint* ptrInts = _ptrInts;
                fixed (uint* _ptrRetInts = retInts)
                {
                    uint* ptrRetInts = _ptrRetInts;
                    *ptrRetInts = parameter; ptrRetInts++; *ptrRetInts = delta; ptrRetInts++;
                    i = 0;
                    while (i < c)
                    {
                        _int = *ptrInts;
                        if (k < cap)
                        {
                            k = cap - k; _int = (uint)(_int >> k); _addInt += _int;
                            *ptrRetInts = _addInt; ptrRetInts++;
                            _addInt = 0; _int = ints[i];
                            k = 32 - k; _int = (uint)(_int << k); _addInt += _int;
                        }
                        else if (k > cap) { k = k - cap; _int = (uint)(_int << k); _addInt += _int; }
                        else if (k == cap) { _addInt += _int; *ptrRetInts = _addInt; ptrRetInts++; _addInt = 0; k = 32; }
                        i++; ptrInts++;
                    }
                }
            }
            c = retInts.Length;
            ret = new byte[4 * c]; i = 0;
            while (i < c)
            {
                byte[] __intBytes = BitConverter.GetBytes(retInts[i]);
                ret[4 * i] = __intBytes[0]; ret[4 * i + 1] = __intBytes[1];
                ret[4 * i + 2] = __intBytes[2]; ret[4 * i + 3] = __intBytes[3];
                i++;
            }
            return ret;
        }

        public static unsafe byte[] Module4(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone(); int i = 0, c = bytes.Length;
            if (c % 8 != 0) return null;
            ulong min = 0xFFFFFFFFFFFFFFFF, max = 0, parameter, delta;
            ulong[] longs = new ulong[c / 8];
            c = longs.Length;
            while (i < c) { longs[i] = BitConverter.ToUInt64(bytes, 8 * i); i++; }
            i = 0;
            while (i < c) { if (min > longs[i]) min = longs[i]; if (max < longs[i]) max = longs[i]; i++; }
            parameter = min; delta = (ulong)(max - min);
            i = 0;
            while (i < c) { longs[i] -= min; i++; }
            int cap = bitCapacity(delta), k = 64;
            if (cap == 64) return null;
            if ((cap * c) % 64 != 0) return null;
            ulong[] retLongs = new ulong[(cap * c) / 64 + 2];
            ulong _long = 0, _addLong = 0;
            fixed (ulong* _ptrLongs = longs)
            {
                ulong* ptrLongs = _ptrLongs;
                fixed (ulong* _ptrRetLongs = retLongs)
                {
                    ulong* ptrRetLongs = _ptrRetLongs;
                    *ptrRetLongs = parameter; ptrRetLongs++; *ptrRetLongs = delta; ptrRetLongs++;
                    i = 0;
                    while (i < c)
                    {
                        _long = *ptrLongs;
                        if (k < cap)
                        {
                            k = cap - k; _long = (ulong)(_long >> k); _addLong += _long;
                            *ptrRetLongs = _addLong; ptrRetLongs++;
                            _addLong = 0; _long = longs[i];
                            k = 64 - k; _long = (ulong)(_long << k); _addLong += _long;
                        }
                        else if (k > cap) { k = k - cap; _long = (ulong)(_long << k); _addLong += _long; }
                        else if (k == cap) { _addLong += _long; *ptrRetLongs = _addLong; ptrRetLongs++; _addLong = 0; k = 64; }
                        i++; ptrLongs++;
                    }
                }
            }
            c = retLongs.Length;
            ret = new byte[8 * c]; i = 0;
            while (i < c)
            {
                byte[] __longBytes = BitConverter.GetBytes(retLongs[i]);
                ret[8 * i] = __longBytes[0]; ret[8 * i + 1] = __longBytes[1];
                ret[8 * i + 2] = __longBytes[2]; ret[8 * i + 3] = __longBytes[3];
                ret[8 * i + 4] = __longBytes[4]; ret[8 * i + 5] = __longBytes[5];
                ret[8 * i + 6] = __longBytes[6]; ret[8 * i + 7] = __longBytes[7];
                i++;
            }
            return ret;
        }

        public static byte[] Module5(byte[] _bytes)
        {
            byte[] ret = null, bytes = (byte[])_bytes.Clone();
            int i = 0, c = _bytes.Length; byte maxA = 0, minB = 255, b = 0;
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
            byte[] module1ret = Module1(bytes);
            if (module1ret == null) return null;
            ret = new byte[2 + module1ret.Length];
            Array.Copy(module1ret, 0, ret, 2, module1ret.Length);
            ret[0] = maxA; ret[1] = minB;
            return ret;
        }

        public static byte[] UnModule1(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            System.Collections.ObjectModel.Collection<byte> byteArray = new System.Collections.ObjectModel.Collection<byte>();
            byte min = bytes[0], delta = bytes[1], _byte = 0, _takeByte = 0;
            int cap = bitCapacity(delta), k = 8, i = 2, c = bytes.Length;
            while (i < c)
            {
                _byte = bytes[i];
                _takeByte = _byte;
                _takeByte = (byte)(_takeByte & ((byte)(0xFF >> (8 - k))));
                if (k > cap) { k = k - cap; _takeByte = (byte)(_takeByte >> k); _takeByte += min; byteArray.Add(_takeByte); }
                else if (k < cap)
                {
                    k = cap - k; _takeByte = (byte)(_takeByte << k); i++;
                    _byte = bytes[i]; k = 8 - k; _byte = (byte)(_byte >> k);
                    _takeByte += _byte; _takeByte += min; byteArray.Add(_takeByte);
                }
                else if (k == cap) { _takeByte += min; byteArray.Add(_takeByte); k = 8; i++; }
            }
            ret = new byte[byteArray.Count];
            i = 0;
            while (i < ret.Length) { ret[i] = byteArray[i]; i++; }
            return ret;
        }

        public static byte[] UnModule2(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            ushort[] shorts = new ushort[bytes.Length / 2];
            int i = 0, c = shorts.Length;
            while (i < c) { shorts[i] = BitConverter.ToUInt16(bytes, 2 * i); i++; }
            System.Collections.ObjectModel.Collection<ushort> shortArray = new System.Collections.ObjectModel.Collection<ushort>();
            ushort min = shorts[0], delta = shorts[1], _short = 0, _takeShort = 0;
            int cap = bitCapacity(delta), k = 16; i = 2;
            while (i < c)
            {
                _short = shorts[i];
                _takeShort = _short;
                _takeShort = (ushort)(_takeShort & ((ushort)(0xFFFF >> (16 - k))));
                if (k > cap)
                {
                    k = k - cap;
                    _takeShort = (ushort)(_takeShort >> k);
                    _takeShort += min; shortArray.Add(_takeShort);
                }
                else if (k < cap)
                {
                    k = cap - k; _takeShort = (ushort)(_takeShort << k); i++;
                    _short = shorts[i]; k = 16 - k; _short = (ushort)(_short >> k);
                    _takeShort += _short; _takeShort += min; shortArray.Add(_takeShort);
                }
                else if (k == cap)
                {
                    _takeShort += min; shortArray.Add(_takeShort); k = 16; i++;
                }
            }
            ret = new byte[shortArray.Count * 2]; i = 0; c = shortArray.Count;
            while (i < c) { byte[] __shortBytes = BitConverter.GetBytes(shortArray[i]); ret[2 * i] = __shortBytes[0]; ret[2 * i + 1] = __shortBytes[1]; i++; }
            return ret;
        }

        public static byte[] UnModule3(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            uint[] ints = new uint[bytes.Length / 4];
            int i = 0, c = ints.Length;
            while (i < c) { ints[i] = BitConverter.ToUInt32(bytes, 4 * i); i++; }
            System.Collections.ObjectModel.Collection<uint> intArray = new System.Collections.ObjectModel.Collection<uint>();
            uint min = ints[0], delta = ints[1], _int = 0, _takeInt = 0;
            int cap = bitCapacity(delta), k = 32; i = 2;
            while (i < c)
            {
                _int = ints[i];
                _takeInt = _int;
                _takeInt = (uint)(_takeInt & ((uint)(0xFFFFFFFF >> (32 - k))));
                if (k > cap)
                {
                    k = k - cap;
                    _takeInt = (uint)(_takeInt >> k);
                    _takeInt += min; intArray.Add(_takeInt);
                }
                else if (k < cap)
                {
                    k = cap - k; _takeInt = (uint)(_takeInt << k); i++;
                    _int = ints[i]; k = 32 - k; _int = (uint)(_int >> k);
                    _takeInt += _int; _takeInt += min; intArray.Add(_takeInt);
                }
                else if (k == cap)
                {
                    _takeInt += min; intArray.Add(_takeInt); k = 32; i++;
                }
            }
            ret = new byte[intArray.Count * 4]; i = 0; c = intArray.Count;
            while (i < c)
            {
                byte[] __intBytes = BitConverter.GetBytes(intArray[i]);
                ret[4 * i] = __intBytes[0];
                ret[4 * i + 1] = __intBytes[1];
                ret[4 * i + 2] = __intBytes[2];
                ret[4 * i + 3] = __intBytes[3];
                i++;
            }
            return ret;
        }

        public static byte[] UnModule4(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            ulong[] longs = new ulong[bytes.Length / 8];
            int i = 0, c = longs.Length;
            while (i < c) { longs[i] = BitConverter.ToUInt64(bytes, 8 * i); i++; }
            System.Collections.ObjectModel.Collection<ulong> longArray = new System.Collections.ObjectModel.Collection<ulong>();
            ulong min = longs[0], delta = longs[1], _long = 0, _takeLong = 0;
            int cap = bitCapacity(delta), k = 64; i = 2;
            while (i < c)
            {
                _long = longs[i];
                _takeLong = _long;
                _takeLong = (ulong)(_takeLong & ((ulong)(0xFFFFFFFFFFFFFFFF >> (64 - k))));
                if (k > cap)
                {
                    k = k - cap;
                    _takeLong = (ulong)(_takeLong >> k);
                    _takeLong += min; longArray.Add(_takeLong);
                }
                else if (k < cap)
                {
                    k = cap - k; _takeLong = (ulong)(_takeLong << k); i++;
                    _long = longs[i]; k = 64 - k; _long = (ulong)(_long >> k);
                    _takeLong += _long; _takeLong += min; longArray.Add(_takeLong);
                }
                else if (k == cap)
                {
                    _takeLong += min; longArray.Add(_takeLong); k = 64; i++;
                }
            }
            ret = new byte[longArray.Count * 8]; i = 0; c = longArray.Count;
            while (i < c)
            {
                byte[] __longBytes = BitConverter.GetBytes(longArray[i]);
                ret[8 * i] = __longBytes[0]; ret[8 * i + 1] = __longBytes[1];
                ret[8 * i + 2] = __longBytes[2]; ret[8 * i + 3] = __longBytes[3];
                ret[8 * i + 4] = __longBytes[4]; ret[8 * i + 5] = __longBytes[5];
                ret[8 * i + 6] = __longBytes[6]; ret[8 * i + 7] = __longBytes[7];
                i++;
            }
            return ret;
        }

        public static byte[] UnModule5(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = new byte[_bytes.Length - 2];
            byte maxA = _bytes[0], minB = _bytes[1], b = 0;
            Array.Copy(_bytes, 2, bytes, 0, bytes.Length);
            byte[] unmodule1ret = UnModule1(bytes);
            int i = 0, c = unmodule1ret.Length;
            while (i < c)
            {
                b = unmodule1ret[i];
                if (b < 128) { unmodule1ret[i] -= maxA; } else { unmodule1ret[i] += minB; }
                i++;
            }
            ret = unmodule1ret;
            return ret;
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
