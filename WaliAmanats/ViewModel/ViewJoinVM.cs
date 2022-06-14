using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
    public class ViewJoinVM
    {
        public DateTime TanggalBunga { get; set; }
        public Int64 Perusahaan_Id { get; set; }
        public string Perusahaan { get; set; }
        public string Jalan { get; set; }
        public string Kota { get; set; }
        public Int64 NamaPerwakilan_Id { get; set; }
        public string Perwakilan { get; set; }
        public Int64 StatusCetak { get; set; }
        public string Penyebut { get; set; }
        public Int64 Jabatan_Id { get; set; }
        public string Jabatan { get; set; }
        public Int64 JenisTugas_Id { get; set; }
        public string JenisTugas { get; set; }
        public Int64 MataUang_Id { get; set; }
        public decimal Fee { get; set; }

        //Pengiriman
        public string JenisPengiriman { get; set; }
        public string NamaBank { get; set; }
        public string NamaPenerima { get; set; }
        public string NoRekening { get; set; }


        public List<ListProduk> ListProduk { get; set; }
    }

    public class ListProduk
    {
        public Int64 Id { get; set; }
        public Int64 Produk_Id { get; set; }
        public string Produk { get; set; }
        public string NamaProduk { get; set; }

        public IEnumerable<TransaksiTanggal> ListBunga { get; set; }
    }
}