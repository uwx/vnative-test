using System;
using System.Collections.Generic;
using System.Text;

namespace HSNXT.DSharpPlus.VoiceNative
{
    internal class UdpQueue : IDisposable
    {
        private readonly ulong _bufferCapacity;
        private readonly byte[] _packetBuffer;
        private readonly IntPtr _instance;
        private bool _released;

        private readonly object _locker = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bufferCapacity">Maximum number of packets in one queue</param>
        /// <param name="packetInterval">Time interval between packets in a queue</param>
        /// <param name="maximumPacketSize">Maximum packet size</param>
        public UdpQueue(ulong bufferCapacity, long packetInterval, int maximumPacketSize)
        {
            _bufferCapacity = bufferCapacity;
            _packetBuffer = new byte[maximumPacketSize];
            _instance = UdpQueueInterop.Create(bufferCapacity, packetInterval);
        }

        /// <summary>
        /// If the queue does not exist yet, returns the maximum number of packets in a queue.
        /// </summary>
        /// <param name="key">Unique queue identifier</param>
        /// <returns>Number of empty packet slots in the specified queue</returns>
        public ulong GetRemainingCapacity(ulong key)
        {
            lock (_locker)
            {
                return _released ? 0 : UdpQueueInterop.GetRemainingCapacity(_instance, key);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Total capacity used for queues in this manager.</returns>
        public ulong GetCapacity()
        {
            return _bufferCapacity;
        }

        public bool QueuePacket(ulong key, string hostAddress, int port, byte[] data, ulong length)
        {
            lock (_locker)
            {
                if (_released)
                {
                    return false;
                }

                Array.Copy(data, 0, _packetBuffer, 0, (int)length);

                return UdpQueueInterop.QueuePacket(_instance, key, hostAddress, port, _packetBuffer, length);
            }
        }

        public void Process()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            UdpQueueInterop.Process(_instance);
        }

        public void Dispose()
        {
            lock (_locker)
            {
                _released = true;
                UdpQueueInterop.Destroy(_instance);
            }
        }
    }
}
