using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public enum UserStatus
    {
        LoggedIn = 1,
        LoggedOut = 2,
        UserUnknown = 3,
        PasswordIncorrect = 4,
        PasswordChanged = 5,
        Other = 6,
        ForcedLogout = 7,
        SessionShutdown = 8
    }

    internal static class UserStatUtil
    {
        public static UserStatus Read(XmlElement xml, string name)
        {
            int number = FixmlUtil.ReadInt(xml, name);
            if (!Enum.IsDefined(typeof(UserStatus), (UserStatus)number))
                FixmlUtil.Error(xml, name, number, "- unknown UserStatus");
            return (UserStatus)number;
        }
    }
}
