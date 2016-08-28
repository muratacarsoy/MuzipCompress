using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MuzipCompress
{
    public class MuzipCompressingFileEventArgs : EventArgs
    {
        public string TargetFile { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public long TotalSize { get; set; }
        public long MuzipSize { get; set; }
    }

    public class MuzipCompressingFinishedEventArgs : EventArgs
    {
        public string TargetFile { get; set; }
        public float CompressRate { get; set; }
    }

    public class MuzipUnCompressingFinishedEventArgs : EventArgs
    {
        public string TargetPath { get; set; }
    }

    public class MuzipUnCompressingFileEventArgs : EventArgs
    {
        public string TargetPath { get; set; }
        public string FileName { get; set; }
        public long TotalMuzipSize { get; set; }
        public long MuzipSize { get; set; }
    }

    public delegate void MuzipCompressingFileEventHandler(object sender, MuzipCompressingFileEventArgs e);
    public delegate void MuzipCompressingFinishedEventHandler(object sender, MuzipCompressingFinishedEventArgs e);
    public delegate void MuzipUnCompressingFinishedEventHandler(object sender, MuzipUnCompressingFinishedEventArgs e);
    public delegate void MuzipUnCompressingFileEventHandler(object sender, MuzipUnCompressingFileEventArgs e);

    public class MuzipSystem
    {
        public string TargetPath { get; set; }
        public string TargetFile { get; set; }
        public int BlockSize { get; set; }
        private List<FileInfo> files;
        private List<DirectoryInfo> directories;
        private List<string> allFilePaths;
        private List<long> allFileSizes;

        public event MuzipCompressingFileEventHandler CompressingFiles;
        public event CompressingEventHandler Compressing;
        public event PreparePartsEventHandler PreparingParts;
        public event MuzipCompressingFinishedEventHandler CompressingFinished;
        public event MuzipUnCompressingFinishedEventHandler UnCompressingFinished;
        public event UnCompressingEventHandler UnCompressing;
        public event MuzipUnCompressingFileEventHandler UnCompressingFiles;

        public void OnCompressingFiles(MuzipCompressingFileEventArgs e)
        {
            MuzipCompressingFileEventHandler handler = CompressingFiles;
            if (handler != null) handler(this, e);
        }

        public void OnCompressing(CompressingEventArgs e)
        {
            CompressingEventHandler handler = Compressing;
            if (handler != null) handler(this, e);
        }

        public void OnPreparingParts(PreparePartsEventArgs e)
        {
            PreparePartsEventHandler handler = PreparingParts;
            if (handler != null) handler(this, e);
        }

        public void OnCompressingFinished(MuzipCompressingFinishedEventArgs e)
        {
            MuzipCompressingFinishedEventHandler handler = CompressingFinished;
            if (handler != null) handler(this, e);
        }

        public void OnUnCompressingFinished(MuzipUnCompressingFinishedEventArgs e)
        {
            MuzipUnCompressingFinishedEventHandler handler = UnCompressingFinished;
            if (handler != null) handler(this, e);
        }

        public void OnUnCompressing(UnCompressingEventArgs e)
        {
            UnCompressingEventHandler handler = UnCompressing;
            if (handler != null) handler(this, e);
        }

        public void OnUnCompressingFiles(MuzipUnCompressingFileEventArgs e)
        {
            MuzipUnCompressingFileEventHandler handler = UnCompressingFiles;
            if (handler != null) handler(this, e);
        }

        public MuzipSystem()
        {
            files = new List<FileInfo>();
            directories = new List<DirectoryInfo>();
            allFilePaths = new List<string>();
            allFileSizes = new List<long>();
        }

        public void AddPath(string path)
        {
            FileInfo pathInfo = new FileInfo(path);
            if (File.Exists(path)) files.Add(new FileInfo(path));
            else if (Directory.Exists(path)) directories.Add(new DirectoryInfo(path));
        }

        private byte[] logDataFile(FileInfo fileInfo)
        {
            char[] name_chars = fileInfo.Name.ToCharArray();
            long creation_time = fileInfo.CreationTime.ToBinary(), last_access_time = fileInfo.LastAccessTime.ToBinary(),
                last_write_time = fileInfo.LastWriteTime.ToBinary();
            bool is_readonly = fileInfo.IsReadOnly;

            byte[] data_name_chars = new byte[name_chars.Length * 2];
            short len_name_chars = (short)name_chars.Length;
            byte[] data_len_name_chars = BitConverter.GetBytes(len_name_chars);
            data_name_chars[0] = data_len_name_chars[0]; data_name_chars[1] = data_len_name_chars[1];
            int i = 0;
            while (i < len_name_chars)
            {
                byte[] data_char = BitConverter.GetBytes(name_chars[i]);
                data_name_chars[2 * i] = data_char[0]; data_name_chars[2 * i] = data_char[0];
                i++;
            }

            byte[] data_datetimes_is_readonly = new byte[26];
            Array.Copy(BitConverter.GetBytes(creation_time), 0, data_datetimes_is_readonly, 0, 8);
            Array.Copy(BitConverter.GetBytes(last_access_time), 0, data_datetimes_is_readonly, 8, 8);
            Array.Copy(BitConverter.GetBytes(last_write_time), 0, data_datetimes_is_readonly, 16, 8);
            Array.Copy(BitConverter.GetBytes(is_readonly), 0, data_datetimes_is_readonly, 24, 2);

            byte[] ret = new byte[data_name_chars.Length + 27];
            ret[0] = 0;

            return ret;
        }

        public void Compress()
        {
            byte[] log = CreateLog();
            int i, c = allFilePaths.Count;
            int[] lengths = new int[c];
            int[] org_lengths = new int[c];
            byte[][] file_parts = new byte[c][];
            MuzipFile forLog = new MuzipFile(); forLog.MaxLayer = 1;
            byte[] compressed_log = forLog._CompressBytes(BlockSize, log);
            FileStream fs = new FileStream(TargetFile, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            int len_compressed_log = compressed_log.Length;
            bw.Write(len_compressed_log);
            bw.Write(compressed_log);
            int len_lengths_array = lengths.Length;
            MuzipCompressingFileEventArgs e = new MuzipCompressingFileEventArgs(); e.TargetFile = TargetFile; e.TotalSize = 0;
            e.MuzipSize = 0; e.FileSize = 0;
            foreach (long size in allFileSizes) e.TotalSize += size;
            i = 0;
            while (i < c)
            {
                string path = allFilePaths[i];
                e.FilePath = path; OnCompressingFiles(e);
                MuzipFile muzipFile = new MuzipFile();
                muzipFile.PreparingPartsProgress += muzipFile_PreparingPartsProgress;
                muzipFile.CompressingProgress += muzipFile_CompressingProgress;
                muzipFile.MaxLayer = 1; muzipFile.FilePath = path;
                file_parts[i] = muzipFile._CompressFile(BlockSize);
                lengths[i] = file_parts[i].Length;
                e.FileSize += allFileSizes[i]; e.MuzipSize += lengths[i];
                i++;
            }

            bw.Write(len_lengths_array);
            i = 0;
            while (i < c) { bw.Write(lengths[i]); i++; }
            i = 0;
            while (i < c) { bw.Write(file_parts[i]); i++; }
            long tot = e.TotalSize, muz = fs.Length;
            fs.Flush(); fs.Close();
            MuzipCompressingFinishedEventArgs e_finished = new MuzipCompressingFinishedEventArgs();
            double _tot = tot, _muz = muz;
            double _rate = 100.0 - (_muz * 100.0 / _tot);
            float rate = (float)_rate;
            e_finished.CompressRate = rate;
            e_finished.TargetFile = TargetFile;
            OnCompressingFinished(e_finished);
        }

        void muzipFile_CompressingProgress(object sender, CompressingEventArgs e)
        {
            OnCompressing(e);
        }

        void muzipFile_PreparingPartsProgress(object sender, PreparePartsEventArgs e)
        {
            OnPreparingParts(e);
        }

        public void UnCompress(string path)
        {
            if (!File.Exists(path)) return;
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            int len_compressed_log = br.ReadInt32();
            byte[] compressed_log = br.ReadBytes(len_compressed_log);
            MuzipFile muzip_log = new MuzipFile();
            byte[] log = muzip_log.UnCompressBytes(compressed_log); ReadLog(log);
            int len_lengths_array = br.ReadInt32();
            int[] lengths = new int[len_lengths_array];
            MuzipUnCompressingFileEventArgs e = new MuzipUnCompressingFileEventArgs(); e.TargetPath = TargetPath;
            int i = 0; e.TotalMuzipSize = 0; e.MuzipSize = 0;
            while (i < len_lengths_array) { lengths[i] = br.ReadInt32(); e.TotalMuzipSize += lengths[i]; i++; }
            byte[][] compressed_parts = new byte[len_lengths_array][];
            i = 0;
            while (i < len_lengths_array)
            {
                byte[] compressed_part = br.ReadBytes(lengths[i]);
                MuzipFile muzip_file = new MuzipFile();
                muzip_file.UnCompressingProgress += muzip_file_UnCompressingProgress;
                FileStream ps = new FileStream(allFilePaths[i], FileMode.Create); e.FileName = allFilePaths[i];
                byte[] file_part = muzip_file.UnCompressBytes(compressed_part);
                ps.Write(file_part, 0, file_part.Length); e.MuzipSize += compressed_part.Length;
                ps.Flush(); ps.Close();
                OnUnCompressingFiles(e);
                i++;
            }
            fs.Flush(); fs.Close();
            MuzipUnCompressingFinishedEventArgs e_finished = new MuzipUnCompressingFinishedEventArgs(); e_finished.TargetPath = TargetPath;
            OnUnCompressingFinished(e_finished);
        }

        void muzip_file_UnCompressingProgress(object sender, UnCompressingEventArgs e)
        {
            OnUnCompressing(e);
        }

        public void ReadLog(byte[] log_bytes)
        {
            MemoryStream mem_log = new MemoryStream(log_bytes);
            BinaryReader reader = new BinaryReader(mem_log);
            int i = 0, count = reader.ReadInt32();
            if (!Directory.Exists(TargetPath)) Directory.CreateDirectory(TargetPath);
            while (i < count)
            {
                byte zero_one = reader.ReadByte();
                if (zero_one == 0) { readLogFile(reader, TargetPath); }
                else { readLogDirectory(reader, TargetPath); }
                i++;
            }
            mem_log.Flush(); mem_log.Close();
        }

        private FileInfo readLogFile(BinaryReader reader, string dir_path)
        {
            string name = reader.ReadString();
            string path = dir_path + "\\" + name;
            DateTime creation, last_access, last_write;
            bool is_readonly;
            creation = DateTime.FromBinary(reader.ReadInt64());
            last_access = DateTime.FromBinary(reader.ReadInt64());
            last_write = DateTime.FromBinary(reader.ReadInt64());
            is_readonly = reader.ReadBoolean();

            FileInfo ret = new FileInfo(path);
            if (!ret.Exists) { FileStream crt = ret.Create(); crt.Flush(); crt.Close(); }
            ret.CreationTime = creation; ret.LastAccessTime = last_access; ret.LastWriteTime = last_write;
            ret.IsReadOnly = is_readonly;
            allFilePaths.Add(path);
            return ret;
        }

        private DirectoryInfo readLogDirectory(BinaryReader reader, string dir_path)
        {
            string name = reader.ReadString();
            string path = dir_path + "\\" + name;
            DateTime creation, last_access, last_write;
            creation = DateTime.FromBinary(reader.ReadInt64());
            last_access = DateTime.FromBinary(reader.ReadInt64());
            last_write = DateTime.FromBinary(reader.ReadInt64());

            DirectoryInfo ret = new DirectoryInfo(path);
            if (!ret.Exists) ret.Create();
            int i = 0, subsCount = reader.ReadInt32();
            while (i < subsCount)
            {
                byte zero_one = reader.ReadByte();
                if (zero_one == 0) { readLogFile(reader, path); }
                else { readLogDirectory(reader, path); }
                i++;
            }
            ret.CreationTime = creation; ret.LastAccessTime = last_access; ret.LastWriteTime = last_write;
            return ret;
        }

        public byte[] CreateLog()
        {
            MemoryStream memLog = new MemoryStream();
            BinaryWriter binWriter = new BinaryWriter(memLog);
            int count = directories.Count + files.Count;
            binWriter.Write(count);
            foreach (DirectoryInfo dir in directories) writeLogDirectory(binWriter, dir);
            foreach (FileInfo file in files) writeLogFile(binWriter, file);
            byte[] ret = new byte[memLog.Length];
            memLog.Position = 0;
            memLog.Read(ret, 0, ret.Length);
            memLog.Flush(); memLog.Close();
            return ret;
        }

        private void writeLogFile(BinaryWriter writer, FileInfo fileInfo)
        {
            byte zero = 0;
            writer.Write(zero); writer.Write(fileInfo.Name);
            writer.Write(fileInfo.CreationTime.ToBinary());
            writer.Write(fileInfo.LastAccessTime.ToBinary());
            writer.Write(fileInfo.LastWriteTime.ToBinary());
            writer.Write(fileInfo.IsReadOnly);
            allFilePaths.Add(fileInfo.FullName);
            allFileSizes.Add(fileInfo.Length);
        }

        private void writeLogDirectory(BinaryWriter writer, DirectoryInfo directoryInfo)
        {
            byte one = 1;
            writer.Write(one); writer.Write(directoryInfo.Name);
            writer.Write(directoryInfo.CreationTime.ToBinary());
            writer.Write(directoryInfo.LastAccessTime.ToBinary());
            writer.Write(directoryInfo.LastWriteTime.ToBinary());
            DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
            FileInfo[] subFiles = directoryInfo.GetFiles();
            writer.Write(subDirectories.Length + subFiles.Length);
            foreach (DirectoryInfo dir in subDirectories) writeLogDirectory(writer, dir);
            foreach (FileInfo file in subFiles) writeLogFile(writer, file);
        }
    }
}
