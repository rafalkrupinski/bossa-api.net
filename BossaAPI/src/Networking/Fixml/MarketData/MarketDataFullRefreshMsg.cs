using System.Net.Sockets;

namespace pjank.BossaAPI.Fixml
{
    public class MarketDataFullRefreshMsg : FixmlMsg
    {
        public const string MsgName = "MktDataFull";

        public int RequestId { get; private set; }

        public MarketDataFullRefreshMsg(Socket socket) : base(socket) { }

        protected override void ParseXmlMessage(string name)
        {
            base.ParseXmlMessage(MsgName);
            // NOL3 w razie problemów czasem zwraca tutaj ujemną wartość -1
            RequestId = FixmlUtil.ReadInt(xml, "ReqID");
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}]", Xml.Name, RequestId);
        }

    }
}
