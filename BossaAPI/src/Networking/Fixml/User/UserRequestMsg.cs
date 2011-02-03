using System;

namespace pjank.BossaAPI.Fixml
{
    public class UserRequestMsg : FixmlMsg
    {

        private static uint nextId = 0;

        public uint Id;
        public UserRequestType Type;
        public string Username;
        public string Password;
        public string NewPassword;

        public UserRequestMsg()
        {
            Id = nextId++;
        }

        protected override void PrepareXmlMessage(string name)
        {
            base.PrepareXmlMessage("UserReq");
            xml.SetAttribute("UserReqID", Id.ToString());
            xml.SetAttribute("UserReqTyp", Type.ToString("d"));
            xml.SetAttribute("Username", Username);
            if (Password != null) xml.SetAttribute("Password", Password);
            if (NewPassword != null) xml.SetAttribute("NewPassword", NewPassword);
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}] '{2}' {3}", Xml.Name, Id, Username, Type);
        }

    }
}
