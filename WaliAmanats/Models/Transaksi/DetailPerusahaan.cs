using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Models.Transaksi
{
    public class DetailPerusahaan : BaseModel
    {
        [ForeignKey("KopSurat_Id")]
        public KopSurat KopSurat { get; set; }
        public Int64 KopSurat_Id { get; set; }
        [ForeignKey("Ratio_Id")]
        public Ratio Ratio { get; set; }
        public Int64 Ratio_Id { get; set; }
        [ForeignKey("Measurement_Id")]
        public Measurement Measurement { get; set; }
        public Int64 Measurement_Id { get; set; }
        public string Target { get; set; } 
        public string Realisasi { get; set; }
        [ForeignKey("Matching_Id")]
        public Matching Matching { get; set; }
        public Int64 Matching_Id { get; set; }
        public DateTime CreateDate { get; set; }
    }
}