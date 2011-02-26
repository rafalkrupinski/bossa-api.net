using System;
using System.Runtime.Serialization;

namespace pjank.BossaAPI.Fixml
{
	/// <summary>
	/// Błąd wynikający z przerwania kanału komunikacyjnego w trakcie przesyłania komunikatu.
	/// </summary>
	public class FixmlSocketException : FixmlException
	{
		public FixmlSocketException() : this("Connection terminated") { }
		public FixmlSocketException(string message) : base(message) { }
		public FixmlSocketException(string message, Exception inner) : base(message, inner) { }

		protected FixmlSocketException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
