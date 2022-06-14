using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Models.Transaksi
{
    public class DetailKeuangan : BaseModel
    {
        [ForeignKey("TransaksiKeuangan_Id")]
        public TransaksiKeuangan TransaksiKeuangan { get; set; }
        public Int64 TransaksiKeuangan_Id { get; set; }
        public DateTime TanggalSurat { get; set; }
        public DateTime TanggalCetak { get; set; }
        [ForeignKey("StatusCetak_Id")]
        public StatusCetak StatusCetak { get; set; }
        public Int64 StatusCetak_Id { get; set; }
        public bool Status { get; set; }
        public string Path { get; set; }
    }
}