using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Master;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
    public class RatiosVM
    {

        public IEnumerable<Perusahaan> Perusahaan { get; set; }
      
        public List<KopSuratVM> KopSuratVM { get; set; }
        public List<DetailPerusahaan> DetailPerusahaan { get; set; }
        public KopSurat KopSurat { get; set;}
      

    }
    public class KopSuratVM
    {
        public string NamaPerusahaan { get; set; }
        public string Nosurat { get; set; }
        public DateTime? TanggalSurat { get; set; }
        public string StatusCetak { get; set; }
        public string WarnaCetak { get; set; }
        public bool Status { get; set; }
        public DetailPerusahaan DetailPerusahaan { get; set; }
    }

    public class RatVM
    {
        public Int64 Id { get; set; }
        public string Nama { get; set; }
    }
    public class SettingVM
    {
        public Int64 Id { get; set; }
        public string Ratio { get; set; }
        public string Measurement { get; set; }
        public decimal Target { get; set; }
    }
}