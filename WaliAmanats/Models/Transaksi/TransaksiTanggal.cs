using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Models.Transaksi
{
    public class TransaksiTanggal : BaseModel
    {
        public Int64 Detail_Id { get; set; }
        public DateTime TglSuratBunga { get; set; }
        public int NoKupon { get; set; }
        public bool Status { get; set; }
        public decimal NilaiBunga { get; set; }

        [ForeignKey("Detail_Id")]
        public TransaksiDetail TransaksiDetail { get; set; }

        
    }
}