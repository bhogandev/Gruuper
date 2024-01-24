using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Events
{
    public class MessagingConnectEvent : Event
    {
        private static string eventType = "MESSENGERCONNECTION";
        private static string eventContext = "This User Connected To Messenger:";

        public MessagingConnectEvent(BibleVerse.DALV2.BVIdentityContext context, string author)
            :base(context, string.Format("{0}{1}",eventContext, author), eventType)
        {

        }
    }
}
