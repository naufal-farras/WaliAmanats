using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Transaksi
{
    public class DetailCetak : BaseModel
    {
        public Int64 TransaksiCetak_Id { get; set; }
        public Int64 TransaksiLaporan_Id { get; set;}

        [ForeignKey("TransaksiCetak_Id")]
        public TransaksiCetak TransaksiCetak { get; set; }
        [ForeignKey("TransaksiLaporan_Id")]
        public TransaksiLaporan TransaksiLaporan { get; set; }
    }
}