using System.Net.Sockets;

namespace pjank.BossaAPI.Fixml
{
    public class TradingSessionStatusMsg : FixmlMsg
    {
        public const string MsgName = "TrdgSesStat";

        public uint RequestId { get; private set; }
        public string SessionId { get; private set; }
        public TradingSessionPhase SessionPhase { get; private set; }
        public TradingSessionStatus SessionStatus { get; private set; }
        public TrdSesStatRejectReason RejectReason { get; private set; }
        public FixmlInstrument Instrument { get; private set; }

        public TradingSessionStatusMsg(Socket socket) : base(socket) { }
        public TradingSessionStatusMsg(FixmlMsg msg) : base(msg) { }

        protected override void ParseXmlMessage(string name)
        {
            base.ParseXmlMessage(MsgName);
            RequestId = FixmlUtil.ReadUInt(xml, "ReqID");
            SessionId = FixmlUtil.ReadString(xml, "SesID", true);
            try { SessionPhase = TrdgSesPhaseUtil.Read(xml, "SesSub"); }
            catch (FixmlException e) { e.PrintWarning(); }
            try { SessionStatus = TrdgSesStatusUtil.Read(xml, "Stat"); }
            catch (FixmlException e) { e.PrintWarning(); }
            RejectReason = TrdSesStatRejRsnUtil.Read(xml, "StatRejRsn");
            Instrument = FixmlInstrument.Read(xml, "Instrmt");
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}]" + (Instrument != null ? " {2,-10}  phase = {3}, status = {4} {5}" : ""),
                Xml.Name, RequestId, Instrument, SessionPhase, SessionStatus,
                (SessionStatus == TradingSessionStatus.RequestRejected) ? "(" + RejectReason + ")" : null);
        }

    }
}
