using System;
using System.Runtime.InteropServices;

namespace Server
{
    public unsafe class Native
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* memcpy(void* dest, void* src, uint size);

        [DllImport("winmm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint timeGetTime();

        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void BF_set_key(IntPtr _key, int len, byte[] data);

        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void BF_ecb_encrypt(byte[] in_, byte[] out_, IntPtr schedule, int enc);

        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void BF_cbc_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, int enc);

        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void BF_cfb64_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, ref int num, int enc);

        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void BF_ofb64_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, out int num);
    }
}