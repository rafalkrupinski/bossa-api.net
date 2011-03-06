using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	/// <summary>
	/// Klasa bazowa dla wszystkich (wysyłanych/odbieranych) komunikatów FIXML.
	/// </summary>
	public class FixmlMsg
	{

		public static string DebugRecvCategory = "fixml-recv";
		public static string DebugSendCategory = "fixml-send";
		public static string DebugCategory = "fixml";
		public static BooleanSwitch DebugOriginalXml = new BooleanSwitch("fixmlOriginalXml",
			"FIXML message debugging - print original (singleline) message string");
		public static BooleanSwitch DebugFormattedXml = new BooleanSwitch("fixmlFormattedXml",
			"FIXML message debugging - print formatted (multiline) XML message");
		public static BooleanSwitch DebugParsedMessage = new BooleanSwitch("fixmlParsedMessage",
			"FIXML message debugging - print parsed message summary");
		public static BooleanSwitch DebugInternals = new BooleanSwitch("fixmlInternals",
			"FixmlMsg class debugging - verbose info about the class internal opartions");

		// reprezentuje cały dokument XML
		protected XmlDocument xmlDoc;
		// pomocnicza referencja do głównego elementu komunikatu XML (pomijając sam element <FIXML/>)
		protected XmlElement xml;

		/// <summary>
		/// Bezpośredni dostęp do komunikatu w formie drzewa XML (zwraca pierwszy element *wewnątrz* głównego "FIXML").
		/// Tylko do zastosowań "specjalnych" - zwykle powinny nam wystarczyć właściwości udostępniane przez klasy pochodne, 
		/// stworzone do konkretnych rodzajów komunikatów. 
		/// </summary>
		public XmlElement Xml
		{
			get { if (xml == null) PrepareXmlMessage(null); return xml; }
		}

		/// <summary>
		/// Konstruktor używany dla komunikatów wychodzących.
		/// </summary>
		public FixmlMsg()
		{
			// obiekty xml tworzymy dopiero w "PrepareXmlMessage"...
			// (w klasach pochodnych zwykle najpierw ustalamy parametry, a potem -
			// dopiero przed wysyłką komunikatu - budowana jest treść komunikatu XML)
		}

		// Przygotowanie treści XML przed wysyłką komunikatu
		// (tutaj tylko bazowe <FIXML/>, reszta w klasach pochodnych).
		protected virtual void PrepareXmlMessage(string name)
		{
			Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + ".PrepareXml...", DebugCategory);
			xmlDoc = new XmlDocument();
			xmlDoc.AppendChild(xmlDoc.CreateElement("FIXML"));
			xmlDoc.DocumentElement.SetAttribute("v", "5.0");
			xmlDoc.DocumentElement.SetAttribute("r", "20080317");
			xmlDoc.DocumentElement.SetAttribute("s", "20080314");
			if (name == null) throw new ArgumentNullException("name", "FIXML message name cannot be null");
			xml = AddElement(xmlDoc.DocumentElement, name);
		}

		/// <summary>
		/// Konstruktor używany dla komunikatów przychodzących.
		/// </summary>
		/// <param name="socket">Socket, z którego chcemy odebrać komunikat.</param>
		public FixmlMsg(Socket socket)
		{
			Debug.WriteLineIf(DebugInternals.Enabled, string.Format("new {0} from socket", GetType().Name), DebugCategory);
			string text = Receive(socket);
			Debug.WriteLineIf(DebugOriginalXml.Enabled, "'" + text + "'", DebugRecvCategory);
			if (text == "") throw new FixmlException("Received empty message!");
			xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(text);
			Debug.WriteLineIf(DebugFormattedXml.Enabled, "\n" + xmlDoc.FormattedXml(), DebugRecvCategory);
			ParseXmlMessage(null);
			Debug.WriteLineIf(DebugParsedMessage.Enabled && (GetType() != typeof(FixmlMsg)), this.ToString(), DebugRecvCategory);
			Debug.WriteLineIf(DebugInternals.Enabled, string.Format("new {0} ok", GetType().Name), DebugCategory);
		}

		/// <summary>
		/// Funkcja zamieniająca komunikat odebrany z socketa (klasy bazowej FixmlMsg)
		/// na konkretną klasę pochodną FixmlMsg - odpowiednią dla typu danego komunikatu.
		/// Obecnie przystosowane tylko dla komunikatów nadsyłanych w kanale asynchronicznym
		/// (raczej tylko tam ma to sens, normalnie wiemy jakiego komunikatu się spodziewać).
		/// </summary>
		/// <param name="msg">Obiekt klasy bazowej FixmlMsg</param>
		/// <returns>Obiekt konkretnej klasy pochodnej FixmlMsg, zależnej od typu komunikatu.
		/// Jeśli trafi na nieznany komunikat, zwraca ten sam obiekt bazowy FixmlMsg.
		/// </returns>
		public static FixmlMsg GetParsedMsg(FixmlMsg msg)
		{
			if (msg.GetType() != typeof(FixmlMsg))
				throw new ArgumentException("Base 'FixmlMsg' class object required", "msg");
			switch (msg.Xml.Name)
			{
				case AppMessageReportMsg.MsgName: return new AppMessageReportMsg(msg);
				case ExecutionReportMsg.MsgName: return new ExecutionReportMsg(msg);
				case MarketDataIncRefreshMsg.MsgName: return new MarketDataIncRefreshMsg(msg);
				case TradingSessionStatusMsg.MsgName: return new TradingSessionStatusMsg(msg);
				case NewsMsg.MsgName: return new NewsMsg(msg);
				case StatementMsg.MsgName: return new StatementMsg(msg);
				case UserResponseMsg.MsgName: return new UserResponseMsg(msg);
				case "Heartbeat": return msg;  // <- dla tego szkoda oddzielnej klasy ;-)
				default:
					string txt = string.Format("Unexpected async message '{0}'", msg.Xml.Name);
					MyUtil.PrintWarning(txt);
					if (!FixmlMsg.DebugOriginalXml.Enabled && !FixmlMsg.DebugFormattedXml.Enabled)
						Trace.WriteLine(string.Format("'{0}'", MyUtil.FormattedXml(msg.Xml.OwnerDocument)));
					return msg;
			}
		}

		// Konstruktor używany wewnętrznie dla komunikatów przychodzących - 
		// "opakowywuje" odebrany komunikat w inną, bardziej precyzyjną klasę pochodną. 
		protected FixmlMsg(FixmlMsg msg)
		{
			Debug.WriteLineIf(DebugInternals.Enabled, string.Format("new {0} from FixmlMsg", GetType().Name), DebugCategory);
			xmlDoc = msg.xmlDoc;
			ParseXmlMessage(null);
			Debug.WriteLineIf(DebugParsedMessage.Enabled, this.ToString(), DebugRecvCategory);
			Debug.WriteLineIf(DebugInternals.Enabled, string.Format("new {0} ok", GetType().Name), DebugCategory);
		}

		// Analiza treści XML odebranego komunikatu
		// (tutaj tylko bazowe <FIXML> i ewentualnie nazwa komunikatu, reszta w klasach pochodnych).
		protected virtual void ParseXmlMessage(string name)
		{
			Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + ".ParseXml...", DebugCategory);
			if (xmlDoc.DocumentElement == null) throw new FixmlException("XML data missing!");
			if (xmlDoc.DocumentElement.Name != "FIXML") throw new FixmlException("FIXML root element missing!");
			if (xmlDoc.DocumentElement.GetAttribute("v") != "5.0") throw new FixmlException("Unknown FIXML protocol version!");
			xml = xmlDoc.DocumentElement.FirstChild as XmlElement;
			if (xml == null) throw new FixmlException("Empty FIXML message!");
			if ((name != null) && (xml.Name != name))
			{
				if ((xml.Name == BizMessageRejectMsg.MsgName) && !(this is BizMessageRejectMsg))
					throw new BizMessageRejectException(new BizMessageRejectMsg(this));
				if ((xml.Name == MarketDataReqRejectMsg.MsgName) && !(this is MarketDataReqRejectMsg))
					throw new FixmlErrorMsgException(new MarketDataReqRejectMsg(this));
				throw new FixmlException("Unexpected FIXML message name: " + xml.Name);
			}
		}

		/// <summary>
		/// Wysyłka niniejszego komunikatu do serwera.
		/// </summary>
		/// <param name="socket">Socket, przez który chcemy wysłać komunikat.</param>
		public void Send(Socket socket)
		{
			Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + ".Send...", DebugCategory);
			if (xmlDoc == null) PrepareXmlMessage(null);
			Debug.WriteLineIf(DebugParsedMessage.Enabled, this.ToString(), DebugSendCategory);
			Debug.WriteLineIf(DebugFormattedXml.Enabled, "\n" + xmlDoc.FormattedXml(), DebugSendCategory);
			string text = xmlDoc.InnerXml + '\0';   // NOL3 czasem głupieje, gdy zabraknie terminatora
			byte[] data_buf = Encoding.ASCII.GetBytes(text);
			byte[] size_buf = BitConverter.GetBytes(data_buf.Length);
			if (socket.Send(size_buf) < size_buf.Length) throw new FixmlSocketException();
			if (socket.Send(data_buf) < data_buf.Length) throw new FixmlSocketException();
			Debug.WriteLineIf(DebugOriginalXml.Enabled, "'" + xmlDoc.InnerXml + "'", DebugSendCategory);
		}

		// Odbiór treści kolejnego komunikatu od serwera.
		private string Receive(Socket socket)
		{
			Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + ".Receive...", DebugCategory);
			byte[] size_buf = new byte[4];
			if (socket.Receive(size_buf) < size_buf.Length) throw new FixmlSocketException();
			byte[] data_buf = new byte[BitConverter.ToInt32(size_buf, 0)];
			if (socket.Receive(data_buf) < data_buf.Length) throw new FixmlSocketException();
			string text = Encoding.Default.GetString(data_buf);
			return text.TrimEnd('\0');   // znak '\0' na końcu nam by tylko przeszkadzał (np. w okienku Output)
		}

		// Zwraca krótką informację opisującą dany komunikat.
		// W przypadku bazowej klasy FixmlMsg jest to sama nazwa konkretnego komunikatu.
		// W klasach pochodnych może to być rozwinięte o wybrane atrybuty komunikatu.
		// Zastosowana m.in. dla komunikatów błędów, to jest przekazywane jako treść wyjątku.
		public override string ToString()
		{
			return string.Format("[{0}]", Xml.Name);
		}


		protected XmlElement AddElement(string name)
		{
			return AddElement(xml, name);
		}

		protected XmlElement AddElement(XmlElement parent, string name)
		{
			XmlElement elem = xmlDoc.CreateElement(name);
			parent.AppendChild(elem);
			return elem;
		}
	}
}
