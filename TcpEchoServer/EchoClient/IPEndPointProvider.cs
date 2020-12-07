using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace EchoClient
{
    public class IPEndPointProvider
    {
        private readonly List<IPEndPoint> _availableIpAddresses;
        private int _currentIndex; 
        

        public IPEndPointProvider(List<IPEndPoint> availableIpAddresses)
        {
            _availableIpAddresses = availableIpAddresses.OrderBy(x => Guid.NewGuid()).ToList();
            _currentIndex = 0;
        }

        public IPEndPoint GetNewAddress()
        {
            int nextIndex = _currentIndex % _availableIpAddresses.Count;
            IPEndPoint newAddress = _availableIpAddresses[nextIndex];
            Interlocked.Increment(ref _currentIndex);

            return newAddress;
        }
    }
}
