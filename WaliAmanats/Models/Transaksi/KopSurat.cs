using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Models.Transaksi
{
    public class KopSurat : BaseModel
    {
        [ForeignKey("Perusahaan_Id")]
        public Perusahaan Perusahaan { get; set; }
        public Int64 Perusahaan_Id { get; set; }
        public string NoSurat { get; set; }
        public DateTime? TanggalSurat { get; set; }
        public string Periode { get; set; }
        public bool Status { get; set; }
        [ForeignKey("StatusCetak_Id")]
        public StatusCetak StatusCetak { get; set; }
        public Int64 StatusCetak_Id { get; set; }
        public string Path { get; set; }
    }
}