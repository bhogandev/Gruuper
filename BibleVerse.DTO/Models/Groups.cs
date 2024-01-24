using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO.Models
{
    public class Groups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupID { get; set; }

        public string GroupUID { get; set; }

        public string GroupName { get; set; }

        [NotMapped]
        public List<GroupMembers> Members {get; set;}

        public DateTime CreateDateTime { get; set; }

        public DateTime ChangeDateTime { get; set; }

        #region Non Mapped Properties
        [NotMapped]
        public List<BibleVerse.DTO.ClientModels.ResponseMessage> Conversation { get; set; }
        #endregion
    }
}
