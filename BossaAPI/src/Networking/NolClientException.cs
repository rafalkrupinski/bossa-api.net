using System;
using System.Runtime.Serialization;

namespace pjank.BossaAPI
{
    public class NolClientException : Exception
    {
        public NolClientException() {}
        public NolClientException(string message) : base(message) {}
        public NolClientException(string message, Exception inner) : base(message,inner) {}
 
        protected NolClientException(SerializationInfo info, StreamingContext context) : base(info,context) {}
    }
}
