using System;

namespace pjank.BossaAPI.Fixml
{
	public class TrdSesStatusRequestMsg : FixmlMsg
	{

		private static uint nextId = 0;

		public uint Id;
		public SubscriptionRequestType Type;

		public TrdSesStatusRequestMsg()
		{
			Id = nextId++;
		}

		protected override void PrepareXmlMessage(string name)
		{
			base.PrepareXmlMessage("TrdgSesStatReq");
			xml.SetAttribute("ReqID", Id.ToString());
			xml.SetAttribute("SubReqTyp", Type.ToString("d"));
		}

		public override string ToString()
		{
			return string.Format("[{0}:{1}] {2}", Xml.Name, Id, Type);
		}

	}
}
