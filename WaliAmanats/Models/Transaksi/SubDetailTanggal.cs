using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Transaksi
{
    public class SubDetailTanggal : BaseModel
    {
        public Int64 SubDetail_Id { get; set; }
        public Int64 TransTanggal_Id { get; set; }

        [ForeignKey("SubDetail_Id")]
        public SubDetailCetak SubDetailCetak { get; set; }
        [ForeignKey("TransTanggal_Id")]
        public TransaksiTanggal TransaksiTanggal { get; set; }
    }
}