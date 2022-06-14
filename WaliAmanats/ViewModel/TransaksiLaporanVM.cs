using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
    public class TransaksiLaporanVM
    {
        public int Id { get; set; }
        public int Perusahaan_Id { get; set; }
        public int NamaPerwakilan_Id { get; set; }
        public int Jabatan_Id { get; set; }
        public int Produk_Id { get; set; }
        public string NamaProduk { get; set; }
        public int JenisTugas_Id { get; set; }
        public int MataUang_Id { get; set; }
        public string JenisPembayaran { get; set; }
        public int Fee { get; set; }
        
        //Pengiriman
        public string JenisPengiriman { get; set; }
        public string NamaBank { get; set; }
        public string NamaPenerima { get; set; }
        public string NoRekening { get; set; }

        public ICollection<TransaksiDetail> Details { get; set; }
    }
}