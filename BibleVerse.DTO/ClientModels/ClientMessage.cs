using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.DTO.ClientModels
{
    public class ClientMessage
    {
        public string conId { get; set; }

        public string token { get; set; }

        public string groupId { get; set; }

        public BibleVerse.DTO.Messages Message { get; set; }
    }
}
