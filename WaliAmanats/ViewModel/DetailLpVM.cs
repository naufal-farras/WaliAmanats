using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
    public class DetailLpVM
    {
        public Int64 IdDetail { get; set; }
        public DetailKeuangan DetailKeuangan { get; set; }
        public Int64 Perusahaan_Id { get; set; }
        public string Perusahaan { get; set; }
        public Int64 Produk_Id { get; set; }
        public string Produk { get; set; }
        public bool Status { get; set; }
        public Int64 StatusCetakId { get; set; }
        public string StatusCetakNama { get; set; }
        public string Path { get; set; }
        public Int64 TransaksiLaporan_Id { get; set; }
        public string NamaProduk { get; set; }
        public DateTime TanggalCetak { get; set; }
        public DateTime TanggalSurat { get; set; }

        //public List<ListDetail> ListDetail { get; set; }
    }
    //public class ListDetail
    //{
    //    public DetailKeuangan DetailKeuangan { get; set; }
    //}

}