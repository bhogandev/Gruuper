using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.DTO.ClientModels
{
    public class ResponseMessage
    {
        public string user { get; set; }

        public string message { get; set; }

        public string time { get; set; }
    }
}
