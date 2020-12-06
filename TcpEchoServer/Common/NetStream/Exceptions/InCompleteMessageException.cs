using System;

namespace Common.NetStream
{
    internal class InCompleteMessageException : Exception
    {
        public InCompleteMessageException() { }
        public InCompleteMessageException(string message) : base(message) { }
    }
}
