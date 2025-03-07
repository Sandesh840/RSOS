using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Model.Models
{
    public class tblSchemeFile
    {
        public int Id { get; set; }
        [Required]
        public string FileUrl { get; set; }
        public int FileId { get; set; }

        [ForeignKey("FileId")]
        public tblScheme scheme { get; set; }
        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedOn { get; set; }
    }
}
