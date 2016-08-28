using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuzipCompress
{
    class MuzipBitArray
    {
        private byte[] _data;
        private int pointer, byte_pointer, k, b;

        public MuzipBitArray(int capacity) { _data = new byte[capacity]; pointer = 0; }

        public MuzipBitArray(byte[] data) { _data = data; pointer = 0; }

        public void WriteBits(byte _byte, int cap)
        {
            byte __byte = _byte;
            byte_pointer = pointer / 8;
            if (byte_pointer >= _data.Length) return;
            b = pointer % 8; k = 8 - b;
            if (k < cap)
            {
                _data[byte_pointer] += (byte)(__byte >> (cap - k));
                byte_pointer++;
                if (byte_pointer >= _data.Length) return;
                __byte = _byte;
                _data[byte_pointer] += (byte)(__byte << (8 + k - cap));
            }
            else if (k >= cap)
            {
                _data[byte_pointer] += (byte)(__byte << (k - cap));
            }
            pointer += cap;
        }

        public byte ReadBits(int cap)
        {
            byte ret = 0, _byte = 0;
            byte_pointer = pointer / 8;
            if (byte_pointer >= _data.Length) return ret;
            b = pointer % 8; k = 8 - b;
            _byte = _data[byte_pointer];
            if (k < cap)
            {
                ret = (byte)(_byte << (cap - k));
                byte_pointer++;
                if (byte_pointer >= _data.Length) return ret;
                _byte = _data[byte_pointer];
                ret += (byte)(_byte >> (8 + k - cap));
            }
            else if (k >= cap)
            {
                ret += (byte)(_byte >> (k - cap));
            }
            ret = (byte)(ret & (0xFF >> (8 - cap)));
            pointer += cap;
            return ret;
        }
        public void CutBytes(int lastCount)
        {
            byte[] __data = new byte[_data.Length - lastCount];
            int i = 0, c = __data.Length;
            while (i < c) { __data[i] = _data[i]; i++; }
            _data = __data;
        }

        public bool EndOfArray { get { byte_pointer = pointer / 8; return byte_pointer >= _data.Length; } }
        public int Pointer { get { return pointer; } set { pointer = value; } }
        public byte[] Data { get { return _data; } }
        public byte[] DataClone { get { return (byte[])_data.Clone(); } }
        public void ResetBytes() { int i = 0, c = _data.Length; while (i < c) { _data[i] = 0; i++; } }
        public bool Compare(MuzipBitArray _comp)
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
