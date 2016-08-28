using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuzipCompress
{
    class MuzipModules
    {
        public static byte[] Muzip(byte[] _bytes, byte mod, int alloc)
        {
            byte[] ret = null;
            switch (mod)
            {
                case 0: ret = Module0(_bytes); break;
                case 1: ret = Module1(_bytes); break;
                case 2: ret = Module2(_bytes); break;
                case 3: ret = Module3(_bytes); break;
                case 4: ret = Module4(_bytes); break;
                case 5: ret = Module5(_bytes); break;
                case 20: ret = ModuleHuffman(_bytes, alloc); break;
                case 21: ret = ModuleHuffman2(_bytes); break;
                case 22: ret = ModuleHuffman3(_bytes); break;
                case 30: ret = ModuleRunLength0(_bytes); break;
                case 31: ret = ModuleRunLength1(_bytes); break;
                case 32: ret = ModuleRunLength2(_bytes); break;
                case 40: ret = ModuleLZ78(_bytes, alloc); break;
                case 41: ret = ModuleLZ78_2(_bytes, alloc); break;
                case 42: ret = ModuleLZ78_3(_bytes, alloc); break;
                case 255: ret = ModuleNon(_bytes); break;
            }
            return ret;
        }

        public static byte[] UnMuzip(byte[] _bytes, byte mod)
        {
            byte[] ret = null;
            switch (mod)
            {
                case 0: ret = UnModule0(_bytes); break;
                case 1: ret = UnModule1(_bytes); break;
                case 2: ret = UnModule2(_bytes); break;
                case 3: ret = UnModule3(_bytes); break;
                case 4: ret = UnModule4(_bytes); break;
                case 5: ret = UnModule5(_bytes); break;
                case 20: ret = UnModuleHuffman(_bytes); break;
                case 21: ret = UnModuleHuffman2(_bytes); break;
                case 22: ret = UnModuleHuffman3(_bytes); break;
                case 30: ret = UnModuleRunLength0(_bytes); break;
                case 31: ret = UnModuleRunLength1(_bytes); break;
                case 32: ret = UnModuleRunLength2(_bytes); break;
                case 40: ret = UnModuleLZ78(_bytes); break;
                case 41: ret = UnModuleLZ78_2(_bytes); break;
                case 42: ret = UnModuleLZ78_3(_bytes); break;
                case 255: ret = UnModuleNon(_bytes); break;
            }
            return ret;
        }

        public static byte[] Module0(byte[] _bytes)
        {
            byte[] ret = RunLength.Module0(_bytes); return ret;
        }

        public static byte[] UnModule0(byte[] _bytes)
        {
            byte[] ret = RunLength.UnModule0(_bytes); return ret;
        }

        public static byte[] Module1(byte[] _bytes)
        {
            byte[] ret = UnSafeBitCutting.Module1(_bytes); return ret;
        }

        public static byte[] Module2(byte[] _bytes)
        {
            byte[] ret = UnSafeBitCutting.Module2(_bytes); return ret;
        }

        public static byte[] Module3(byte[] _bytes)
        {
            byte[] ret = UnSafeBitCutting.Module3(_bytes); return ret;
        }

        public static byte[] Module4(byte[] _bytes)
        {
            byte[] ret = UnSafeBitCutting.Module4(_bytes); return ret;
        }

        public static byte[] Module5(byte[] _bytes)
        {
            byte[] ret = UnSafeBitCutting.Module5(_bytes); return ret;
        }

        public static byte[] ModuleHuffman(byte[] _bytes, int alloc)
        {
            byte[] ret = null;
            ret = UnSafeHuffman.MuzipHuffman(_bytes);
            return ret;
        }

        public static byte[] UnModuleHuffman(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            ret = Huffman.UnzipHuffman(bytes);
            return ret;
        }

        public static byte[] ModuleHuffman2(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            int i = 0, c = bytes.Length;
            while (i < c) { if (i % 2 == 0) bytes[i] = (byte)~bytes[i]; i++; }
            ret = UnSafeHuffman.MuzipHuffman(bytes);
            return ret;
        }

        public static byte[] UnModuleHuffman2(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            ret = Huffman.UnzipHuffman(bytes);
            int i = 0, c = ret.Length;
            while (i < c) { if (i % 2 == 0) ret[i] = (byte)~ret[i]; i++; }
            return ret;
        }

        public static byte[] ModuleHuffman3(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            int i = 0, c = bytes.Length;
            while (i < c) { if (i % 3 == 0) bytes[i] = (byte)~bytes[i]; i++; }
            ret = UnSafeHuffman.MuzipHuffman(bytes);
            return ret;
        }

        public static byte[] UnModuleHuffman3(byte[] _bytes)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            ret = Huffman.UnzipHuffman(bytes);
            int i = 0, c = ret.Length;
            while (i < c) { if (i % 3 == 0) ret[i] = (byte)~ret[i]; i++; }
            return ret;
        }

        public static byte[] ModuleRunLength0(byte[] _bytes)
        {
            byte[] ret = RunLength.ModuleRunLength0(_bytes); return ret;
        }

        public static byte[] ModuleRunLength1(byte[] _bytes)
        {
            byte[] ret = RunLength.ModuleRunLength1(_bytes); return ret;
        }

        public static byte[] ModuleRunLength2(byte[] _bytes)
        {
            byte[] ret = RunLength.ModuleRunLength2(_bytes); return ret;
        }

        public static byte[] UnModuleRunLength0(byte[] _bytes)
        {
            byte[] ret = RunLength.UnModuleRunLength0(_bytes); return ret;
        }

        public static byte[] UnModuleRunLength1(byte[] _bytes)
        {
            byte[] ret = RunLength.UnModuleRunLength1(_bytes); return ret;
        }

        public static byte[] UnModuleRunLength2(byte[] _bytes)
        {
            byte[] ret = RunLength.UnModuleRunLength2(_bytes); return ret;
        }

        public static byte[] ModuleLZ78(byte[] _bytes, int alloc)
        {
            byte[] ret = UnSafeLZ78.MuzipLZ78(_bytes, alloc); return ret;
        }

        public static byte[] ModuleLZ78_2(byte[] _bytes, int alloc)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            int i = 0, c = bytes.Length;
            while (i < c) { if (i % 2 == 0) bytes[i] = (byte)~bytes[i]; i++; }
            ret = UnSafeLZ78.MuzipLZ78(bytes, alloc);
            return ret;
        }

        public static byte[] ModuleLZ78_3(byte[] _bytes, int alloc)
        {
            byte[] ret = null; byte[] bytes = (byte[])_bytes.Clone();
            int i = 0, c = bytes.Length;
            while (i < c) { if (i % 3 == 0) bytes[i] = (byte)~bytes[i]; i++; }
            ret = UnSafeLZ78.MuzipLZ78(bytes, alloc);
            return ret;
        }

        public static byte[] UnModuleLZ78(byte[] _bytes)
        {
            byte[] ret = LZ78.UnMuzipLZ78(_bytes); return ret;
        }

        public static byte[] UnModuleLZ78_2(byte[] _bytes)
        {
            byte[] ret = LZ78.UnMuzipLZ78(_bytes);
            int i = 0, c = ret.Length;
            while (i < c) { if (i % 2 == 0) ret[i] = (byte)~ret[i]; i++; }
            return ret;
        }

        public static byte[] UnModuleLZ78_3(byte[] _bytes)
        {
            byte[] ret = LZ78.UnMuzipLZ78(_bytes);
            int i = 0, c = ret.Length;
            while (i < c) { if (i % 3 == 0) ret[i] = (byte)~ret[i]; i++; }
            return ret;
        }

        public static byte[] ModuleNon(byte[] _bytes)
        {
            return (byte[])_bytes.Clone();
        }

        public static byte[] UnModuleNon(byte[] _bytes)
        {
            return (byte[])_bytes.Clone();
        }

        public static byte[] UnModule1(byte[] _bytes)
        {
            byte[] ret = BitCutting.UnModule1(_bytes); return ret;
        }

        public static byte[] UnModule2(byte[] _bytes)
        {
            byte[] ret = BitCutting.UnModule2(_bytes); return ret;
        }

        public static byte[] UnModule3(byte[] _bytes)
        {
            byte[] ret = BitCutting.UnModule3(_bytes); return ret;
        }

        public static byte[] UnModule4(byte[] _bytes)
        {
            byte[] ret = BitCutting.UnModule4(_bytes); return ret;
        }

        public static byte[] UnModule5(byte[] _bytes)
        {
            byte[] ret = BitCutting.UnModule5(_bytes); return ret;
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
