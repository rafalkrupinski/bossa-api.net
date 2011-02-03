using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public class StatementMsg : FixmlMsg
    {
        public const string MsgName = "Statement";

        public StatementData[] Statements { get; private set; }

        public StatementMsg(FixmlMsg m) : base(m) { }

        protected override void ParseXmlMessage(string name)
        {
            base.ParseXmlMessage(MsgName);
            List<StatementData> list = new List<StatementData>();
            foreach (XmlElement stmt in xmlDoc.GetElementsByTagName(MsgName))
                list.Add(new StatementData(stmt));
            Statements = list.ToArray();
        }

        public override string ToString()
        {
            var a = Statements.Select(s => s.ToString()).ToArray();
            return base.ToString() + "\n" + string.Join("\n", a);
        }

    }
}
