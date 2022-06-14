using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaliAmanats.ViewModel
{
    public class TransaksiCetakVM
    {
        public bool Status { get; set; }
        public Int64 StatusCetak_Id { get; set; }
        public DateTime TglSurat { get; set; }
    }

    public class TransDetailCetakVM
    {
        public string Perusahaan { get; set; }
        public string Produk { get; set; }
        public string NamaProduk { get; set; }
        public DateTime TglSurat { get; set; }
        public DateTime TglBunga { get; set; }

    }

    public class OjkVM
    {
        public int Detail_Id { get; set; }
        public string Nama_Perusahaan { get; set; }
        public int Persentase { get; set; }
        public string Initial { get; set; }
        public string NamaProduk { get; set; }
        public string Seri { get; set; }
        public DateTime TglTerbit { get; set; }
        public DateTime TglTempo { get; set; }
        public decimal Nominal { get; set; }
        public int Kupon { get; set; }
        public decimal NilaiBunga { get; set; }

    }

    public class LaporanKeuanganVM
    {
        public string Nama_Perusahaan { get; set; }
        public string Gedung { get; set; }
        public string Jalan { get; set; }
        public string Kota { get; set; }

        public string NamaPerwakilan { get; set; }
        public string Gender { get; set; }

        public string NamaJabatan { get; set; }
        public string NamaAgen { get; set; }

        public DateTime TanggalInput { get; set; }
        public DateTime JatuhTempo { get; set; }

    }

    public class LaporanJaminanVM
    {
        public string Nama_Perusahaan { get; set; }
        public string Gedung { get; set; }
        public string Jalan { get; set; }
        public string Kota { get; set; }

        public string NamaPerwakilan { get; set; }
        public string Gender { get; set; }

        public string NamaJabatan { get; set; }
        public string NamaAgen { get; set; }

        public string Initial { get; set; }
        public string NamaProduk { get; set; }

    }

    public class LaporanRatioVM
    {
        public string Nama_Perusahaan { get; set; }
        public string Gedung { get; set; }
        public string Jalan { get; set; }
        public string Kota { get; set; }

        public string NamaPerwakilan { get; set; }
        public string Gender { get; set; }

        public string NoSurat { get; set; }
        public DateTime? TanggalSurat { get; set; }

        public DateTime TanggalInput { get; set; }
        public string NamaRasio { get; set; }
        public string NamaMeasurements { get; set; }
        public string NamaMatchings { get; set; }
        public string Target { get; set; }
        public string Realisasi { get; set; }



    }
    public class LaporanRatioVM2
    {
        public string NamaJabatan { get; set; }
        public string NamaAgen { get; set; }

        public string Initial { get; set; }
        public string NamaProduk { get; set; }

    }

    public class LaporanRatingVM
    {
        public string Nama_Perusahaan { get; set; }
        public string Gedung { get; set; }
        public string Jalan { get; set; }
        public string Kota { get; set; }

        public string NamaPerwakilan { get; set; }
        public string Gender { get; set; }

        public string NamaJabatan { get; set; }
        public string NamaAgen { get; set; }
        public DateTime TanggalInput { get; set; }

        public string Initial { get; set; }
        public string NamaProduk { get; set; }

        public Int64 Target { get; set; }
        public Int64 Realisasi { get; set; }
        public string Keterangan { get; set; }


    }

    public class LaporanMatchingVM
    {
        public Int64 Target { get; set; }
        public Int64 Realisasi { get; set; }
        public string Keterangan { get; set; }


    }


    public class LaporanDanaVM
    {
        public string Nama_Perusahaan { get; set; }
        public string Gedung { get; set; }
        public string Jalan { get; set; }
        public string Kota { get; set; }

        public string NamaPerwakilan { get; set; }
        public string Gender { get; set; }

        public string NamaJabatan { get; set; }
        public string NamaAgen { get; set; }
        //public DateTime TanggalInput { get; set; }

        public string Initial { get; set; }
        public string NamaProduk { get; set; }
        public Int64 TransId { get; set; }
        public decimal Outstanding { get; set; }




    }
}