using System.Net.Sockets;

namespace pjank.BossaAPI.Fixml
{
	public class UserResponseMsg : FixmlMsg
	{
		public const string MsgName = "UserRsp";

		public uint UserReqID { get; private set; }
		public string Username { get; private set; }
		public UserStatus Status { get; private set; }
		public string StatusText { get; private set; }

		public UserResponseMsg(Socket socket) : base(socket) { }
		public UserResponseMsg(FixmlMsg m) : base(m) { }

		protected override void ParseXmlMessage(string name)
		{
			base.ParseXmlMessage(MsgName);
			UserReqID = FixmlUtil.ReadUInt(xml, "UserReqID");
			Username = FixmlUtil.ReadString(xml, "Username", true);
			Status = UserStatUtil.Read(xml, "UserStat");
			StatusText = FixmlUtil.ReadString(xml, "UserStatText", true);
		}

		public override string ToString()
		{
			return string.Format("[{0}:{1}] '{2}' {3} {4}",
				Xml.Name, UserReqID, Username, Status,
				(StatusText != null ? "(" + StatusText + ")" : ""));
		}

	}
}
