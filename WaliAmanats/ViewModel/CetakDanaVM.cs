using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaliAmanats.ViewModel
{
    public class CetakDanaVM
    {
        public string Produk { get; set; }
        public string NamaProduk { get; set; }
        public string Initial { get; set; }
        public string Nama_Perusahaan { get; set; }
        public string Gedung { get; set; }
        public string Jalan { get; set; }
        public string Kota { get; set; }
        public string NamaPerwakilan { get; set; }
        public string Gender { get; set; }
        public string NamaJabatan { get; set; }
        public string JenisTugas { get; set; }
        public List<Detail> Detail { get; set; }

    }
    public class Detail
    {
        public string NamaProd { get; set; }
        public string Init { get; set; }
        public string MataUang { get; set; }
        public decimal NominalOutStanding { get; set; }
    }
}