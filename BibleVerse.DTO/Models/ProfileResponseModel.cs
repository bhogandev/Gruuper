using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.DTO.Models
{
    public class ProfileResponseModel
    {

        public string ProfileId { get; set; }

        public string ProfilePictureURL { get; set; }

        public string UserName { get; set; }

        public string Bio { get; set; }

        public string Relationship { get; set; }

        public int Followers { get; set; }

        public int Following { get; set; }

        public string Theme { get; set; }

    }
}
