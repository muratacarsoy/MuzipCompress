using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuzipCompress
{
    unsafe class MuzipBitPointer
    {
        private byte[] _data;
        private int byte_pointer, k;
        private byte* __ptrData;

        public MuzipBitPointer(int capacity) { _data = new byte[capacity]; byte_pointer = 0; k = 8; }

        public MuzipBitPointer(byte[] data)
        {
            _data = data; byte_pointer = 0; k = 8;
            fixed (byte* _ptrData = _data)
            {
                __ptrData = _ptrData;
            }
        }

        public unsafe void WriteBits(byte _byte, int cap)
        {
            byte __byte = _byte;
            if (byte_pointer >= _data.Length) return;
            if (k < cap)
            {
                k = cap - k;
                *__ptrData += (byte)(__byte >> k);
                //_data[byte_pointer] += (byte)(__byte >> (cap - k));
                __ptrData++;
                byte_pointer++;
                if (byte_pointer >= _data.Length) return;
                __byte = _byte; k = 8 - k;
                *__ptrData += (byte)(__byte << k);
                //_data[byte_pointer] += (byte)(__byte << (8 + k - cap));
            }
            else if (k > cap)
            {
                k = k - cap;
                *__ptrData += (byte)(__byte << k);
                //_data[byte_pointer] += (byte)(__byte << (k - cap));
            }
            else if (k == cap)
            {
                *__ptrData += __byte; __ptrData++; byte_pointer++; k = 8;
            }
        }

        public void CutBytes(int lastCount)
        {
            byte[] __data = new byte[_data.Length - lastCount];
            int i = 0, c = __data.Length;
            while (i < c) { __data[i] = _data[i]; i++; }
            _data = __data;
        }

        public bool EndOfArray { get { return byte_pointer >= _data.Length; } }
        public int Pointer { get { return byte_pointer; } }
        public byte[] Data { get { return _data; } }
        public byte[] DataClone { get { return (byte[])_data.Clone(); } }
        public void ResetBytes() { int i = 0, c = _data.Length; while (i < c) { _data[i] = 0; i++; } }
        public bool Compare(MuzipBitPointer _comp)
        {
            bool ret = false; int i = 0, c = _comp._data.Length;
            if (c != _data.Length) return ret;
            ret = true;
            while (i < c)
            {
                if (_comp._data[i] != _data[i]) { ret = false; break; }
                i++;
            }
            return ret;
        }
        public bool Compare(byte[] _bytes)
        {
            bool ret = false; int i = 0, c = _bytes.Length;
            if (c > _data.Length) return ret;
            ret = true;
            while (i < c)
            {
                if (_bytes[i] != _data[i]) { ret = false; break; }
                i++;
            }
            return ret;
        }
    }
}
