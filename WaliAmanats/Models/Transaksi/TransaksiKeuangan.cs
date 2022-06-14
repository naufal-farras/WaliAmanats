using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Models.Transaksi
{
    public class TransaksiKeuangan : BaseModel
    {
        
        [ForeignKey("TransaksiLaporan_Id")]
        public TransaksiLaporan TransaksiLaporan { get; set; }
        public Int64 TransaksiLaporan_Id { get; set; }
        public DateTime TanggalInput { get; set; }
        public DateTime JatuhTempo { get; set; }

    }
}