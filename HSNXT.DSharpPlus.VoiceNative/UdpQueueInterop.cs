using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using size_t = System.UInt64;
using ref_queue_manager_t = System.IntPtr;
// ReSharper disable BuiltInTypeReferenceStyle

namespace HSNXT.DSharpPlus.VoiceNative
{
    /*[SuppressMessage("ReSharper", "InconsistentNaming")]
    internal struct queue_manager_t
    {

    }*/

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class UdpQueueInterop
    {
        [DllImport("hsnxt-nas.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ref_queue_manager_t Create(size_t queue_buffer_capacity, long packet_interval);
        
        [DllImport("hsnxt-nas.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy(ref_queue_manager_t instance);
        
        [DllImport("hsnxt-nas.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t GetRemainingCapacity(ref_queue_manager_t instance, ulong key);

        // LPStr for PCSTR
        [DllImport("hsnxt-nas.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool QueuePacket(ref_queue_manager_t instance, ulong key, [MarshalAs(UnmanagedType.LPStr)] string address_string, int port, byte[] data_buffer, size_t data_length);
        
        [DllImport("hsnxt-nas.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Process(ref_queue_manager_t instance);
    }
}
