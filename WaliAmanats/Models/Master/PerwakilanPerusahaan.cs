using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    [Table("PerwakilanPerusahaan")]
    public class PerwakilanPerusahaan : BaseModel
    {
        public Int64 Perusahaan_Id { get; set; }
        public string Nama { get; set; }
        public string Gender { get; set; }
        public Int64 Jabatan_Id { get; set; }

        [ForeignKey("Perusahaan_Id")]
        public Perusahaan Perusahaan { get; set; }
        [ForeignKey("Jabatan_Id")]
        public Jabatan Jabatan { get; set; }
    }
}