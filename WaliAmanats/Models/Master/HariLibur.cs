using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    [Table("HariLibur")]
    public class HariLibur : BaseModel
    {
        public string Nama { get; set; }
        public DateTime TanggalLibur { get; set; }
    }
}