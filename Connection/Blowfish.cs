using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Server
{
    public class GameCrypto
    {
        Blowfish _blowfish;
        public GameCrypto(byte[] key)
        {
            _blowfish = new Blowfish(BlowfishAlgorithm.CFB64);
            _blowfish.SetKey(key);
        }
        public void Decrypt(byte[] packet)
        {
            byte[] buffer = _blowfish.Decrypt(packet);
            System.Buffer.BlockCopy(buffer, 0, packet, 0, buffer.Length);
        }
        public void Encrypt(byte[] packet)
        {
            byte[] buffer = _blowfish.Encrypt(packet);
            System.Buffer.BlockCopy(buffer, 0, packet, 0, buffer.Length);
        }
        public byte[] Decrypts(byte[] packet)
        {
            byte[] buffer = _blowfish.Decrypt(packet);
            System.Buffer.BlockCopy(buffer, 0, packet, 0, buffer.Length);
            return buffer;
        }
        public byte[] Encrypts(byte[] packet)
        {
            byte[] buffer = _blowfish.Encrypt(packet);
            System.Buffer.BlockCopy(buffer, 0, packet, 0, buffer.Length);
            return buffer;
        }
        public Blowfish Blowfish
        {
            get { return _blowfish; }
        }
    }
    public enum BlowfishAlgorithm
    {
        ECB,
        CBC,
        CFB64,
        OFB64,
    };
    public class Blowfish : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        struct bf_key_st
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
            public UInt32[] P;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public UInt32[] S;
        }
        private BlowfishAlgorithm _algorithm;
        private IntPtr _key;
        private byte[] _encryptIv;
        private byte[] _decryptIv;
        private int _encryptNum;
        private int _decryptNum;
        public Blowfish(BlowfishAlgorithm algorithm)
        {
            _algorithm = algorithm;
            _encryptIv = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            _decryptIv = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            bf_key_st key = new bf_key_st();
            key.P = new UInt32[16 + 2];
            key.S = new UInt32[4 * 256];
            _key = Marshal.AllocHGlobal(key.P.Length * sizeof(UInt32) + key.S.Length * sizeof(UInt32));
            Marshal.StructureToPtr(key, _key, false);
            _encryptNum = 0;
            _decryptNum = 0;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_key);
        }
        public void SetKey(byte[] data)
        {
            _encryptNum = 0;
            _decryptNum = 0;
            Native.BF_set_key(_key, data.Length, data);
        }
        public byte[] Encrypt(byte[] buffer)
        {
            byte[] ret = new byte[buffer.Length];
            switch (_algorithm)
            {
                case BlowfishAlgorithm.ECB:
                    Native.BF_ecb_encrypt(buffer, ret, _key, 1);
                    break;
                case BlowfishAlgorithm.CBC:
                    Native.BF_cbc_encrypt(buffer, ret, buffer.Length, _key, _encryptIv, 1);
                    break;
                case BlowfishAlgorithm.CFB64:
                    Native.BF_cfb64_encrypt(buffer, ret, buffer.Length, _key, _encryptIv, ref _encryptNum, 1);
                    break;
                case BlowfishAlgorithm.OFB64:
                    Native.BF_ofb64_encrypt(buffer, ret, buffer.Length, _key, _encryptIv, out _encryptNum);
                    break;
            }
            return ret;
        }
        public byte[] Decrypt(byte[] buffer)
        {
            byte[] ret = new byte[buffer.Length];
            switch (_algorithm)
            {
                case BlowfishAlgorithm.ECB:
                    Native.BF_ecb_encrypt(buffer, ret, _key, 0);
                    break;
                case BlowfishAlgorithm.CBC:
                    Native.BF_cbc_encrypt(buffer, ret, buffer.Length, _key, _decryptIv, 0);
                    break;
                case BlowfishAlgorithm.CFB64:
                    Native.BF_cfb64_encrypt(buffer, ret, buffer.Length, _key, _decryptIv, ref _decryptNum, 0);
                    break;
                case BlowfishAlgorithm.OFB64:
                    Native.BF_ofb64_encrypt(buffer, ret, buffer.Length, _key, _decryptIv, out _decryptNum);
                    break;
            }
            return ret;
        }
        public byte[] EncryptIV
        {
            get { return _encryptIv; }
            set { System.Buffer.BlockCopy(value, 0, _encryptIv, 0, 8); }
        }
        public byte[] DecryptIV
        {
            get { return _decryptIv; }
            set { System.Buffer.BlockCopy(value, 0, _decryptIv, 0, 8); }
        }
    }
}