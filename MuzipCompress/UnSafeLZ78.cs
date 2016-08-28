using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuzipCompress
{
    class UnSafeLZ78
    {
        public static unsafe int MuzipSize(byte[] _bytes, int compressedOthers)
        {
            int ret = 0;
            int* indexes = stackalloc int[257];
            int i = 0, c = _bytes.Length, n0, n1, j, wordCount = 0, k, f, lastWordStart = 0; byte key = 0;
            bool exist = false, add = true;
            fixed (byte* _ptrBytes = _bytes)
            {
                byte* _ptrBytes1 = _ptrBytes;
                byte* _ptrBytes2 = _ptrBytes;
                while (i < c)
                {
                    if (add)
                    {
                        indexes[wordCount] = i;
                        lastWordStart = i;
                        add = false;
                        wordCount++;
                    }
                    if (wordCount == 257) { wordCount = 1; indexes[0] = indexes[256]; }
                    exist = false;
                    j = 0;
                    while (j < wordCount)
                    {
                        n0 = indexes[j]; n1 = indexes[j + 1];
                        if (n1 - n0 == i - lastWordStart + 1)
                        {
                            k = 0; f = i - lastWordStart + 1;
                            _ptrBytes1 += n0; _ptrBytes2 += lastWordStart;
                            while (k < f)
                            {
                                if (*_ptrBytes1 != *_ptrBytes2)
                                    break;
                                _ptrBytes1++; _ptrBytes2++; k++;
                            }
                            _ptrBytes1 -= n0 + k; _ptrBytes2 -= lastWordStart + k;
                            exist = k == f;
                            if (exist) { key = (byte)(j + 1); break; }
                        }
                        j++;
                    }
                    if (!exist) { add = true; ret += 2; key = 0; }
                    i++;
                    if (ret > compressedOthers) { ret = -1; break; }
                    if (i == c) { if (key != 0) ret++; }
                }
            }
            return ret;
        }

        public static unsafe byte[] MuzipLZ78(byte[] _bytes, int alloc)
        {
            byte[] ret = new byte[alloc]; List<byte> byteArray = new List<byte>();
            int* indexes = stackalloc int[257];
            int i = 0, c = _bytes.Length, n0, n1, j, wordCount = 0, k, f, lastWordStart = 0; byte key = 0;
            bool exist = false, add = true;
            fixed (byte* _ptrBytes = _bytes)
            {
                byte* __ptrBytes = _ptrBytes;
                byte* _ptrBytes1 = _ptrBytes;
                byte* _ptrBytes2 = _ptrBytes;
                fixed (byte* _ptrRet = ret)
                {
                    byte* __ptrRet = _ptrRet;
                    while (i < c)
                    {
                        if (add) { indexes[wordCount] = i; lastWordStart = i; add = false; wordCount++; }
                        if (wordCount == 257) { wordCount = 1; indexes[0] = indexes[256]; }
                        exist = false;
                        j = 0;
                        while (j < wordCount)
                        {
                            n0 = indexes[j]; n1 = indexes[j + 1];
                            if (n1 - n0 == i - lastWordStart + 1)
                            {
                                k = 0; f = i - lastWordStart + 1;
                                _ptrBytes1 += n0; _ptrBytes2 += lastWordStart;
                                while (k < f)
                                {
                                    if (*_ptrBytes1 != *_ptrBytes2)
                                        break;
                                    _ptrBytes1++; _ptrBytes2++; k++;
                                }
                                _ptrBytes1 -= n0 + k; _ptrBytes2 -= lastWordStart + k;
                                exist = k == f;
                                if (exist) { key = (byte)(j + 1); break; }
                            }
                            j++;
                        }
                        if (!exist) { add = true; *__ptrRet = key; __ptrRet++; *__ptrRet = *__ptrBytes; __ptrRet++; key = 0; }
                        __ptrBytes++;
                        i++;
                        if (i == c) { if (key != 0) *__ptrRet = key; }
                    }
                }
            }
            return ret;
        }

        public static byte[] UnMuzipLZ78(byte[] _bytes)
        {
            byte[] ret = null; List<byte> byteArray = new List<byte>();
            byte[][] words = new byte[257][];
            int i = 0, c = _bytes.Length, wordCount = 1, _len, j; byte key;
            words[0] = new byte[0];
            while (i < c)
            {
                key = _bytes[i];
                _len = words[key].Length;
                i++;
                if (i == c)
                {
                    words[wordCount] = new byte[_len];
                    Array.Copy(words[key], words[wordCount], _len);
                }
                else
                {
                    words[wordCount] = new byte[_len + 1];
                    Array.Copy(words[key], words[wordCount], _len);
                    words[wordCount][_len] = _bytes[i];
                }
                wordCount++;
                if (wordCount == 257)
                {
                    j = 0;
                    while (j < wordCount)
                    {
                        foreach (byte _b in words[j]) byteArray.Add(_b);
                        j++;
                    }
                    wordCount = 1;
                }
                i++;
            }
            if (wordCount != 0)
            {
                j = 0;
                while (j < wordCount)
                {
                    foreach (byte _b in words[j]) byteArray.Add(_b);
                    j++;
                }
                wordCount = 0;
            }
            i = 0; c = byteArray.Count;
            ret = new byte[c];
            while (i < c) { ret[i] = byteArray[i]; i++; }
            return ret;
        }
    }
}
