using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace MuzipCompress
{
    public class CompressingEventArgs : EventArgs
    {
        public byte Layer { get; set; }
        public int Progress { get; set; }
        public int PartsCount { get; set; }
    }

    public class UnCompressingEventArgs
    {
        public byte Layer { get; set; }
        public int Progress { get; set; }
        public int PartsCount { get; set; }
    }

    public delegate void CompressingEventHandler(object sender, CompressingEventArgs e);
    public delegate void UnCompressingEventHandler(object sender, UnCompressingEventArgs e);

    class MuzipFile
    {
        public MuzipFile() { }
        public string FilePath { get; set; }
        public byte MaxLayer { get; set; }

        public event CompressingEventHandler CompressingProgress;
        public event PreparePartsEventHandler PreparingPartsProgress;
        public event UnCompressingEventHandler UnCompressingProgress;

        public void OnPreparingPartsProgress(PreparePartsEventArgs e)
        {
            PreparePartsEventHandler handler = PreparingPartsProgress;
            if (handler != null) handler(this, e);
        }

        public void OnCompressingProgress(CompressingEventArgs e)
        {
            CompressingEventHandler handler = CompressingProgress;
            if (handler != null) handler(this, e);
        }

        public void OnUnCompressingProgress(UnCompressingEventArgs e)
        {
            UnCompressingEventHandler handler = UnCompressingProgress;
            if (handler != null) handler(this, e);
        }

        public unsafe byte[] _CompressBytes(int block, byte[] bytes)
        {
            byte[] fs_bytes = bytes;
            long len = bytes.Length, comp_len;
            byte[][] parts = _SplitParts(fs_bytes, block);
            byte[] comp_bytes = null;
            byte lay_i = 1; CompressingEventArgs e = new CompressingEventArgs();
            while (lay_i <= MaxLayer)
            {
                MuzipLayerData lay = new MuzipLayerData();
                lay.PartCalculated += lay_PartCalculated;
                comp_len = lay.PrepareParts(parts); lay.Layer = lay_i; e.Layer = lay_i;
                byte[] by = lay.GetBytesData();
                if (comp_len > len && lay_i > 1) { break; }
                comp_bytes = new byte[comp_len];
                int i, c, j = 0, d;
                fixed (byte* _ptrCompBytes = comp_bytes)
                {
                    byte* ptrCompBytes = _ptrCompBytes;
                    fixed (byte* _ptrBy = by)
                    {
                        byte* ptrBy = _ptrBy;
                        j = 0; d = by.Length;
                        while (j < d) { *ptrCompBytes = *ptrBy; ptrCompBytes++; ptrBy++; j++; }
                    }

                    i = 0; c = lay.Parts.Count; j = 0; e.PartsCount = c;
                    while (i < c)
                    {
                        byte[] cmp = MuzipModules.Muzip(parts[i], lay.Parts[i].Module, lay.Parts[i].MuzipLength);
                        fixed (byte* _ptrCmp = cmp)
                        {
                            byte* ptrCmp = _ptrCmp;
                            j = 0; d = cmp.Length;
                            while (j < d) { *ptrCompBytes = *ptrCmp; ptrCompBytes++; ptrCmp++; j++; }
                        }
                        e.Progress = i; OnCompressingProgress(e);
                        i++;
                    }
                }
                len = comp_bytes.Length;
                parts = _SplitParts(comp_bytes, block);
                lay_i++;
            }
            return comp_bytes;
        }

        public unsafe byte[] _CompressFile(int block)
        {
            if (!File.Exists(FilePath)) return null;
            FileStream fs = new FileStream(FilePath, FileMode.Open);
            byte[] fs_bytes = new byte[fs.Length];
            long len = fs.Length, comp_len;
            fs.Read(fs_bytes, 0, fs_bytes.Length);
            fs.Close();
            byte[][] parts = _SplitParts(fs_bytes, block);
            byte[] comp_bytes = null;
            byte lay_i = 1; CompressingEventArgs e = new CompressingEventArgs();
            while (lay_i <= MaxLayer)
            {
                MuzipLayerData lay = new MuzipLayerData();
                lay.PartCalculated += lay_PartCalculated;
                comp_len = lay.PrepareParts(parts); lay.Layer = lay_i; e.Layer = lay_i;
                byte[] by = lay.GetBytesData();
                if (comp_len > len && lay_i > 1) { break; }
                comp_bytes = new byte[comp_len];
                int i, c, j = 0, d;
                fixed (byte* _ptrCompBytes = comp_bytes)
                {
                    byte* ptrCompBytes = _ptrCompBytes;
                    fixed (byte* _ptrBy = by)
                    {
                        byte* ptrBy = _ptrBy;
                        j = 0; d = by.Length;
                        while (j < d) { *ptrCompBytes = *ptrBy; ptrCompBytes++; ptrBy++; j++; }
                    }
                    i = 0; c = lay.Parts.Count; j = 0; e.PartsCount = c;
                    while (i < c)
                    {
                        byte[] cmp = MuzipModules.Muzip(parts[i], lay.Parts[i].Module, lay.Parts[i].MuzipLength);
                        fixed (byte* _ptrCmp = cmp)
                        {
                            byte* ptrCmp = _ptrCmp;
                            j = 0; d = cmp.Length;
                            while (j < d) { *ptrCompBytes = *ptrCmp; ptrCompBytes++; ptrCmp++; j++; }
                        }
                        e.Progress = i; OnCompressingProgress(e);
                        i++;
                    }
                }
                len = comp_bytes.Length;
                parts = _SplitParts(comp_bytes, block);
                lay_i++;
            }
            return comp_bytes;
        }

        void lay_PartCalculated(object sender, PreparePartsEventArgs e)
        {
            OnPreparingPartsProgress(e);
        }

        private unsafe byte[][] _SplitParts(byte[] _bytes, int splitMethod)
        {
            int i = 0, c = _bytes.Length, l = 512, p, j;
            while (i < splitMethod) { l *= 2; i++; }
            i = c; p = (c % l == 0) ? c / l : (c - c % l) / l + 1;
            byte[][] ret = new byte[p][]; p = 0;
            fixed (byte* _ptrMem = _bytes)
            {
                byte* ptrMem = _ptrMem;
                while (i > 0)
                {
                    byte[] part = new byte[i > l ? l : i];
                    fixed (byte* _ptrPart = part)
                    {
                        byte* ptrPart = _ptrPart;
                        j = 0;
                        while (j < part.Length)
                        {
                            *ptrPart = *ptrMem; ptrMem++;
                            j++; ptrPart++;
                        }
                        ret[p] = part;
                        p++;
                        i -= l;
                    }
                }
            }
            return ret;
        }

        public byte[] UnCompressBytes(byte[] bytes)
        {
            MemoryStream mem = new MemoryStream(bytes);
            byte[] ret = null;
            bool willEnd = true; UnCompressingEventArgs e = new UnCompressingEventArgs();
            while (willEnd)
            {
                mem.Position = 0;
                byte layerCount = (byte)mem.ReadByte(); willEnd = layerCount != 1; e.Layer = layerCount;
                byte[] partCount = new byte[4];
                partCount[0] = (byte)mem.ReadByte(); partCount[1] = (byte)mem.ReadByte();
                partCount[2] = (byte)mem.ReadByte(); partCount[3] = (byte)mem.ReadByte();
                int _partCount = BitConverter.ToInt32(partCount, 0); e.PartsCount = _partCount;
                MuzipLayerData lay = new MuzipLayerData();
                lay.Layer = layerCount;
                byte[][] parts = new byte[_partCount][];
                int i = 0, j = 0, total_len = 0;
                while (j < _partCount)
                {
                    byte _module = (byte)mem.ReadByte();
                    byte[] _len = new byte[2];
                    _len[0] = (byte)mem.ReadByte();
                    _len[1] = (byte)mem.ReadByte();
                    ushort __len = BitConverter.ToUInt16(_len, 0);
                    lay.Parts.Add(new MuzipFilePart() { Module = _module, PartLength = __len });
                    j++;
                }
                int ret_i = 0;
                while (i < _partCount)
                {
                    MuzipFilePart _part = lay.Parts[i];
                    int part_len = _part.PartLength;
                    byte[] part = new byte[part_len];
                    j = 0;
                    while (j < part_len)
                    {
                        part[j] = (byte)mem.ReadByte();
                        j++;
                    }
                    parts[i] = MuzipModules.UnMuzip(part, _part.Module);
                    total_len += parts[i].Length;
                    e.Progress = i; OnUnCompressingProgress(e);
                    i++;
                }
                ret = new byte[total_len]; i = 0;
                while (i < _partCount)
                {
                    byte[] __part = parts[i];
                    Array.Copy(__part, 0, ret, ret_i, __part.Length); ret_i += __part.Length;
                    i++;
                }
                mem = new MemoryStream(ret);
            }
            return ret;
        }

        public MemoryStream CompressFile(int block)
        {
            if (!File.Exists(FilePath)) return null;
            FileStream fs = new FileStream(FilePath, FileMode.Open);
            MemoryStream mem = new MemoryStream();
            fs.CopyTo(mem);
            fs.Flush();
            fs.Close();
            mem.Position = 0;
            long len = mem.Length, comp_len;
            byte[][] parts = SplitParts(mem, block);
            byte lay_i = 1; CompressingEventArgs e = new CompressingEventArgs();
            while (lay_i <= MaxLayer)
            {
                MuzipLayerData lay = new MuzipLayerData();
                lay.PartCalculated += lay_PartCalculated;
                comp_len = lay.PrepareParts(parts); lay.Layer = lay_i; e.Layer = lay_i;
                byte[] ly = lay.GetBytesData();
                if (comp_len > len && lay_i > 1) { break; }
                mem.Flush();
                mem.Close();
                mem = new MemoryStream();
                foreach (byte b in ly) mem.WriteByte(b);
                int i = 0, c = lay.Parts.Count; e.PartsCount = c;
                while (i < c)
                {
                    byte[] cmp = MuzipModules.Muzip(parts[i], lay.Parts[i].Module, lay.Parts[i].MuzipLength);
                    foreach (byte b in cmp) mem.WriteByte(b);
                    e.Progress = i; OnCompressingProgress(e);
                    i++;
                }
                mem.Position = 0;
                len = mem.Length;
                parts = SplitParts(mem, block);
                lay_i++;
            }
            return mem;
        }

        private unsafe byte[][] SplitParts(MemoryStream mem, int splitMethod)
        {
            int i = 0, c = (int)mem.Length, l = 512, p, j;
            while (i < splitMethod) { l *= 2; i++; }
            i = c; p = (c % l == 0) ? c / l : (c - c % l) / l + 1;
            byte[] bytesMem = mem.GetBuffer();
            byte[][] ret = new byte[p][]; p = 0;
            fixed (byte* _ptrMem = bytesMem)
            {
                byte* ptrMem = _ptrMem;
                while (i > 0)
                {
                    byte[] part = new byte[i > l ? l : i];
                    fixed (byte* _ptrPart = part)
                    {
                        byte* ptrPart = _ptrPart;
                        j = 0;
                        while (j < part.Length)
                        {
                            *ptrPart = *ptrMem; ptrMem++;
                            j++; ptrPart++;
                        }
                        ret[p] = part;
                        p++;
                        i -= l;
                    }
                }
            }
            return ret;
        }

        public MemoryStream UnCompressFile()
        {
            if (!File.Exists(FilePath)) return null;
            FileStream fs = new FileStream(FilePath, FileMode.Open);
            MemoryStream mem = new MemoryStream();
            fs.CopyTo(mem); fs.Flush(); fs.Close();
            bool willEnd = true;
            while (willEnd)
            {
                mem.Position = 0;
                byte layerCount = (byte)mem.ReadByte(); willEnd = layerCount != 1;
                byte[] partCount = new byte[4]; byte[] ret = null;
                partCount[0] = (byte)mem.ReadByte(); partCount[1] = (byte)mem.ReadByte();
                partCount[2] = (byte)mem.ReadByte(); partCount[3] = (byte)mem.ReadByte();
                int _partCount = BitConverter.ToInt32(partCount, 0);
                MuzipLayerData lay = new MuzipLayerData();
                lay.Layer = layerCount;
                byte[][] parts = new byte[_partCount][];
                int i = 0, j = 0, total_len = 0;
                while (j < _partCount)
                {
                    byte _module = (byte)mem.ReadByte();
                    byte[] _len = new byte[2];
                    _len[0] = (byte)mem.ReadByte();
                    _len[1] = (byte)mem.ReadByte();
                    ushort __len = BitConverter.ToUInt16(_len, 0);
                    lay.Parts.Add(new MuzipFilePart() { Module = _module, PartLength = __len });
                    j++;
                }
                int ret_i = 0;
                while (i < _partCount)
                {
                    MuzipFilePart _part = lay.Parts[i];
                    int part_len = _part.PartLength;
                    byte[] part = new byte[part_len];
                    j = 0;
                    while (j < part_len)
                    {
                        part[j] = (byte)mem.ReadByte();
                        j++;
                    }
                    parts[i] = MuzipModules.UnMuzip(part, _part.Module);
                    total_len += parts[i].Length;
                    i++;
                }
                ret = new byte[total_len]; i = 0;
                while (i < _partCount)
                {
                    byte[] __part = parts[i];
                    Array.Copy(__part, 0, ret, ret_i, __part.Length); ret_i += __part.Length;
                    i++;
                }
                mem = new MemoryStream(ret);
            }
            return mem;
        }
    }

    class MuzipFilePart
    {
        public MuzipFilePart() { Module = 255; }
        public byte Module { get; set; }
        public ushort PartLength { get; set; }
        public ushort MuzipLength { get; set; }

        public void CalcBestMuzipModule(byte[] _bytes)
        {
            PartLength = (ushort)_bytes.Length;
            byte[] methods = CompressSizes.CompressMethods();
            int i = 0, m = methods.Length, min_size;
            Module = methods[i];
            min_size = CompressSizes.CompressSizeOfData(_bytes, Module, _bytes.Length);
            MuzipLength = (ushort)min_size;
            while (i < m)
            {
                int _size = CompressSizes.CompressSizeOfData(_bytes, methods[i], min_size);
                if (_size != -1 && _size < min_size)
                {
                    Module = methods[i]; min_size = _size;
                    MuzipLength = (ushort)min_size;
                }
                i++;
            }
        }
    }

    public class PreparePartsEventArgs : EventArgs
    {
        public int Progress { get; set; }
        public int PartsCount { get; set; }
    }

    public delegate void PreparePartsEventHandler(object sender, PreparePartsEventArgs e);

    class MuzipLayerData
    {
        public MuzipLayerData() { Parts = new List<MuzipFilePart>(); }
        public byte Layer { get; set; }
        public List<MuzipFilePart> Parts { get; set; }

        public event PreparePartsEventHandler PartCalculated;

        public void OnPartCalculated(PreparePartsEventArgs e)
        {
            PreparePartsEventHandler handler = PartCalculated;
            if (handler != null) handler(this, e);
        }

        public long PrepareParts(byte[][] _parts)
        {
            long tot = 5L;
            int i = 0, c = _parts.Length;
            PreparePartsEventArgs e = new PreparePartsEventArgs(); e.PartsCount = c;
            while (i < c)
            {
                MuzipFilePart part = new MuzipFilePart();
                part.CalcBestMuzipModule(_parts[i]);
                tot += part.MuzipLength + 3;
                Parts.Add(part);
                e.Progress = i; OnPartCalculated(e);
                i++;
            }
            return tot;
        }

        public byte[] GetBytesData()
        {
            byte[] ret = new byte[Parts.Count * 3 + 5], len;
            ret[0] = Layer; int i = 0, c = Parts.Count;
            byte[] _count = BitConverter.GetBytes(c);
            ret[1] = _count[0]; ret[2] = _count[1]; ret[3] = _count[2]; ret[4] = _count[3];
            while (i < c)
            {
                ret[i * 3 + 5] = Parts[i].Module;
                len = BitConverter.GetBytes(Parts[i].MuzipLength);
                ret[i * 3 + 6] = len[0];
                ret[i * 3 + 7] = len[1];
                i++;
            }
            return ret;
        }
    }
}
