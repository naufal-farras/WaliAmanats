using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    [Table("Jabatan")]
    public class Jabatan : BaseModel
    {
        public string NamaJabatan { get; set; }
        public bool Jenis { get; set; }
    }
}