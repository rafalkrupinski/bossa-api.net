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
            Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + " - preparing message", DebugCategory);
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
            Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + " constructor (from socket)", DebugCategory);
            string text = Receive(socket);
            Debug.WriteLineIf(DebugOriginalXml.Enabled, "'" + text + "'", DebugRecvCategory);
            if (text == "") throw new FixmlException("Received empty message!");
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(text);
            Debug.WriteLineIf(DebugFormattedXml.Enabled, "\n" + xmlDoc.FormattedXml(), DebugRecvCategory);
            ParseXmlMessage(null);
            Debug.WriteLineIf(DebugParsedMessage.Enabled && (GetType() != typeof(FixmlMsg)), this.ToString(), DebugRecvCategory);
        }

        // Konstruktor używany wewnętrznie dla komunikatów przychodzących - 
        // "opakowywuje" odebrany komunikat w inną, bardziej precyzyjną klasę pochodną. 
        protected FixmlMsg(FixmlMsg msg)
        {
            Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + " constructor (msg copy)", DebugCategory);
            xmlDoc = msg.xmlDoc;
            ParseXmlMessage(null);
            Debug.WriteLineIf(DebugParsedMessage.Enabled, this.ToString(), DebugRecvCategory);
        }

        // Analiza treści XML odebranego komunikatu
        // (tutaj tylko bazowe <FIXML> i ewentualnie nazwa komunikatu, reszta w klasach pochodnych).
        protected virtual void ParseXmlMessage(string name)
        {
            Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + " - parsing message", DebugCategory);
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
            Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + " - sending message...", DebugCategory);
            if (xmlDoc == null) PrepareXmlMessage(null);
            Debug.WriteLineIf(DebugParsedMessage.Enabled, this.ToString(), DebugSendCategory);
            Debug.WriteLineIf(DebugFormattedXml.Enabled, "\n" + xmlDoc.FormattedXml(), DebugSendCategory);
            string text = xmlDoc.InnerXml +'\0';   // NOL3 czasem głupieje, gdy zabraknie terminatora
            byte[] data_buf = Encoding.ASCII.GetBytes(text);
            byte[] size_buf = BitConverter.GetBytes(data_buf.Length);
            if (socket.Send(size_buf) < size_buf.Length) throw new FixmlSocketException();
            if (socket.Send(data_buf) < data_buf.Length) throw new FixmlSocketException();
            Debug.WriteLineIf(DebugOriginalXml.Enabled, "'" + text + "'", DebugSendCategory);
            Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + " - message sent", DebugCategory);
        }

        // Odbiór treści kolejnego komunikatu od serwera.
        private string Receive(Socket socket)
        {
            Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + " - receiving message...", DebugCategory);
            byte[] size_buf = new byte[4];
            if (socket.Receive(size_buf) < size_buf.Length) throw new FixmlSocketException();
            byte[] data_buf = new byte[BitConverter.ToInt32(size_buf, 0)];
            if (socket.Receive(data_buf) < data_buf.Length) throw new FixmlSocketException();
            string text = Encoding.Default.GetString(data_buf);
            Debug.WriteLineIf(DebugInternals.Enabled, GetType().Name + " - message received", DebugCategory);
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
