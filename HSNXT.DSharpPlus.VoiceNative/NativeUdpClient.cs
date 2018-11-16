#define HAS_NAT_TRAVERSAL

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Net.Udp;

namespace HSNXT.DSharpPlus.VoiceNative
{
    /// <summary>
    /// Custom VoiceNative UDP backed by hsnxt-nas
    /// </summary>
    internal class NativeUdpClient : BaseUdpClient
    {
        private readonly struct NativeEndpoint
        {
            public NativeEndpoint(string hostname, int port)
            {
                Hostname = hostname;
                Port = port;
                UdpHost = Dns.GetHostAddresses(hostname)[0].ToString(); //TODO async // TODO less stupid dns resolution
            }

            public string Hostname { get; }
            public string UdpHost { get; }
            public int Port { get; }
        }

        public override int DataAvailable => 0;

        private readonly UdpClient _client;
        private NativeEndpoint _endpoint;
        private readonly AudioSender _audioSender;
        private readonly ulong _key;

        public NativeUdpClient(AudioSender audioSender)
        {
            _audioSender = audioSender;
            _key = _audioSender.GetUniqueIdentifier();
            
            _client = new UdpClient();
            // TODO: Solve for .NET Standard, this is possibly default behaviour (???)
#if HAS_NAT_TRAVERSAL
            _client.AllowNatTraversal(true);
#endif
        }

        public override void Setup(ConnectionEndpoint endpoint)
        {
            _endpoint = new NativeEndpoint(endpoint.Hostname, endpoint.Port);
        }
        
        public override Task SendAsync(byte[] data, int dataLength)
            => _client.SendAsync(data, dataLength, _endpoint.Hostname, _endpoint.Port);

        public Task SendNativelyAsync(byte[] data, int dataLength)
            => _audioSender.Send(_endpoint.UdpHost, _endpoint.Port, _key, data, (ulong) dataLength);

        public override async Task<byte[]> ReceiveAsync()
        {
            var result = await _client.ReceiveAsync().ConfigureAwait(false);
            return result.Buffer;
        }

        public override void Close()
        {
            // TODO: Solve later
        }
    }
}
