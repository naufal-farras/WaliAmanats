using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    public class EmitenNotes : BaseModel
    {
        public string notes { get; set; }
        public Int64 Perusahaan_Id { get; set; }
        [ForeignKey("Perusahaan_Id")]
        public Perusahaan Perusahaan { get; set; }
    }
}