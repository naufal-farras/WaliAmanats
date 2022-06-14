using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Models.Transaksi
{
    public class TransaksiLaporan : BaseModel
    {
        public Int64 Perusahaan_Id { get; set; }
        public Int64 NamaPerwakilan_Id { get; set; }
        public Int64 Jabatan_Id { get; set; }
        public Int64 Produk_Id { get; set; }
        public string NamaProduk { get; set; }
        public string JenisPembayaran { get; set; }
        public Int64 JenisTugas_Id { get; set; }
        public Int64 MataUang_Id { get; set; }
        public decimal Fee { get; set; }
        public string NoRef { get; set; }
        public Int64 Status_Id { get; set; }
        public Int64 StatusCetak_Id { get; set; }
        public string UserId { get; set; }
        public bool isDelete { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser Maker { get; set; }

        [ForeignKey("Perusahaan_Id")]
        public Perusahaan Perusahaan { get; set; }
        [ForeignKey("NamaPerwakilan_Id")]
        public PerwakilanPerusahaan Perwakilan { get; set; }
        [ForeignKey("Produk_Id")]
        public Produk Produk { get; set; }
        [ForeignKey("Jabatan_Id")]
        public Jabatan Jabatan { get; set; }
        [ForeignKey("JenisTugas_Id")]
        public JenisTugas JenisTugas { get; set; }
        [ForeignKey("MataUang_Id")]
        public MataUang MataUang { get; set; }
        [ForeignKey("Status_Id")]
        public Status Status { get; set; }
        [ForeignKey("StatusCetak_Id")]
        public StatusCetak StatusCetak { get; set; }


        //Pengiriman
        public string JenisPengiriman { get; set; }
        public string NamaBank { get; set; }
        public string NamaPenerima { get; set; }
        public string NoRekening { get; set; }
    }
}