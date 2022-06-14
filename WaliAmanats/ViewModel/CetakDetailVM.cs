using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
    public class CetakDetailVM
    {
        public Int64 Id { get; set; }
        public Int64 Perusahaan_Id { get; set; }
        public string Perusahaan { get; set; }
        public string AlamatPerusahaan { get; set; }
        public Int64 NamaPerwakilan_Id { get; set; }
        public string Perwakilan { get; set; }
        public Int64 Jabatan_Id { get; set; }
        public string Jabatan { get; set; }
        public Int64 Produk_Id { get; set; }
        public string Produk { get; set; }
        public string NamaProduk { get; set; }
        public Int64 JenisTugas_Id { get; set; }
        public string JenisTugas { get; set; }
        public Int64 MataUang_Id { get; set; }
        public decimal Fee { get; set; }
        public Int64 StatusCetak_Id { get; set; }

        //Pengiriman
        public string JenisPengiriman { get; set; }
        public string NamaBank { get; set; }
        public string NamaPenerima { get; set; }
        public string NoRekening { get; set; }

        public IEnumerable<DetailVM> Details { get; set; }
    }

    public class DetailVM
    {
        public Int64 Id { get; set; }
        public Int64 Trans_Id { get; set; }
        public string Seri { get; set; }
        public decimal Nominal { get; set; }
        public DateTime TglTerbit { get; set; }
        public double Bunga { get; set; }
        public DateTime TglJatuhTempo { get; set; }
        public int Periode { get; set; }
        public decimal JumlahBunga { get; set; }
        public DateTime TransaksiTanggals { get; set; }
        public decimal NilaiBunga { get; set; }
        public IEnumerable<TransaksiTanggalVM> Tanggal { get; set; }
    }

    public class TransaksiTanggalVM
    {
        public Int64 Id { get; set; }
        public Int64 Detail_Id { get; set; }
        public DateTime TglSuratBunga { get; set; }
        public int NoKupon { get; set; }
        public bool Status { get; set; }
        public decimal NilaiBunga { get; set; }
    }
}