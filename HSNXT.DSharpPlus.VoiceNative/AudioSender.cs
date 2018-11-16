using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HSNXT.DSharpPlus.VoiceNative
{
    internal class AudioSender
    {
        private const ulong DEFAULT_BUFFER_DURATION = 400;
        private const ulong PACKET_INTERVAL = 20;
        private const int MAXIMUM_PACKET_SIZE = 4096;

        private ulong identifierCounter = 0;

        private readonly ulong bufferDuration;
        private readonly object locker = new object();
        private volatile UdpQueue queue;

        public bool Initialized => queue != null;

        public AudioSender() : this(DEFAULT_BUFFER_DURATION)
        {
        }

        public AudioSender(ulong bufferDuration)
        {
            this.bufferDuration = bufferDuration;
        }

        internal void Init()
        {
            queue = new UdpQueue(bufferDuration / PACKET_INTERVAL, (long)PACKET_INTERVAL * 1_000_000L, MAXIMUM_PACKET_SIZE);

            var thread = new Thread(queue.Process)
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true
            };
            thread.Start();
        }

        public ulong GetUniqueIdentifier()
        {
            lock (locker)
            {
                return identifierCounter++;
            }
        }

        internal async Task Send(string hostAddress, int port, ulong queueKey, byte[] data, ulong length)
        {
            // this looks bad but a task completion source would probably do the same under the hood (i think)
            // GetRemainingCapacity returns queue_buffer_capacity when queue is null
            while (queue.GetRemainingCapacity(queueKey) == 0)
            {
                Console.WriteLine("the queue is full, waiting");
                await Task.Delay(10);
            }

            queue.QueuePacket(queueKey, hostAddress, port, data, length);
        }
    }
}
