using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Master;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
    public class KejadianVM
    {
        public Int64? IdKejadian { get; set; }
        public Int64? IdPerusahaan { get; set; }
        public string NamaPerusahaan { get; set; }
        public string Informasi { get; set; }
        public string Kejadian { get; set; }
        public TransaksiKejadian TransaksiKejadian { get; set; }
        public List<ProductVM> ProductVM { get; set; }
        public IEnumerable<Perusahaan> Perusahaan { get; set; }
    }
    public class ProductVM
    {
        public Int64? IdLaporan { get; set; }
        public string Produk { get; set; }
        public string NamaProduk { get; set; }
        public IEnumerable<TransaksiTanggal> ListTanggal { get; set; }
    }
    public class ListVM
    {
        public Int64? Id { get; set; }
        public string Nama { get; set; }
        public int? Kupon { get; set; }
    }
  

}