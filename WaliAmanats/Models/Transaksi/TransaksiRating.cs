using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Models.Transaksi
{
    public class TransaksiRating : BaseModel
    {
      
        [ForeignKey("Perusahaan_Id")]
        public Perusahaan Perusahaan { get; set; }
        public Int64 Perusahaan_Id { get; set; }
        [ForeignKey("Rating_Id")]
        public Rating Rating { get; set; }
        public Int64 Rating_Id { get; set; }
        public decimal Nilai { get; set; }
        [ForeignKey("Matching_Id")]
        public Matching Matching { get; set; }
        public Int64 Matching_Id { get; set; }
        public DateTime TanggalInput { get; set; }
        [ForeignKey("StatusCetak_Id")]
        public StatusCetak StatusCetak { get; set; }
        public Int64 StatusCetak_Id { get; set; }
        public bool Status { get; set; }
        public string Path { get; set; }
        
    }
}