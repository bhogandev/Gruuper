using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Events.Resources
{
    public class EmailResources
    {
        public static string ReturnNotificationBody(BibleVerse.DTO.Notifications notification)
        {
            string notificationBody = string.Empty;

            switch (notification.Type)
            {
                case "FORGOTPASSWORD":
                    return "";
                default:
                    return notificationBody;
            }
        }
    }
}
