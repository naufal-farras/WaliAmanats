using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Models.Transaksi
{
    public class TransaksiCetak : BaseModel
    {
        public bool Status { get; set; }
        public Int64 StatusCetak_Id { get; set; }
        public DateTime TglSurat { get; set; } 
        public DateTime TglJatuhTempo { get; set; }

        [ForeignKey("StatusCetak_Id")]
        public StatusCetak StatusCetak { get; set; }
    }
}