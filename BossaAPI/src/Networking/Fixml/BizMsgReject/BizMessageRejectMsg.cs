
namespace pjank.BossaAPI.Fixml
{
	public class BizMessageRejectMsg : FixmlMsg
	{
		public const string MsgName = "BizMsgRej";

		public BizMsgReferenceMsgType ReferenceMsgType { get; private set; }
		public BizMsgRejectReason RejectReason { get; private set; }
		public string RejectText { get; private set; }

		public BizMessageRejectMsg(FixmlMsg m) : base(m) { }

		protected override void ParseXmlMessage(string name)
		{
			base.ParseXmlMessage(MsgName);
			ReferenceMsgType = BizRefMsgTypeUtil.Read(xml, "RefMsgTyp");
			RejectReason = BizRejRsnUtil.Read(xml, "BizRejRsn");
			RejectText = FixmlUtil.ReadString(xml, "Txt", true);
		}

		public override string ToString()
		{
			return string.Format("[{0}] '{1}' ({2}, {3})",
				Xml.Name, RejectText, ReferenceMsgType, RejectReason);
		}

	}
}
