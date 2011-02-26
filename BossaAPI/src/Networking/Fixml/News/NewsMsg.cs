using System.Text;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public class NewsMsg : FixmlMsg
	{
		public const string MsgName = "News";

		public string OrigTime { get; private set; }
		public string Headline { get; private set; }
		public string Text { get; private set; }

		public NewsMsg(FixmlMsg m) : base(m) { }

		protected override void ParseXmlMessage(string name)
		{
			base.ParseXmlMessage(MsgName);
			OrigTime = FixmlUtil.ReadString(xml, "OrigTm");
			Headline = FixmlUtil.ReadString(xml, "Headline");
			StringBuilder sb = new StringBuilder();
			foreach (XmlElement elem in xml.GetElementsByTagName("TxtLn"))
			{
				if (sb.Length > 0) sb.AppendLine();
				sb.Append(FixmlUtil.ReadString(elem, "Txt").TrimEnd());
			}
			Text = sb.ToString();
		}

		public override string ToString()
		{
			return string.Format("[{0}] '{1}'", Xml.Name, Text);
		}

	}
}
