using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    [Table("Perusahaan")]
    public class Perusahaan : BaseModel
    {
        public string Nama { get; set; }
        public string NoTelp { get; set; }
        public string Email { get; set; }
        public string Gedung { get; set; }
        public string Jalan { get; set; }
        public string Kota { get; set; }
        public int PersentaseKredit { get; set; }
        [ForeignKey("Rating_Id")]
        public Rating Rating { get; set; }
        public Int64 Rating_Id { get; set; }
        public decimal Nilai { get; set; }
    }
}