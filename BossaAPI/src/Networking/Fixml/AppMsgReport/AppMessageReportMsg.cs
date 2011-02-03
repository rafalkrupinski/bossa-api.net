
namespace pjank.BossaAPI.Fixml
{
    public class AppMessageReportMsg : FixmlMsg
    {
        public const string MsgName = "ApplMsgRpt";

        public string Id { get; private set; }
        public int? Type { get; private set; }
        public string Text { get; private set; }

        public AppMessageReportMsg(FixmlMsg m) : base(m) { }

        protected override void ParseXmlMessage(string name)
        {
            base.ParseXmlMessage(MsgName);
            Id = FixmlUtil.ReadString(xml, "ApplRepID");
            Type = FixmlUtil.ReadInt(xml, "ApplRepTyp", true);
            Text = FixmlUtil.ReadString(xml, "Txt");
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} {2}", 
                Xml.Name, Text, (Type != null ? "(" + Type + ")" : ""));
        }

    }
}
