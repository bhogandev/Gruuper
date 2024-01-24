using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.DTO.ClientModels
{
    public class GroupModificationRequest
    {
        public string conID { get; set; }

        public string token { get; set; }

        public string userName { get; set; }

        public string groupId { get; set; }

        public string modification { get; set; }
    }
}
