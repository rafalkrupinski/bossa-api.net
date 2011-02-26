
namespace pjank.BossaAPI.Fixml
{
	public class MarketDataReqRejectMsg : FixmlMsg
	{
		public const string MsgName = "MktDataReqRej";

		public int? RequestId { get; private set; }
		public MDRejectReason RejectReason { get; private set; }
		public string RejectText { get; private set; }

		public MarketDataReqRejectMsg(FixmlMsg m) : base(m) { }

		protected override void ParseXmlMessage(string name)
		{
			base.ParseXmlMessage(MsgName);
			RequestId = FixmlUtil.ReadInt(xml, "ReqID", true);
			RejectReason = MDRejResnUtil.Read(xml, "ReqRejResn");
			RejectText = FixmlUtil.ReadString(xml, "Txt", true);
		}

		public override string ToString()
		{
			return string.Format("[{0}:{1}] {2} {3}",
				Xml.Name, RequestId, RejectReason,
				(RejectText != null ? "(" + RejectText + ")" : ""));
		}

	}
}
