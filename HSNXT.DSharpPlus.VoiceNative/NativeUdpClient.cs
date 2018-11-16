#define HAS_NAT_TRAVERSAL

using System;
using System.Collections.Generic;
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
        public override int DataAvailable => 0;

        private readonly UdpClient _client;
        private ConnectionEndpoint _endpoint;
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
            _endpoint = endpoint;
        }

        public override Task SendAsync(byte[] data, int dataLength)
            => _audioSender.Send(_endpoint.Hostname, _endpoint.Port, _key, data, (ulong) dataLength);

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
