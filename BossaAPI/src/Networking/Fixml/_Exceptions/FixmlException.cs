using System;
using System.Runtime.Serialization;

namespace pjank.BossaAPI.Fixml
{
    /// <summary>
    /// Każdy błąd wywodzący się z tej biblioteki (zw. z komunikacją FIXML)
    /// </summary>
    public class FixmlException : Exception
    {
        public FixmlException() { }
        public FixmlException(string message) : base(message) { }
        public FixmlException(string message, Exception inner) : base(message, inner) { }

        protected FixmlException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
