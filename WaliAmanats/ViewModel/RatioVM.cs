using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Master;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
    public class RatioVM
    {
        public Int64 IdPerusahaan { get; set; }
        public Int64 IdProduk { get; set; }
        // public TransaksiRatio TransaksiRatio { get; set; }
        public List<SettingRatio> Setting { get; set; }
        public List<DetailPerusahaan> DetailPerusahaan { get; set; }


        public KopSurat KopSurat { get; set; }
        public DetailPerusahaan RealisasiRatio { get; set; }
        public IEnumerable<Ratio> Ratio { get; set; }

    }
}