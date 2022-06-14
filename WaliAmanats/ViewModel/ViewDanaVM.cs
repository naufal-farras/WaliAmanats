using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Master;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
    public class ViewDanaVM
    {
        public Int64 Iddana { get; set; }
        public Int64 IdPerusahaan { get; set; }
        public string NamaPerusahaan { get; set; }
        //public Perusahaan Perusahaan { get; set; }
        public TransaksiPenggunaanDana TransaksiPenggunaanDana { get; set; }
        public List<ProdukVM> ProdukVM { get; set; }
    }
    public class ProdukVM
    {
      public Int64 IdProduk { get; set; }
      public Int64 IdLaporan { get; set; }
      public string Produk { get; set; }
      public string NamaProduk { get; set; }
   //   public List<TransaksiDetail> TransaksiDetail { get; set; }
      public List<DetailProdukVM> DetailProdukVM { get; set; }  
    
    }
    public class DetailProdukVM
    {
        public Int64 IdDetail { get; set; }
        public decimal Nominal { get; set; }
        public string Seri { get; set; }
        public List<TransaksiDetail> TransaksiDetail { get; set; }
    }


}