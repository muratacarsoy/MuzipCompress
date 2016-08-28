using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MuzipCompress
{
    class Huffman
    {
        public static int MuzipSize(byte[] _bytes)
        {
            Tree tree = new Tree();
            tree.GiveData(_bytes);
            int treeBytes = tree.Root.TreeEncodeBitsCount() / 8 + 1,
                zippedBytes = tree.Root.zippedBitsCount() / 8 + 1;
            return treeBytes + zippedBytes + 6;
        }

        public static byte[] MuzipHuffman(byte[] _bytes)
        {
            byte[] ret = null;
            Tree tree = new Tree();
            tree.GiveData(_bytes);
            byte[] zippedTree = tree.Root.TreeEncode();
            byte[] zippedHuffman = new byte[tree.Root.zippedBitsCount() / 8 + 1];
            MuzipBitArray muzipBits = new MuzipBitArray(zippedHuffman);
            Dictionary<byte, Node> table = tree.Root.nodeTable();
            int i = 0, c = _bytes.Length;
            while (i < c)
            {
                byte _byte = _bytes[i]; Node _tableItem = table[_byte];
                foreach (byte item in _tableItem.Bits) { muzipBits.WriteBits(item, 1); }
                i++;
            }
            ushort len = (ushort)zippedTree.Length; int weight = tree.Root.Weight;
            byte[] _len = BitConverter.GetBytes(len); byte[] _weight = BitConverter.GetBytes(weight);
            ret = new byte[zippedTree.Length + zippedHuffman.Length + 6];
            Array.Copy(_len, ret, 2);
            Array.Copy(zippedTree, 0, ret, 2, zippedTree.Length);
            Array.Copy(_weight, 0, ret, zippedTree.Length + 2, 4);
            Array.Copy(zippedHuffman, 0, ret, zippedTree.Length + 6, zippedHuffman.Length);
            return ret;
        }

        public static byte[] UnzipHuffman(byte[] _bytes)
        {
            byte[] ret = null; Collection<byte> _ret = new Collection<byte>();
            Tree tree = new Tree();
            byte[] _treeBytes = null, _huffBytes = null, _len = null, _weight = null;
            ushort len; _len = new byte[2];
            Array.Copy(_bytes, _len, 2); len = BitConverter.ToUInt16(_len, 0);
            _treeBytes = new byte[(int)len];
            Array.Copy(_bytes, 2, _treeBytes, 0, _treeBytes.Length);
            int weight = 0; _weight = new byte[4];
            Array.Copy(_bytes, _treeBytes.Length + 2, _weight, 0, 4);
            weight = BitConverter.ToInt32(_weight, 0);
            int i = _treeBytes.Length + 6, c = _bytes.Length;
            _huffBytes = new byte[c - i];
            Array.Copy(_bytes, i, _huffBytes, 0, _huffBytes.Length);
            tree.TakeData(_treeBytes);
            i = 0; Node node = tree.Root; c = 8;
            List<bool> listBl = new List<bool>(); bool _break = false;
            foreach (byte _b in _huffBytes)
            {
                i = 7;
                while (i >= 0)
                {
                    if (!node.isLeaf())
                    {
                        bool bit = (byte)(_b & (1 << i)) != 0; i--;
                        if (bit) { node = node.Right; } else { node = node.Left; }
                    }
                    if (node.isLeaf())
                    {
                        _ret.Add(node.Value); node = tree.Root; _break = _ret.Count == weight;
                        if (_break) break;
                    }
                }
                if (_break) break;
            }
            i = 0; c = _ret.Count; ret = new byte[c];
            while (i < c) { ret[i] = _ret[i]; i++; }
            return ret;
        }

        private class Node
        {
            public Node() { Bits = new Collection<byte>(); }
            public Collection<byte> Bits { get; set; }
            public byte Value { get; set; }
            public int Weight { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public void CalcBits(Collection<byte> _bits)
            {
                foreach (byte _b in _bits) { Bits.Insert(Bits.Count - 1, _b); }
                if (Left != null) { Left.Bits.Add(0); Left.CalcBits(Bits); }
                if (Right != null) { Right.Bits.Add(1); Right.CalcBits(Bits); }
            }

            public bool isLeaf() { return Left == null && Right == null; }
            protected byte[] _TreeEncode()
            {
                byte[] ret = null;
                if (isLeaf()) { ret = new byte[2]; ret[0] = 1; ret[1] = Value; }
                else
                {
                    byte[] subRetLeft = Left._TreeEncode(), subRetRight = Right._TreeEncode();
                    int c = subRetLeft.Length + subRetRight.Length + 1;
                    ret = new byte[c];
                    ret[0] = 0;
                    subRetLeft.CopyTo(ret, 1); subRetRight.CopyTo(ret, 1 + subRetLeft.Length);
                }
                return ret;
            }

            public byte[] TreeEncode()
            {
                byte[] ret = null;
                byte[] bytes = _TreeEncode();
                MuzipBitArray bits = new MuzipBitArray(new byte[512]);
                int i = 0, c = bytes.Length; byte _byte = 0;
                while (i < c)
                {
                    _byte = bytes[i]; bits.WriteBits(_byte, 1);
                    if (_byte == 1) { i++; _byte = bytes[i]; bits.WriteBits(_byte, 8); }
                    i++;
                }
                int cutBytes = 511 - bits.Pointer / 8;
                bits.CutBytes(cutBytes);
                ret = bits.Data;
                return ret;
            }
            public int TreeEncodeBitsCount()
            {
                int ret = 0;
                if (isLeaf()) { ret = 9; }
                else { ret += Left.TreeEncodeBitsCount() + Right.TreeEncodeBitsCount() + 1; }
                return ret;
            }

            public int TreeDecode(byte[] _bytes, int start)
            {
                int ret = 0, right = 0;
                Left = new Node();
                if (_bytes[start] == 0) { right = Left.TreeDecode(_bytes, start + 1); }
                else { Left.Value = _bytes[start + 1]; right = start + 2; }
                Right = new Node(); ret += right;
                if (_bytes[right] == 0) { ret = Right.TreeDecode(_bytes, right + 1); }
                else { Right.Value = _bytes[right + 1]; ret = right + 2; }
                return ret;
            }
            public int zippedBitsCount()
            {
                int ret = 0;
                if (isLeaf()) ret = Weight * Bits.Count;
                else { ret += Left.zippedBitsCount() + Right.zippedBitsCount(); }
                return ret;
            }
            public Dictionary<byte, Node> nodeTable()
            {
                Dictionary<byte, Node> ret = new Dictionary<byte, Node>();
                if (Left != null)
                {
                    Dictionary<byte, Node> leftTable = Left.nodeTable();
                    foreach (KeyValuePair<byte, Node> item in leftTable) { ret.Add(item.Key, item.Value); }
                }
                if (Right != null)
                {
                    Dictionary<byte, Node> rightTable = Right.nodeTable();
                    foreach (KeyValuePair<byte, Node> item in rightTable) { ret.Add(item.Key, item.Value); }
                }
                if (isLeaf()) { ret.Add(this.Value, this); }
                return ret;
            }
        }

        private class Tree
        {
            public Node Root { get; set; }
            public void GiveData(byte[] _bytes)
            {
                Dictionary<byte, int> WeightTable = new Dictionary<byte, int>();
                List<Node> nodes = new List<Node>();
                int i = 0, c = _bytes.Length; byte _byte;
                while (i < c)
                {
                    _byte = _bytes[i];
                    if (!WeightTable.ContainsKey(_byte)) { WeightTable.Add(_byte, 0); }
                    WeightTable[_byte]++; i++;
                }
                foreach (KeyValuePair<byte, int> val in WeightTable)
                {
                    nodes.Add(new Node() { Value = val.Key, Weight = val.Value });
                }
                while (nodes.Count > 1)
                {
                    List<Node> sorted = nodes.OrderBy(node => node.Weight).ToList<Node>();
                    if (sorted.Count > 1)
                    {
                        Node parent = new Node(); Node left = sorted[0], right = sorted[1];
                        parent.Weight = left.Weight + right.Weight; parent.Left = left; parent.Right = right;
                        nodes.Remove(left); nodes.Remove(right); nodes.Add(parent);
                    }
                }
                byte[] bitBytes = new byte[64];
                Root = nodes.FirstOrDefault(); Root.CalcBits(new Collection<byte>()); WeightTable = null;
            }

            public void TakeData(byte[] _bytes)
            {
                Collection<byte> openedBytes = new Collection<byte>();
                MuzipBitArray bits = new MuzipBitArray(_bytes);
                while (!bits.EndOfArray)
                {
                    byte _b = bits.ReadBits(1); openedBytes.Add(_b);
                    if (_b == 1) { openedBytes.Add(bits.ReadBits(8)); }
                }
                int i = 0, c = openedBytes.Count;
                byte[] _openedBytes = new byte[c];
                while (i < c) { _openedBytes[i] = openedBytes[i]; i++; }
                Root = new Node();
                if (_openedBytes[0] == 0)
                {
                    Root.TreeDecode(_openedBytes, 1);
                }
                else
                {
                    Root.Value = _openedBytes[1];
                }
                Root.CalcBits(new Collection<byte>()); openedBytes = null;
            }
        }
    }
}
