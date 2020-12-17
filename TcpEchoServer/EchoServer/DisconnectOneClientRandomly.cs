using Common.NetStream;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EchoServer
{
    public class DisconnectOneClientRandomly
    {
        private readonly int _delayTimeInSeconds;
        private readonly ConcurrentCollection<int, NetStreamHandler> _clients;

        public DisconnectOneClientRandomly(ConcurrentCollection<int, NetStreamHandler> clients, int delayTimeInSeconds)
        {
            _clients = clients;
            _delayTimeInSeconds = delayTimeInSeconds;
            DisconnectClientRandomly(delayInSeconds: _delayTimeInSeconds).SafeFireAndForget();
        }

        private int GetRandomClientId()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            List<int> keys = _clients.GetAllKeys();

            int randIndex = random.Next(0, keys.Count);
            int randId = keys[randIndex];

            return randId;
        }

        private async Task DisconnectClientRandomly(int delayInSeconds)
        {
            await Task.Delay(delayInSeconds * 1000);
            int randomClientId = GetRandomClientId();

            Logger.Info("diconnect client {0} after 3 seconds", randomClientId);
            DisconnectClientAfterSeconds(randomClientId, 3).SafeFireAndForget();
        }

        private async Task DisconnectClientAfterSeconds(int clientId, int seconds)
        {
            await Task.Delay(seconds * 1000);
            NetStreamHandler client = _clients.Remove(clientId);
            client?.Disconnect();
        }

    }
}
