using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Models.Transaksi
{
    public class TransaksiKejadian : BaseModel
    {
        [ForeignKey("Perusahaan_Id")]
        public Perusahaan Perusahaan{ get; set; }
        public Int64? Perusahaan_Id { get; set; }

        [ForeignKey("TransaksiDetail_Id")]
        public TransaksiDetail TransaksiDetail { get; set; }
        public Int64? TransaksiDetail_Id { get; set; }
        [ForeignKey("TransaksiTanggal_Id")]
        public TransaksiTanggal TransaksiTanggal { get; set; }
        public Int64? TransaksiTanggal_Id { get; set; }
        public DateTime TanggalInput { get; set; }
        public DateTime TanggalCetak { get; set; }
        [ForeignKey("Kejadian_Id")]
        public KejadianPenting KejadianPenting { get; set; }
        public Int64 Kejadian_Id { get; set; }
        public bool Status { get; set; }
        [ForeignKey("StatusCetak_Id")]
        public StatusCetak StatusCetak { get; set; }
        public Int64 StatusCetak_Id { get; set; }
        public string Path { get; set; }
        public string Keterangan { get; set; }
        public string Informasi { get; set; }
    }
}