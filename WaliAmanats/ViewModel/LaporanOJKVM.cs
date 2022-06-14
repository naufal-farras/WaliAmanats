using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaliAmanats.ViewModel
{
    public class LaporanOJKVM
    {
        public Int64 Id { get; set; }
        public Int64 Perusahaan_Id { get; set; }
        public string Perusahaan { get; set; }
        public Int64 Produk_Id { get; set; }
        public string Produk { get; set; }
        public string NamaProduk { get; set; }
        public IEnumerable<DetailOJKVM> Details { get; set; }
    }

    public class DetailOJKVM
    {
        public Int64 Id { get; set; }
        public string Seri { get; set; }
        public decimal Nominal { get; set; }
        public DateTime TglTerbit { get; set; }
        public double Bunga { get; set; }
        public DateTime TglJatuhTempo { get; set; }
        public int Periode { get; set; }
        public decimal JumlahBunga { get; set; }
        public IEnumerable<TanggalBungaVM> TanggalBunga { get; set; }
    }

    public class TanggalBungaVM
    {
        public Int64 Id { get; set; }
        public Int64 Detail_Id { get; set; }
        public DateTime TglSuratBunga { get; set; }
        public int NoKupon { get; set; }
        public bool Status { get; set; }
        public decimal NilaiBunga { get; set; }
    }
}