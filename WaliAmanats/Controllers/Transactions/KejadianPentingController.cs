using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using Syncfusion.DocIO;
using System.Data.Entity;
using WaliAmanats.ViewModel;
using WaliAmanats.Models.Transaksi;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using Syncfusion.DocIO.DLS;
using System.Globalization;
using WaliAmanats.Models.Master;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;

namespace WaliAmanats.Controllers.Transactions
{
    public class KejadianPentingController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        private SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());
        // GET: KejadianPenting
        public ActionResult Index()
        {
            KejadianVM result = new KejadianVM();
            result.Perusahaan = _context.Perusahaan.ToList();
            return View(result);
        }
        public ActionResult CetakKejadian()
        {
            return View();
        }
        public ActionResult MasterKejadian()
        {
            return View();
        }
        public JsonResult Savemaster(KejadianPenting jaminan)
        {
            if (jaminan.Id == 0)
            {
                KejadianPenting rat = new KejadianPenting();
                rat.Nama = jaminan.Nama;
            
                _context.KejadianPenting.Add(rat);
                _context.SaveChanges();
            }
            else
            {
                var jamin = _context.KejadianPenting.Where(x => x.Id == jaminan.Id).FirstOrDefault();
                jamin.Id = jaminan.Id;
                jamin.Nama = jaminan.Nama;
                _context.Entry(jamin).State = EntityState.Modified;
            }


            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDataMaster()
        {
            var result = _context.KejadianPenting.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetById(int id)
        {
            var result = _context.KejadianPenting.Where(x => x.Id == id).FirstOrDefault();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewKejadianMTN(Int64 Id, DateTime tglCetak)
        {
            var cet = _context.TransaksiKejadian.Where(x => x.Id == Id).FirstOrDefault();

            var result = (from transaksiKejadian in _context.TransaksiKejadian
              .Include(x => x.KejadianPenting)
              .Include(x => x.Perusahaan)
              .Where(x => x.Id == Id).ToList()

                          select new MtnVM
                          {
                              IdKejadian = cet.Id,
                              IdPerusahaan = transaksiKejadian.Perusahaan_Id,
                              NamaPerusahaan = transaksiKejadian.Perusahaan.Nama,
                              Informasi = transaksiKejadian.Informasi,
                              Kejadian = transaksiKejadian.Keterangan,



                              TransaksiKejadian = _context.TransaksiKejadian.Include(x => x.KejadianPenting)
                       .Include(x => x.TransaksiDetail)
                       .Include(x => x.TransaksiTanggal)
                       .Where(x => x.Id == Id && x.TanggalCetak == tglCetak).FirstOrDefault()
                          }).ToList(); 


            return View(result);
        }
        public ActionResult ViewKejadian(Int64 Id, DateTime tglCetak)
        {
              var cetak = _context.TransaksiKejadian.Include(x => x.KejadianPenting)
             .Include(x => x.TransaksiDetail)
             .Include(x => x.TransaksiTanggal)
             .Where(x => x.Id == Id)
             .Select(x => x.TransaksiDetail_Id).ToList();



                var tanggal = _context.TransaksiKejadian.Include(x => x.TransaksiTanggal).Where(x => x.Id == Id).Select(x => x.TransaksiTanggal_Id).ToList();
                var cet = _context.TransaksiKejadian.Include(x => x.TransaksiTanggal).Include(x => x.TransaksiDetail).Where(x => x.Id == Id).FirstOrDefault();


                var  result = (from emiten in _context.TransaksiDetail
                              .Include(x => x.TransaksiLaporan)
                              .Include(x => x.TransaksiLaporan.Perusahaan)
                              .Where(x => x.Id == cetak.FirstOrDefault()).ToList()

                              select new KejadianVM
                              {
                                  IdKejadian = cet.Id,
                                  IdPerusahaan = emiten.TransaksiLaporan.Perusahaan.Id,
                                  NamaPerusahaan = emiten.TransaksiLaporan.Perusahaan.Nama,

                                  TransaksiKejadian = _context.TransaksiKejadian.Include(x => x.KejadianPenting)
                                  .Include(x => x.TransaksiDetail)
                                  .Include(x => x.TransaksiTanggal)
                                  .Where(x => x.Id == Id && x.TanggalCetak == tglCetak).FirstOrDefault(),

                                  ProductVM = (from product in _context.TransaksiDetail
                                               .Include(x => x.TransaksiLaporan)
                                               .Include(x => x.TransaksiLaporan.Produk)
                                               .Where(x => cetak.Contains(x.Id)).ToList()
                                               select new ProductVM
                                               {
                                                   IdLaporan = product.TransaksiLaporan.Id,
                                                   NamaProduk = product.TransaksiLaporan.NamaProduk,
                                                   Produk = product.TransaksiLaporan.Produk.Nama,
                                                   ListTanggal = _context.TransaksiTanggal.Include(x => x.TransaksiDetail).Include(x => x.TransaksiDetail.TransaksiLaporan).
                                                   Where(x => x.Detail_Id == product.Id && x.Id == tanggal.FirstOrDefault()).ToList()
                                               }
                                  ).ToList()
                              }
                              ).ToList();
             

            



            return View(result);
        }
        public void CetakK(Int64 Id, DateTime Tanggal, Int64 Idc)
        {
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "TemplateKejadian");
            WordDocument document = new WordDocument(_path, FormatType.Docx);
            var trsCetak = _context.TransaksiKejadian.Single(x => x.Id == Idc);
            trsCetak.StatusCetak_Id = 5;
            //_context.Entry(Lap).State = EntityState.Modified;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();

            var data = _context.TransaksiKejadian
                .Include(x => x.TransaksiTanggal)
                .Include(x => x.TransaksiDetail)
                .Include(x => x.TransaksiDetail.TransaksiLaporan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Produk)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Jabatan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Perwakilan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.JenisTugas)
                .Where(x => x.TransaksiDetail.TransaksiLaporan.Id == Id && x.TanggalCetak == Tanggal)
                .ToList()
                .GroupBy(x => x.TransaksiTanggal_Id)
                .Select(x => x.FirstOrDefault());

            //var aturan = _context.AturanOJK.Single(x => x.Id == Aturan);
            //var alamatOJK = _context.OJK.FirstOrDefault();
            var nokup = data.FirstOrDefault().TransaksiTanggal.NoKupon + 1;
            document.Replace("%%NoKupon%%", nokup.ToString(), false, true);
            document.Replace("%%Produk%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Nama.ToString(), false, true);
            document.Replace("%%Initial%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Initial.ToString(), false, true);
            document.Replace("%%NamaProduk%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.NamaProduk.ToString(), false, true);
            document.Replace("%%Perusahaan%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Nama.ToString(), false, true);
            document.Replace("%%NamaPerwakilan%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Nama.ToString(), false, true);
            document.Replace("%%Jabatan%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Jabatan.NamaJabatan.ToString(), false, true);
            if (data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Gedung == null || data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Gedung == "")
            {
                document.Replace("%%Gedung%%", "", false, true);
            }
            else
            {
                document.Replace("%%Gedung%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Gedung, false, true);
            }
            document.Replace("%%Jalan%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Jalan.ToString(), false, true);
            document.Replace("%%Kota%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Kota.ToString(), false, true);
            document.Replace("%%TanggalCetak%%", data.FirstOrDefault().TransaksiTanggal.TglSuratBunga.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);
            document.Replace("%%JenisTugas%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.JenisTugas.Nama.ToString(), false, true);
            if (data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Gender == "Bapak")
            {
                document.Replace("%%Gender%%", "Bpk.", false, true);
            }
            else
            {
                document.Replace("%%Gender%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Gender, false, true);
            }

            WSection section = document.Sections[0];
            WTable table = section.Tables[0] as WTable;
            WTableRow row;
            WTableCell cell;
            IWParagraphStyle style = document.AddParagraphStyle("User");
            style.CharacterFormat.FontName = "Arial";
            IWParagraph paragraph = section.AddParagraph();

            //CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            // var no = 1;
            decimal nilai = 0;
            foreach (var item in data)
            {
                nilai = item.TransaksiTanggal.NilaiBunga;

                //Column perusahaan
                row = table.AddRow(true, false);
                // row.Height = 18;
                ////Specifies the row height type
                //row.HeightType = TableRowHeightType.AtLeast;
                //Adds the first cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText("Bagi Hasil Ke - " + " " + item.TransaksiTanggal.NoKupon.ToString()).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Adds the second cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(item.TransaksiTanggal.NilaiBunga.ToString("N2")).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Adds the third cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(item.TransaksiDetail.Nominal.ToString("N2")).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                cell = row.AddCell();
                cell.AddParagraph().AppendText(item.TransaksiTanggal.TglSuratBunga.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                //Adds the fourth cell into row

                cell.CellFormat.Paddings.Left = 100;


            }
            //if (data.Count() > 0)
            //{
            row = table.AddRow(true, false);
            //Adds the first cell into row
            cell = row.AddCell();
            cell.AddParagraph().AppendText("Jumlah").OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
            cell = row.AddCell();
            cell.AddParagraph().AppendText(nilai.ToString("N", CultureInfo.CreateSpecificCulture("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
            cell = row.AddCell();
            cell = row.AddCell();
            //}

            document.Save("Gagal Bayar ke Investor (Terbatas).doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }

        public void CetakGBUmum(Int64 Id, DateTime Tanggal, Int64 Idc)
        {
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "Template_GB_Investor");
            WordDocument document = new WordDocument(_path, FormatType.Docx);
            var trsCetak = _context.TransaksiKejadian.Single(x => x.Id == Idc);
            trsCetak.StatusCetak_Id = 5;
            //_context.Entry(Lap).State = EntityState.Modified;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();
            var data = _context.TransaksiKejadian
                .Include(x => x.TransaksiTanggal)
                .Include(x => x.TransaksiDetail)
                .Include(x => x.TransaksiDetail.TransaksiLaporan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Produk)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Jabatan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Perwakilan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.JenisTugas)
                .Where(x => x.TransaksiDetail.TransaksiLaporan.Id == Id && x.TanggalCetak == Tanggal)
                .ToList()
                .GroupBy(x => x.TransaksiTanggal_Id)
                .Select(x => x.FirstOrDefault());

            //var aturan = _context.AturanOJK.Single(x => x.Id == Aturan);
            //var alamatOJK = _context.OJK.FirstOrDefault();
            var nokup = data.FirstOrDefault().TransaksiTanggal.NoKupon + 1;
            document.Replace("%%NoKupon%%", nokup.ToString(), false, true);
            document.Replace("%%Produk%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Nama.ToString(), false, true);
            document.Replace("%%Initial%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Initial.ToString(), false, true);
            document.Replace("%%NamaProduk%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.NamaProduk.ToString(), false, true);
            document.Replace("%%Perusahaan%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Nama.ToString(), false, true);
            document.Replace("%%NamaPerwakilan%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Nama.ToString(), false, true);
            document.Replace("%%Jabatan%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Jabatan.NamaJabatan.ToString(), false, true);
            document.Replace("%%NilaiBunga%%", data.FirstOrDefault().TransaksiDetail.Nominal.ToString("N2"), false, true);
            if (data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Gedung == null || data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Gedung == "")
            {
                document.Replace("%%Gedung%%", "", false, true);
            }
            else
            {
                document.Replace("%%Gedung%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Gedung, false, true);
            }
            document.Replace("%%Jalan%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Jalan.ToString(), false, true);
            document.Replace("%%Kota%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Kota.ToString(), false, true);
            document.Replace("%%TanggalCetak%%", data.FirstOrDefault().TransaksiTanggal.TglSuratBunga.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);
            document.Replace("%%JatuhTempo%%", data.FirstOrDefault().TransaksiDetail.TglJatuhTempo.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);
            document.Replace("%%JenisTugas%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.JenisTugas.Nama.ToString(), false, true);
            if (data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Gender == "Bapak")
            {
                document.Replace("%%Gender%%", "Bpk.", false, true);
            }
            else
            {
                document.Replace("%%Gender%%", data.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Gender, false, true);
            }
            
            document.Save("Gagal Bayar ke Investor (Umum).doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }

        public void CetakMTN(DateTime Tanggal, Int64 Idc)
        {
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "Template_Daftar-MTN");
            WordDocument document = new WordDocument(_path, FormatType.Docx);
            var trsCetak = _context.TransaksiKejadian.Single(x => x.Id == Idc);
            trsCetak.StatusCetak_Id = 5;
            //_context.Entry(Lap).State = EntityState.Modified;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();

            var getIdper = _context.TransaksiKejadian.Where(x => x.Id == Idc).Select(x =>x.Perusahaan_Id).FirstOrDefault();

            var getidperrr = getIdper;
            var Idper = Convert.ToInt64(getidperrr);
            var data = _con.Query<CetakMTNVM>(
           "EXEC SP_CetakMTN @Idper",
            new
            {
                Idper = Idper
            }).ToList();

            //document.Replace("%%Produk%%", data.FirstOrDefault().Produk.Nama.ToString(), false, true);
            //document.Replace("%%Initial%%", data.FirstOrDefault().Initial.ToString(), false, true);
            document.Replace("%%NamaProduk%%", data.FirstOrDefault().NamaProduk.ToString(), false, true);
            document.Replace("%%Perusahaan%%", data.FirstOrDefault().Nama_Perusahaan.ToString(), false, true);
            document.Replace("%%NamaPerwakilan%%", data.FirstOrDefault().NamaPerwakilan.ToString(), false, true);
            document.Replace("%%Jabatan%%", data.FirstOrDefault().NamaJabatan.ToString(), false, true);
            if (data.FirstOrDefault().Gedung == null || data.FirstOrDefault().Gedung == "")
            {
                document.Replace("%%Gedung%%", "", false, true);
            }
            else
            {
                document.Replace("%%Gedung%%", data.FirstOrDefault().Gedung, false, true);
            }
            document.Replace("%%Jalan%%", data.FirstOrDefault().Jalan.ToString(), false, true);
            document.Replace("%%Kota%%", data.FirstOrDefault().Kota.ToString(), false, true);
            //document.Replace("%%TanggalCetak%%", data.FirstOrDefault().TransaksiTanggal.TglSuratBunga.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);
            document.Replace("%%JenisTugas%%", data.FirstOrDefault().NamaAgen.ToString(), false, true);
            if (data.FirstOrDefault().Gender == "Bapak")
            {
                document.Replace("%%Gender%%", "Bpk.", false, true);
            }
            else
            {
                document.Replace("%%Gender%%", data.FirstOrDefault().Gender, false, true);
            }

            WSection section = document.Sections[0];
            WTable table = section.Tables[0] as WTable;
            WTableRow row;
            WTableCell cell;
            IWParagraphStyle style = document.AddParagraphStyle("User");
            style.CharacterFormat.FontName = "Arial";
            IWParagraph paragraph = section.AddParagraph();

            //CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            // var no = 1;
            //decimal nilai = 0;
            decimal no = 1;
            foreach (var item in data)
            {
                //nilai = item.TransaksiTanggal.NilaiBunga;

                //Column perusahaan
                row = table.AddRow(true, false);

                cell = row.AddCell();
                cell.AddParagraph().AppendText(' ' + no.ToString()).CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no++;

                cell = row.AddCell();
                cell.AddParagraph().AppendText("MTN " + " " + item.NamaProduk.ToString() + " " + "Seri" + " " + item.Seri).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Adds the second cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(item.KodeEfek.ToString()).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Adds the third cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(DateTime.Now.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                //cell = row.AddCell();
                //cell.AddParagraph().AppendText(item.TransaksiTanggal.TglSuratBunga.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                ////Adds the fourth cell into row

                cell.CellFormat.Paddings.Left = 100;


            }


            document.Save("Daftar Pemegang MTN.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }

        public JsonResult GetKejadian()
        {
            var result = _context.KejadianPenting.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProduk(Int64 DataId)
        {
            List<ListVM> result = _context.TransaksiLaporan
                .Include(x => x.Perusahaan).Where(x => x.Perusahaan_Id == DataId && x.isDelete == false)
                .Select(x => new ListVM { Id = x.Id, Nama = x.NamaProduk }).OrderBy(x => x.Nama).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProduk2(Int64 DataId)
        {
            

            var result1 = _context.TransaksiKejadian
                         .Where(x => x.Perusahaan_Id == DataId)
                         .Select(x=> x.Perusahaan_Id).FirstOrDefault();

            var result2 = _context.TransaksiTanggal
                         .Where(x =>x.Status==false && x.Detail_Id==result1).ToList();
                        



            List<ListVM> result = _context.TransaksiLaporan
                .Include(x => x.Perusahaan)
                .Where(x => x.Perusahaan_Id == DataId && x.isDelete == false)
                .Select(x => new ListVM { Id = x.Id, Nama = x.NamaProduk }).OrderBy(x => x.Nama).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSeri(Int64 DataId)
        {
            List<ListVM> result = _context.TransaksiDetail.Include(x => x.TransaksiLaporan).Where(x => x.Trans_Id == DataId).Select(x => new ListVM { Id = x.Id, Nama = x.Seri }).OrderBy(x => x.Nama).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetKupon(Int64 DataId)
        {
            List<ListVM> result = _context.TransaksiTanggal.Include(x => x.TransaksiDetail).Where(x => x.Detail_Id == DataId && x.Status == false).Select(x => new ListVM { Id = x.Id, Kupon = x.NoKupon }).OrderBy(x => x.Kupon).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFilter()
        {
            FilterVM result = new FilterVM();
            result.Emiten = _context.Perusahaan.Select(x => x.Nama).ToList();
            result.Jenis = _context.Produk.Select(x => x.Initial).ToList();
            result.Produk = _context.TransaksiLaporan.Include(x => x.Perusahaan).Select(x => x.NamaProduk).ToList();
            result.Batas = _context.Rating.Select(x => x.Nama).ToList();
            result.Ratio = _context.Ratio.Select(x => x.Nama).ToList();
            result.Measure = _context.Measurement.Select(x => x.Nama).ToList();
            result.Keterangan = _context.Matching.Select(x => x.Nama).ToList();
            result.Kejadian = _context.KejadianPenting.Select(x => x.Nama).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string GetNamaPerById (string Id)
        {
            var result = _context.TransaksiLaporan.Where(x => x.Perusahaan_Id.ToString() == Id).FirstOrDefault();

            return result.Perusahaan.Nama.ToString();
        }
        public JsonResult GetAll()
        {
            var  result = _context.TransaksiKejadian.Include(x => x.KejadianPenting)
                .Include(x => x.TransaksiTanggal)
                .Include(x => x.Perusahaan)
                .Include(x => x.TransaksiDetail)
                .Include(x => x.TransaksiDetail.TransaksiLaporan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                .ToList();

            //result.Select(x => { x.TransaksiDetail.TransaksiLaporan.Perusahaan.Nama = GetNamaPerById(x.IdPer.ToString()); return x; }).ToList();

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }
        public JsonResult GetCetak()
        {
            var result = _context.TransaksiKejadian.Include(x => x.KejadianPenting).Include(x => x.TransaksiTanggal).Include(x => x.TransaksiDetail)
               .Include(x => x.TransaksiDetail.TransaksiLaporan).Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan).Include(x => x.StatusCetak)
               .Where(x => x.TanggalCetak.Year == DateTime.Now.Year && x.TanggalCetak.Month == DateTime.Now.Month
               && x.TanggalCetak.Day == DateTime.Now.Day).ToList();

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        public JsonResult GetCetakAll()
        {
            var result = _context.TransaksiKejadian.Include(x => x.KejadianPenting)
                .Include(x => x.Perusahaan)
                .Include(x => x.TransaksiTanggal).Include(x => x.TransaksiDetail)
               .Include(x => x.TransaksiDetail.TransaksiLaporan).Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan).Include(x => x.StatusCetak)
               .ToList();

         

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }
        public JsonResult GetMaster(Int64 id)
        {
            var result = _context.KejadianPenting.Where(x => x.Id == id).SingleOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPerusahaan()
        {
            var result = _context.Perusahaan.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetByIdTK(Int64 Id)
        {
            var result = _context.TransaksiKejadian.Where(x => x.Id == Id).SingleOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPer(Int64 Id)
        {
            var result = _context.TransaksiLaporan.Include(x => x.Perusahaan).ToList().Where(x => x.Perusahaan_Id==Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMTN(Int64 Id)
        {
         
            var result = _context.TransaksiLaporan.ToList().Where(x => x.Perusahaan_Id == Id && x.Produk_Id==1).FirstOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveMTN(Int64 IdPer, Int64 IdKej)
        {
            TransaksiKejadian trans = new TransaksiKejadian();
            trans.Perusahaan_Id = IdPer;
            trans.TransaksiDetail_Id = null;
            trans.TransaksiTanggal_Id = null;
            trans.Kejadian_Id = IdKej;
            trans.Informasi = null;
            trans.Keterangan = null;
            trans.Status = false;
            trans.TanggalInput = DateTime.Now;
            trans.TanggalCetak = DateTime.Now.Date;
            trans.ModifiedDate = DateTime.Now;
            trans.StatusCetak_Id = 2;
            trans.Path = null;
            _context.TransaksiKejadian.Add(trans);
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save(TransaksiKejadian kejadian)
        {
            TransaksiKejadian trans = new TransaksiKejadian();
            trans.Perusahaan_Id = kejadian.Perusahaan_Id;
            trans.TransaksiDetail_Id = kejadian.TransaksiDetail_Id;
            trans.TransaksiTanggal_Id = kejadian.TransaksiTanggal_Id;
            trans.Kejadian_Id = kejadian.Kejadian_Id;
            trans.Informasi = kejadian.Informasi;
            trans.Keterangan = kejadian.Keterangan;
            trans.Status = false;
            trans.TanggalInput = kejadian.TanggalInput;
            trans.TanggalCetak = GetMinusNewDate(kejadian.TanggalInput, 1);
            trans.ModifiedDate = DateTime.Now;
            trans.StatusCetak_Id = 2;
            trans.Path = null;
            _context.TransaksiKejadian.Add(trans);
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SubmitFile(string Id, HttpPostedFileBase file)
        {
            int idDetail;
            bool Detail = Int32.TryParse(Id, out idDetail);

            bool result = false;
            if (Detail == true)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                if (file != null)
                {
                    if (file.FileName.EndsWith("pdf") || file.FileName.EndsWith("PDF"))
                    {
                        var detail = _context.TransaksiKejadian.Include(x => x.TransaksiDetail)
                            .Include(x => x.TransaksiDetail.TransaksiLaporan)
                            .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                            .Include(x => x.TransaksiTanggal).Include(x => x.StatusCetak)
                            .SingleOrDefault(x => x.Id == idDetail);
                        if (detail.Path != null)
                        {
                            string paths = Server.MapPath("~/Files/Kejadian/" + detail.Path);
                            System.IO.File.Delete(paths);
                        }

                        var ext = Path.GetExtension(file.FileName);

                        var pathfile = detail.TransaksiDetail.Seri + "-" + detail.TransaksiTanggal.NoKupon + "-" + detail.TransaksiDetail.TransaksiLaporan.NamaProduk + "" + ext;
                        string path = Server.MapPath("~//Files/Kejadian/" + pathfile);
                        file.SaveAs(path);

                        detail.Path = pathfile;
                        detail.ModifiedDate = DateTime.Now;
                        _context.Entry(detail).State = EntityState.Modified;

                        _context.SaveChanges();
                    }

                    result = true;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Done(Int64 Id)
        {
            bool result = false;
            //foreach (var Ids in Id)
            //{
            var data = _context.TransaksiKejadian.SingleOrDefault(x => x.Id == Id);
            data.Status = true;
            _context.Entry(data).State = EntityState.Modified;
            _context.SaveChanges();
            result = true;
            //}

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(Int64 Id)
        {
            bool result = false;
            var delete = _context.TransaksiKejadian.SingleOrDefault(x => x.Id == Id);
            _context.TransaksiKejadian.Remove(delete);
            _context.SaveChanges();
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMaster(Int64 Id)
        {
            bool result = false;
            var delete = _context.KejadianPenting.SingleOrDefault(x => x.Id == Id);
            _context.KejadianPenting.Remove(delete);
            _context.SaveChanges();
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private DateTime GetMinusNewDate(DateTime startdate, int bil)
        {
            var nextFewJobDays = new List<DateTime>();
            var endDate = startdate.Date;
            endDate = endDate.AddDays(1);
            if (IsHoliday(endDate) || IsWeekEnd(endDate))
            {
                do
                {
                    endDate = endDate.AddDays(1);
                } while (IsHoliday(endDate) || IsWeekEnd(endDate));
            }

            return endDate;
        }
        private bool IsHoliday(DateTime date)
        {
            var holiday = _context.HariLibur.Select(x => x.TanggalLibur).Contains(date);
            return holiday;
        }

        private bool IsWeekEnd(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}