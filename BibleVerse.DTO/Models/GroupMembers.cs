using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO.Models
{
    public class GroupMembers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string GroupID { get; set; }

        public string UserName { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime ChangeDateTime { get; set; }
    }
}
