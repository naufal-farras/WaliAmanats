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
    public class TransaksiDetail : BaseModel
    {
        public Int64 Trans_Id { get; set; }
        public string Seri { get; set; }
        public string KodeEfek { get; set; }
        public decimal Nominal { get; set; }
        public DateTime TglTerbit { get; set; }
        public double Bunga { get; set; }
        public DateTime TglJatuhTempo { get; set; }
        public Int64 Periode_Id { get; set; }

        [ForeignKey("Trans_Id")]
        public TransaksiLaporan TransaksiLaporan { get; set; }

        [ForeignKey("Periode_Id")]
        public Periode Periode { get; set; }




    }
}