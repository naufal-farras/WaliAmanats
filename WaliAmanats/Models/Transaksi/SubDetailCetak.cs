using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Transaksi
{
    public class SubDetailCetak : BaseModel
    {
        public Int64 DetailCetak_Id { get; set; }
        public Int64 TransDet_Id { get; set; }

        [ForeignKey("DetailCetak_Id")]
        public DetailCetak DetailCetak { get; set; }
        [ForeignKey("TransDet_Id")]
        public TransaksiDetail TransaksiDetail { get; set; }
    }
}