using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Data.SqlClient;
using WaliAmanats.Models;
using System.Configuration;
using System.Data.Entity;
using WaliAmanats.Models.Transaksi;
using WaliAmanats.ViewModel;
using System.IO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using System.Globalization;

namespace WaliAmanats.Controllers.Transactions
{
    public class CetakSuratController : Controller
    {
        // GET: CetakSurat
        private SqlConnection _con;
        private ApplicationDbContext _context;
        private int count;

        public CetakSuratController()
        {
            _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());
            _context = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Otorisasi()
        {
            return View();
        }

        public ActionResult Cek()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        public ActionResult listCetak()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            var result = (from lap in _context.TransaksiLaporan
                          .Include(x => x.Perusahaan)
                          .Include(x => x.Produk)
                          .Include(x => x.StatusCetak)
                          .ToList()
                          where lap.Status_Id == 2 && lap.StatusCetak_Id == 1 && lap.isDelete == false
                          select new
                          {
                              Id = lap.Id,
                              lap.Perusahaan,
                              lap.Produk,
                              lap.NamaProduk,
                              TglSuratTerdekat = TanggalSuratTerdekat(lap.Id),
                              TglCetak = TanggalCetak(lap.Id).AddDays(-7),
                              lap.StatusCetak,
                              TglHariIni = DateTime.Now
                          }).ToList();
            //Int64[] ids = { 3 };


            //var result = (from tgl in _context.TransaksiTanggal
            //              .Include(x => x.TransaksiDetail)
            //              .Include(x => x.TransaksiDetail.TransaksiLaporan)
            //              .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
            //              .Include(x => x.TransaksiDetail.TransaksiLaporan.Produk)
            //              .Include(x => x.StatusCetak)
            //              .Where(tgl => tgl.TransaksiDetail.TransaksiLaporan.Status_Id == 2 && tgl.StatusCetak_Id == 1
            //              && tgl.Status == false)
            //              join det in _context.TransaksiDetail on tgl.Detail_Id equals det.Id
            //              where det.
            //              .ToList()
            //              .GroupBy(x => x.TransaksiDetail.Trans_Id)
            //              select new
            //              {
            //                  Id = tgl.FirstOrDefault().Id,
            //                  Trans_Id = tgl.FirstOrDefault().TransaksiDetail.Trans_Id,
            //                  Perusahaan = tgl.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Nama,
            //                  Produk = tgl.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Nama,
            //                  tgl.FirstOrDefault().TransaksiDetail.TransaksiLaporan.NamaProduk,
            //                  TglSuratTerdekat = TanggalSuratTerdekat(tgl.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Id),
            //                  TglCetak = TanggalCetak(tgl.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Id),
            //                  tgl.FirstOrDefault().StatusCetak,
            //                  TglHariIni = DateTime.Now
            //              }).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(Int64 Id)
        {
            bool result = false;
            var b = _context.TransaksiCetak.Where(x => x.Id == Id).FirstOrDefault();
            if (b != null)
            {
                _context.TransaksiCetak.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateStatus(Int64 Id)
        {
            var lap = new TransaksiLaporan();
            lap = _context.TransaksiLaporan.Single(x => x.Id == Id);
            lap.StatusCetak_Id = 2;
            _context.Entry(lap).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult View(Int64 Id)
        {
            var result = (from lap in _context.TransaksiLaporan
                                    .Include(x => x.Perusahaan)
                                    .Include(x => x.Perwakilan)
                                    .Include(x => x.Produk)
                                    .Include(x => x.JenisTugas)
                                    .Include(x => x.Jabatan)
                                    .ToList()
                          where lap.Id == Id
                          select new CetakDetailVM
                          {
                              Id = lap.Id,
                              Perusahaan_Id = lap.Perusahaan_Id,
                              Perusahaan = lap.Perusahaan.Nama,
                              AlamatPerusahaan = lap.Perusahaan.Jalan,
                              Produk_Id = lap.Produk_Id,
                              Produk = lap.Produk.Nama,
                              NamaProduk = lap.NamaProduk,
                              NamaPerwakilan_Id = lap.NamaPerwakilan_Id,
                              Perwakilan = lap.Perwakilan.Nama,
                              Jabatan_Id = lap.Jabatan_Id,
                              Jabatan = lap.Jabatan.NamaJabatan,
                              JenisTugas_Id = lap.JenisTugas_Id,
                              JenisTugas = lap.JenisTugas.Nama,
                              Fee = lap.Fee,
                              JenisPengiriman = lap.JenisPengiriman,
                              NamaBank = lap.NamaBank,
                              NamaPenerima = lap.NamaPenerima,
                              NoRekening = lap.NoRekening,
                              Details = (from det in _context.TransaksiDetail.ToList()
                                         where det.Trans_Id == lap.Id
                                         select new DetailVM
                                         {
                                             Id = det.Id,
                                             Seri = det.Seri,
                                             //Periode = det.Periode,
                                             Nominal = det.Nominal,
                                             TglTerbit = det.TglTerbit,
                                             Bunga = det.Bunga,
                                             TglJatuhTempo = det.TglJatuhTempo,
                                             //TransaksiTanggals = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.TransaksiLaporan.Id == lap.Id && x.Status == false).Select(x => x.TglSuratBunga).FirstOrDefault(),
                                             Tanggal = (from tgl in _context.TransaksiTanggal.ToList()
                                                        where tgl.Detail_Id == det.Id && tgl.Status == false
                                                        select new TransaksiTanggalVM
                                                        {
                                                            Id = tgl.Id,
                                                            TglSuratBunga = tgl.TglSuratBunga,
                                                            NoKupon = tgl.NoKupon,
                                                            Status = tgl.Status,
                                                            NilaiBunga = tgl.NilaiBunga
                                                        })
                                         }).ToList()
                          }).ToList();
            return View(result);
        }

        public ActionResult CetakList(Int64 Id)
        {
            DateTime TglTerdekat = TanggalSuratTerdekat(Id);

            var result = _context.TransaksiTanggal.Include(x => x.TransaksiDetail).Where(x => x.TransaksiDetail.TransaksiLaporan.Id == Id && x.TglSuratBunga == TglTerdekat).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public void Cetak(Int64 Id, DateTime tanggal, string noref, Int64 cetakId)
        {
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "TemplateDoc2"); //mengambil path lokasi file
            WordDocument document = new WordDocument(_path, FormatType.Doc); // membaca isi 
            var dcId = _context.DetailCetak.Where(x => x.TransaksiLaporan_Id == Id).Select(x => x.TransaksiCetak_Id).FirstOrDefault();
            var Lap = _context.TransaksiLaporan.SingleOrDefault(lap => lap.Id == Id);
            var trsCetak = _context.TransaksiCetak.Single(x => x.Id == cetakId);
            //Lap.StatusCetak_Id = 1;
            trsCetak.StatusCetak_Id = 5;
            //_context.Entry(Lap).State = EntityState.Modified;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();

            bool trigger = false;

            var result = _context.TransaksiTanggal
                                    //.OrderBy(x => x.TglSuratBunga)
                                    .Include(x => x.TransaksiDetail)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.Perwakilan)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.Jabatan)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.JenisTugas)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.MataUang)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.Produk)
                                    .Where(x => x.TransaksiDetail.TransaksiLaporan.Id == Id && x.TglSuratBunga == tanggal)
                                    .ToList()
                                    .GroupBy(x => x.Detail_Id)
                                    .Select(x => x.FirstOrDefault());
            var init = result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Initial;
            var jabatan = result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Jabatan.NamaJabatan;
            var last = _context.TransaksiTanggal
                                    //.OrderBy(x => x.TglSuratBunga)
                                    .Include(x => x.TransaksiDetail)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.Perwakilan)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.Jabatan)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.JenisTugas)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.MataUang)
                                    .Include(x => x.TransaksiDetail.TransaksiLaporan.Produk)
                                    .Where(x => x.TransaksiDetail.TransaksiLaporan.Id == Id)
                                    .ToList()
                                    .GroupBy(x => x.Detail_Id)
                                    .Select(x => x.Last());
            document.Replace("%%TglTerbit%%", TanggalDokCetak(result.FirstOrDefault().Id, tanggal).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);
            document.Replace("%%NoKupon%%", result.FirstOrDefault().NoKupon.ToString(), false, true);
            if (result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Id == 1)
            {
                TextSelection textSelection = document.Find("%%Produk%%", true, true);
                WTextRange textRange = textSelection.GetAsOneRange();
                textRange.Text = result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Nama;
                textRange.CharacterFormat.Italic = true;
            }
            else
            {
                TextSelection textSelection = document.Find("%%Produk%%", true, true);
                WTextRange textRange = textSelection.GetAsOneRange();
                textRange.Text = result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Nama;
                textRange.CharacterFormat.Italic = false;
            }
            document.Replace("%%Produk%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Nama, false, true);
            document.Replace("%%Initial%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Produk.Initial, false, true);
            document.Replace("%%NamaProduk%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.NamaProduk, false, true);
            document.Replace("%%Pembayaran%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.JenisPembayaran, false, true);
            document.Replace("%%JenisPembayaran%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.JenisPembayaran, false, true);
            document.Replace("%%Perusahaan%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Nama, false, true);
            if (result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Gedung == null || result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Gedung == "")
            {
                document.Replace("%%Gedung%%", "", false, true);
            }
            else
            {
                document.Replace("%%Gedung%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Gedung, false, true);
            }
            document.Replace("%%Jalan%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Jalan, false, true);
            document.Replace("%%Kota%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perusahaan.Kota, false, true);
            document.Replace("%%Perwakilan%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Nama, false, true);

            if (result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Gender == "Bapak")
            {
                document.Replace("%%Gender1%%", "Bpk.", false, true);
            }
            else
            {
                document.Replace("%%Gender1%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Gender, false, true);
            }

            document.Replace("%%Gender2%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Perwakilan.Gender, false, true);
            if (result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.Jabatan.Jenis == true)
            {
                TextSelection textSelection = document.Find("%%Jabatan%%", true, true);
                WTextRange textRange = textSelection.GetAsOneRange();
                textRange.Text = jabatan;
                textRange.CharacterFormat.Italic = true;
            }
            else
            {
                TextSelection textSelection = document.Find("%%Jabatan%%", true, true);
                WTextRange textRange = textSelection.GetAsOneRange();
                textRange.Text = jabatan;
                textRange.CharacterFormat.Italic = false;
            }
            document.Replace("%%Tugas%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.JenisTugas.Nama, false, true);
            document.Replace("%%Satuan%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.MataUang.Satuan, false, true);
            document.Replace("%%Seri%%", result.FirstOrDefault().TransaksiDetail.KodeEfek, false, true);
            document.Replace("%%Bunga%%", result.FirstOrDefault().TransaksiDetail.Bunga.ToString(), false, true);
            document.Replace("%%Nominal%%", result.FirstOrDefault().TransaksiDetail.Nominal.ToString("N2"), false, true);
            document.Replace("%%TglSediaDana%%", TglDokPenyediaanDana(Id, tanggal).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);
            document.Replace("%%TglBayarBunga%%", TglDokBayarBunga(null, tanggal).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);
            document.Replace("%%matauang%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.MataUang.Nama, false, true);
            document.Replace("%%JenisPengiriman%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.JenisPengiriman, false, true);
            document.Replace("%%NamaBank%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.NamaBank, false, true);
            document.Replace("%%NamaPenerima%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.NamaPenerima, false, true);
            document.Replace("%%NoRekening%%", result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.NoRekening, false, true);
            document.Replace("%%Referensi%%", noref, false, true);

            WSection section = document.Sections[0];
            WTable table = section.Tables[0] as WTable;
            WTableRow row;
            WTableCell cell;
            IWParagraphStyle style = document.AddParagraphStyle("User");
            style.CharacterFormat.FontName = "Arial";
            IWParagraph paragraph = section.AddParagraph();
            decimal total = 0;
            decimal totalPokok = 0;
            decimal jumlah = 0;
            decimal temp = 0;
            foreach (var item in result)
            {
                //Adds the second row into table
                row = table.AddRow(true, false);
                //Adds the first cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(item.TransaksiDetail.Seri.ToString()).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Adds the second cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(item.TransaksiDetail.KodeEfek.ToString()).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                cell = row.AddCell();
                cell.AddParagraph().AppendText(item.TransaksiDetail.Bunga.ToString() + " %").OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                //Adds the third cell into row
                cell = row.AddCell();
                cell.CellFormat.Paddings.Left = 100;
                if (result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.MataUang_Id == 1)
                {
                    cell.AddParagraph().AppendText(item.TransaksiDetail.Nominal.ToString("N", CultureInfo.CreateSpecificCulture("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                    //Adds the forth cell into row
                    cell = row.AddCell();
                    cell.AddParagraph().AppendText(item.NilaiBunga.ToString("N", CultureInfo.CreateSpecificCulture("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                }
                else if (result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.MataUang_Id == 2)
                {
                    cell.AddParagraph().AppendText(item.TransaksiDetail.Nominal.ToString("N2")).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                    //Adds the forth cell into row
                    cell = row.AddCell();
                    cell.AddParagraph().AppendText(item.NilaiBunga.ToString("N2")).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                }
                var lastkupon = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Id == item.TransaksiDetail.Id).ToList();
                if (item.NoKupon == lastkupon.LastOrDefault().NoKupon)
                {
                    temp = temp + item.TransaksiDetail.Nominal;
                    trigger = true;

                }
                totalPokok = totalPokok + item.TransaksiDetail.Nominal;
                total = total + item.NilaiBunga;
                jumlah = total + temp;
            }

            if (result.Count() > 0)
            {   
                row = table.AddRow(true, false);
                //Adds the first cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText("Jumlah").OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                //Adds the second cell into row
                cell = row.AddCell();

                cell = row.AddCell();
                //Adds the third cell into row
                cell = row.AddCell();
                if (result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.MataUang_Id == 1)
                {
                    if (trigger == true)
                    {
                        cell.AddParagraph().AppendText(totalPokok.ToString("N", CultureInfo.CreateSpecificCulture("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        cell = row.AddCell();
                        cell.AddParagraph().AppendText(total.ToString("N", CultureInfo.CreateSpecificCulture("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        document.Replace("%%JumlahPokok%%", totalPokok.ToString("N", CultureInfo.CreateSpecificCulture("id-ID")), false, true);
                        document.Replace("%%JumlahBayar%%", jumlah.ToString("N", CultureInfo.CreateSpecificCulture("id-ID")), false, true);
                        document.Replace("%%terbilang%%", terbilang(jumlah), false, true);
                        document.Replace("%%Lunas%%", "dan Pelunasan Pokok", false, true);

                    }
                    else
                    {
                        cell.AddParagraph().AppendText(totalPokok.ToString("N", CultureInfo.CreateSpecificCulture("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        cell = row.AddCell();
                        cell.AddParagraph().AppendText(total.ToString("N", CultureInfo.CreateSpecificCulture("id-ID"))).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        document.Replace("%%JumlahPokok%%", totalPokok.ToString("N", CultureInfo.CreateSpecificCulture("id-ID")), false, true);
                        document.Replace("%%JumlahBayar%%", total.ToString("N", CultureInfo.CreateSpecificCulture("id-ID")), false, true);
                        document.Replace("%%terbilang%%", terbilang(total), false, true);
                        document.Replace("%%Lunas%%", "", false, true);

                    }



                }
                else if (result.FirstOrDefault().TransaksiDetail.TransaksiLaporan.MataUang_Id == 2)
                {
                    if (trigger == true)
                    {
                        cell.AddParagraph().AppendText(totalPokok.ToString("N2")).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        //Adds the forth cell into row
                        cell = row.AddCell();
                        cell.AddParagraph().AppendText(total.ToString("N2")).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        document.Replace("%%JumlahPokok%%", totalPokok.ToString("N2"), false, true);
                        document.Replace("%%JumlahBayar%%", jumlah.ToString("N2"), false, true);
                        document.Replace("%%terbilang%%", terbilang(jumlah), false, true);
                        document.Replace("%%Lunas%%", " dan Pelunasan Pokok ", false, true);
                    }
                    else
                    {
                        cell.AddParagraph().AppendText(totalPokok.ToString("N2")).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        //Adds the forth cell into row
                        cell = row.AddCell();
                        cell.AddParagraph().AppendText(total.ToString("N2")).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        document.Replace("%%JumlahPokok%%", totalPokok.ToString("N2"), false, true);
                        document.Replace("%%JumlahBayar%%", total.ToString("N2"), false, true);
                        document.Replace("%%terbilang%%", terbilang(total), false, true);
                        document.Replace("%%Lunas%%", " ", false, true);
                    }

                }
                table.ApplyHorizontalMerge(result.Count() + 1, 0, 2);


            }
            else
            {

                document.Replace("%%JumlahPokok%%", totalPokok.ToString("N2"), false, true);
                document.Replace("%%JumlahBayar%%", result.FirstOrDefault().NilaiBunga.ToString("N2"), false, true);
                document.Replace("%%terbilang%%", terbilang(result.FirstOrDefault().NilaiBunga), false, true);



            }
            document.Save("Output.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }

        public void CetakGabung(List<Int64> Id, DateTime tanggal, string noref, Int64 cetakId)
        {
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "TemplateGabung"); //mengambil path lokasi file
            WordDocument document = new WordDocument(_path, FormatType.Doc); // membaca isi file
            var detId = Convert.ToInt64(Id[0]);
            var dcId = _context.DetailCetak.Where(x => x.TransaksiLaporan_Id == detId).Select(x => x.TransaksiCetak_Id).FirstOrDefault();
            //var status = _context.TransaksiCetak.Where(x => x.Id == dcId).FirstOrDefault();
            var trsCetak = _context.TransaksiCetak.Single(x => x.Id == cetakId);
            trsCetak.StatusCetak_Id = 5;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();
            //foreach(var lap in Id)
            //{
            //    var laps = _context.TransaksiLaporan.SingleOrDefault(x => x.Id == lap);
            //    laps.StatusCetak_Id = 2;
            //    _context.Entry(laps).State = EntityState.Modified;
            //    _context.SaveChanges();
            //}
            decimal totalbayar = 0;
            var result = (from lap in _context.TransaksiLaporan
                          .Include(x => x.Perusahaan)
                          .Include(x => x.Perwakilan)
                          .Include(x => x.Jabatan)
                          .Include(x => x.JenisTugas)
                          .Include(x => x.MataUang)
                          .Where(x => x.Id == Id.FirstOrDefault()).ToList()
                          select new ViewDetailCetakVM
                          {
                              Perusahaan_Id = lap.Perusahaan_Id,
                              Perusahaan = lap.Perusahaan.Nama,
                              Gedung = lap.Perusahaan.Gedung,
                              Jalan = lap.Perusahaan.Jalan,
                              Kota = lap.Perusahaan.Kota,
                              NamaPerwakilan_Id = lap.NamaPerwakilan_Id,
                              Perwakilan = lap.Perwakilan.Nama,
                              Penyebut = lap.Perwakilan.Gender,
                              Jabatan_Id = lap.Jabatan_Id,
                              Jabatan = lap.Jabatan.NamaJabatan,
                              JenisKata = lap.Jabatan.Jenis,
                              JenisTugas_Id = lap.JenisTugas_Id,
                              JenisTugas = lap.JenisTugas.Nama,
                              JenisPengiriman = lap.JenisPengiriman,
                              NamaBank = lap.NamaBank,
                              NamaPenerima = lap.NamaPenerima,
                              NoRekening = lap.NoRekening,
                              Pembayaran = lap.JenisPembayaran,
                              Listitem = (from lapp in _context.TransaksiLaporan
                                           .Include(x => x.Produk)
                                           .Where(x => Id.Contains(x.Id)).ToList()
                                          select new Listitem
                                          {
                                              Produk_Id = lapp.Produk_Id,
                                              Produk = lapp.Produk.Nama,
                                              NamaProduk = lapp.NamaProduk,
                                              initProduk = lapp.Produk.Initial,
                                              ListBunga = _context.SubDetailTanggal
                                                          .Include(x => x.TransaksiTanggal)
                                                          .Include(x => x.SubDetailCetak.TransaksiDetail)
                                                          .Where(x => x.SubDetailCetak.TransaksiDetail.TransaksiLaporan.Id == lapp.Id && x.TransaksiTanggal.TglSuratBunga == tanggal)
                                                          .ToList()
                                              //.GroupBy(x => x.TransaksiTanggal.Detail_Id).Select(x => x.FirstOrDefault())
                                          }).ToList()
                          }).ToList();
            //var lastBunga = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.TransaksiLaporan.Id == Id.FirstOrDefault()).LastOrDefault();

            string joinproduk = "";
            string joinproduk2 = "";
            string gabungproduk = "";
            var lastproduk = result.FirstOrDefault().Listitem.Last();

            foreach (var item in result.FirstOrDefault().Listitem)
            {
                string jenisbayar = "";
                string kupon = "";
                string seri = "";
                string produk = item.Produk;
                var produkId = item.Produk_Id;
                string namaproduk = item.NamaProduk;
                string initProduk = item.initProduk;

                if (produkId == 1 || produkId == 2)
                    jenisbayar += "Bunga";

                if (produkId == 3)
                    jenisbayar += "Bagi Hasil";


                var last = item.ListBunga.Last();

                foreach (var item2 in item.ListBunga)
                {
                    kupon = item2.TransaksiTanggal.NoKupon.ToString();
                    if (item2.Equals(last))
                    {
                        seri += item2.SubDetailCetak.TransaksiDetail.Seri;
                    }
                    else
                    {
                        seri += item2.SubDetailCetak.TransaksiDetail.Seri + "-";
                    }
                }

                if (item.Equals(lastproduk))
                {
                    joinproduk += " dan ";
                    joinproduk2 += " dan ";
                    gabungproduk += " dan ";
                }
                gabungproduk += produk;
                joinproduk += jenisbayar + " ke-" + kupon + " " + initProduk + " " + namaproduk + " Seri " + seri;
                joinproduk2 += initProduk + " " + namaproduk + " Seri " + seri;

            }

            document.Replace("%%Initial%%", gabungproduk, false, true);
            document.Replace("%%JoinProduk%%", joinproduk, false, true);
            document.Replace("%%JoinProduk2%%", joinproduk2, false, true);
            document.Replace("%%JoinProduk3%%", joinproduk, false, true);
            document.Replace("%%Produk%%", result.FirstOrDefault().Listitem[0].Produk, false, true);
            document.Replace("%%Pembayaran%%", result.FirstOrDefault().Pembayaran, false, true);
            document.Replace("%%Perusahaan%%", result.FirstOrDefault().Perusahaan, false, true);
            if (result.FirstOrDefault().Gedung == "" || result.FirstOrDefault().Gedung == null)
            {
                document.Replace("%%Gedung%%", "", false, true);

            }
            else
            {
                document.Replace("%%Gedung%%", result.FirstOrDefault().Gedung, false, true);
            }
            document.Replace("%%Jalan%%", result.FirstOrDefault().Jalan, false, true);
            document.Replace("%%Kota%%", result.FirstOrDefault().Kota, false, true);
            document.Replace("%%Perwakilan%%", result.FirstOrDefault().Perwakilan, false, true);
            if (result.FirstOrDefault().Penyebut == "Bapak")
            {
                document.Replace("%%Panggilan1%%", "Bpk.", false, true);
            }
            else
            {
                document.Replace("%%Panggilan1%%", result.FirstOrDefault().Penyebut, false, true);
            }
            document.Replace("%%Panggilan2%%", result.FirstOrDefault().Penyebut, false, true);
            if (result.FirstOrDefault().JenisKata == true)
            {
                TextSelection textSelection = document.Find("%%Jabatan%%", true, true);
                WTextRange textRange = textSelection.GetAsOneRange();
                textRange.Text = result.FirstOrDefault().Jabatan;
                textRange.CharacterFormat.Italic = true;
            }
            else
            {
                TextSelection textSelection = document.Find("%%Jabatan%%", true, true);
                WTextRange textRange = textSelection.GetAsOneRange();
                textRange.Text = result.FirstOrDefault().Jabatan;
                textRange.CharacterFormat.Italic = false;
            }
            document.Replace("%%Tugas%%", result.FirstOrDefault().JenisTugas, false, true);
            document.Replace("%%Satuan%%", result.FirstOrDefault().Listitem.FirstOrDefault().ListBunga.FirstOrDefault().SubDetailCetak.TransaksiDetail.TransaksiLaporan.MataUang.Satuan, false, true);
            //document.Replace("%%Seri%%", result.FirstOrDefault().TransaksiDetail.Seri, false, true);
            //document.Replace("%%Bunga%%", result.FirstOrDefault().TransaksiDetail.Bunga.ToString(), false, true);
            //document.Replace("%%Nominal%%", result.FirstOrDefault().TransaksiDetail.Nominal.ToString("N2"), false, true);
            document.Replace("%%TglSediaDana%%", TglDokPenyediaanDana(null, tanggal).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), false, true);
            document.Replace("%%TglBayarBunga%%", TglDokBayarBunga(null, tanggal).ToString("dd MMMM yyyy", new CultureInfo("id-ID")), false, true);
            document.Replace("%%matauang%%", result.FirstOrDefault().Listitem.FirstOrDefault().ListBunga.FirstOrDefault().SubDetailCetak.TransaksiDetail.TransaksiLaporan.MataUang.Nama, false, true);
            document.Replace("%%JenisPengiriman%%", result.FirstOrDefault().JenisPengiriman, false, true);
            document.Replace("%%NamaBank%%", result.FirstOrDefault().NamaBank, false, true);
            document.Replace("%%NamaPenerima%%", result.FirstOrDefault().NamaPenerima, false, true);
            document.Replace("%%NoRekening%%", result.FirstOrDefault().NoRekening, false, true);
            document.Replace("%%Referensi%%", noref, false, true);

            WSection section = document.Sections[0];
            WTable table = section.Tables[0] as WTable;
            WTableRow row;
            WTableCell cell;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            decimal total = 0;
            decimal totalPokok = 0;
            foreach (var item in result)
            {
                foreach (var item2 in item.Listitem)
                {
                    foreach (var item3 in item2.ListBunga)
                    {

                        //Adds the second row into table
                        row = table.AddRow(true, false);
                        //Adds the 1st cell into row
                        row.Height = 6;
                        cell = row.AddCell();
                        cell.AddParagraph().AppendText(item3.SubDetailCetak.TransaksiDetail.Seri.ToString()).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        //2nd
                        cell = row.AddCell();
                        cell.AddParagraph().AppendText(item3.SubDetailCetak.TransaksiDetail.KodeEfek.ToString()).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Adds the 3rd cell into row
                        if (item2.Produk_Id == 3)
                        {
                            cell = row.AddCell();
                            cell.AddParagraph().AppendText("-").OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        }
                        else
                        {
                            cell = row.AddCell();
                            cell.AddParagraph().AppendText(item3.SubDetailCetak.TransaksiDetail.Bunga.ToString("N2")).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        }
                        //Adds the 4th cell into row
                        cell = row.AddCell();
                        cell.AddParagraph().AppendText(item3.SubDetailCetak.TransaksiDetail.Nominal.ToString("N2", culture)).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;

                        //Adds the 5th cell into row
                        cell = row.AddCell();
                        cell.AddParagraph().AppendText(item3.TransaksiTanggal.NilaiBunga.ToString("N2", culture)).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        totalPokok = totalPokok + item3.SubDetailCetak.TransaksiDetail.Nominal;
                        total = total + item3.TransaksiTanggal.NilaiBunga;
                        totalbayar = total + totalPokok;
                    }
                }
            }
            var count = result[0].Listitem[0].ListBunga.Count() + result[0].Listitem[1].ListBunga.Count();


            row = table.AddRow(true, false);
            //Adds the first cell into row
            cell = row.AddCell();
            cell.AddParagraph().AppendText("Jumlah").OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
            //Adds the second cell into row
            cell = row.AddCell();
            //Adds the third cell into row
            cell = row.AddCell();
            //4th
            cell = row.AddCell();
            cell.AddParagraph().AppendText(totalPokok.ToString("N2", culture)).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right; ;
            //Adds the 5th cell into row
            cell = row.AddCell();
            cell.AddParagraph().AppendText(total.ToString("N2", culture)).OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            //if(result.FirstOrDefault().Listitem.FirstOrDefault().ListBunga.FirstOrDefault().TransaksiTanggal.NoKupon == lastBunga.NoKupon)
            //{
            //    document.Replace("%%JumlahBayar%%", totalbayar.ToString("N2", culture), false, true);
            //    document.Replace("%%terbilang%%", terbilang(totalbayar), false, true);
            //    document.Replace("%%JumlahPokok%%", totalPokok.ToString("N2", culture), false, true);
            //}
            //else
            //{

            document.Replace("%%JumlahBayar%%", total.ToString("N2", culture), false, true);
            document.Replace("%%terbilang%%", terbilang(total), false, true);
            document.Replace("%%JumlahPokok%%", totalPokok.ToString("N2", culture), false, true);

            //}
            table.ApplyHorizontalMerge(count + 1, 0, 2);

            document.Save("Output.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }

        public ActionResult DetailCetak(Int64 Id, DateTime tglTempo)
        {
            var view = new ViewDetailCetakVM();
            var Cetak = _context.DetailCetak.Where(x => x.TransaksiCetak_Id == Id).Select(x => x.TransaksiLaporan_Id).ToList();
            var trId = Convert.ToInt64(Cetak[0]);
            var date = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Trans_Id == trId).FirstOrDefault();
            var laporan = (from lap in _context.TransaksiLaporan
                          .Include(x => x.Perusahaan)
                          .Include(x => x.Perwakilan)
                          .Include(x => x.Jabatan)
                          .Include(x => x.JenisTugas)
                          .Include(x => x.StatusCetak)
                          .Where(x => x.Id == Cetak.FirstOrDefault()).ToList()
                           select new ViewDetailCetakVM
                           {
                               TanggalBunga = TanggalSuratTerdekat(lap.Id),
                               StatusCetak = lap.StatusCetak_Id,
                               Perusahaan_Id = lap.Perusahaan_Id,
                               Perusahaan = lap.Perusahaan.Nama,
                               Jalan = lap.Perusahaan.Jalan,
                               NamaPerwakilan_Id = lap.NamaPerwakilan_Id,
                               Perwakilan = lap.Perwakilan.Nama,
                               Jabatan_Id = lap.Jabatan_Id,
                               Jabatan = lap.Jabatan.NamaJabatan,
                               JenisTugas_Id = lap.JenisTugas_Id,
                               JenisTugas = lap.JenisTugas.Nama,
                               JenisPengiriman = lap.JenisPengiriman,
                               NamaBank = lap.NamaBank,
                               NamaPenerima = lap.NamaPenerima,
                               NoRekening = lap.NoRekening,
                               Listitem = (from lapp in _context.TransaksiLaporan
                                            .Include(x => x.Produk)
                                            .Where(x => Cetak.Contains(x.Id)).ToList()
                                           select new Listitem
                                           {
                                               Id = lapp.Id,
                                               Produk_Id = lapp.Produk_Id,
                                               Produk = lapp.Produk.Nama,
                                               NamaProduk = lapp.NamaProduk,
                                               ListBunga = _context.SubDetailTanggal
                                                          .Include(x => x.TransaksiTanggal)
                                                          .Include(x => x.SubDetailCetak.TransaksiDetail)
                                                          .Include(x => x.SubDetailCetak.DetailCetak)
                                                          .Include(x => x.SubDetailCetak.DetailCetak.TransaksiCetak)
                                                          .Include(x => x.SubDetailCetak.DetailCetak.TransaksiCetak.StatusCetak)
                                                          .Where(x => x.SubDetailCetak.TransaksiDetail.TransaksiLaporan.Id == lapp.Id && x.TransaksiTanggal.TglSuratBunga == tglTempo)
                                                          .ToList()
                                               //.GroupBy(x => x.TransaksiTanggal.TglSuratBunga).Select(x => x.FirstOrDefault())
                                           }).ToList()
                           }).ToList();

            return View("DetailCetak", laporan);
        }

        public ActionResult DetailOtorisasi(Int64 Id)
        {
            var view = new ViewDetailCetakVM();
            var Cetak = _context.DetailCetak.Where(x => x.TransaksiCetak_Id == Id).Select(x => x.TransaksiLaporan_Id).ToList();
            var trId = Convert.ToInt64(Cetak[0]);
            var date = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Trans_Id == trId).FirstOrDefault();
            var laporan = (from lap in _context.TransaksiLaporan
                          .Include(x => x.Perusahaan)
                          .Include(x => x.Perwakilan)
                          .Include(x => x.Jabatan)
                          .Include(x => x.JenisTugas)
                          .Where(x => x.Id == Cetak.FirstOrDefault()).ToList()
                           select new ViewDetailCetakVM
                           {
                               TanggalBunga = TanggalSuratTerdekat(lap.Id),
                               Perusahaan_Id = lap.Perusahaan_Id,
                               Perusahaan = lap.Perusahaan.Nama,
                               Jalan = lap.Perusahaan.Jalan,
                               NamaPerwakilan_Id = lap.NamaPerwakilan_Id,
                               Perwakilan = lap.Perwakilan.Nama,
                               Jabatan_Id = lap.Jabatan_Id,
                               Jabatan = lap.Jabatan.NamaJabatan,
                               JenisTugas_Id = lap.JenisTugas_Id,
                               JenisTugas = lap.JenisTugas.Nama,
                               JenisPengiriman = lap.JenisPengiriman,
                               NamaBank = lap.NamaBank,
                               NamaPenerima = lap.NamaPenerima,
                               NoRekening = lap.NoRekening,
                               Listitem = (from lapp in _context.TransaksiLaporan
                                            .Include(x => x.Produk)
                                            .Where(x => Cetak.Contains(x.Id)).ToList()
                                           select new Listitem
                                           {
                                               Produk_Id = lapp.Produk_Id,
                                               Produk = lapp.Produk.Nama,
                                               NamaProduk = lapp.NamaProduk,
                                               ListBunga = _context.SubDetailTanggal
                                                          .Include(x => x.TransaksiTanggal)
                                                          .Include(x => x.SubDetailCetak)
                                                          .Include(x => x.SubDetailCetak.TransaksiDetail)
                                                          .Where(x => x.SubDetailCetak.TransaksiDetail.TransaksiLaporan.Id == lapp.Id && x.TransaksiTanggal.Status == false)
                                                          .ToList()

                                           }).ToList()
                           }).ToList();

            return View("DetailOtorisasi", laporan);
        }


        public ActionResult JoinSurat(List<Int64> Id, DateTime tanggal)
        {
            var view = new ViewJoinVM();

            var laporan = (from lap in _context.TransaksiLaporan
                          .Include(x => x.Perusahaan)
                          .Include(x => x.Perwakilan)
                          .Include(x => x.Jabatan)
                          .Include(x => x.JenisTugas)
                          .Where(x => x.Id == Id.FirstOrDefault()).ToList()
                           select new ViewJoinVM
                           {
                               TanggalBunga = TanggalSuratTerdekat(lap.Id),
                               Perusahaan_Id = lap.Perusahaan_Id,
                               Perusahaan = lap.Perusahaan.Nama,
                               Jalan = lap.Perusahaan.Jalan,
                               NamaPerwakilan_Id = lap.NamaPerwakilan_Id,
                               Perwakilan = lap.Perwakilan.Nama,
                               Jabatan_Id = lap.Jabatan_Id,
                               Jabatan = lap.Jabatan.NamaJabatan,
                               JenisTugas_Id = lap.JenisTugas_Id,
                               JenisTugas = lap.JenisTugas.Nama,
                               JenisPengiriman = lap.JenisPengiriman,
                               NamaBank = lap.NamaBank,
                               NamaPenerima = lap.NamaPenerima,
                               NoRekening = lap.NoRekening,
                               ListProduk = (from lapp in _context.TransaksiLaporan
                                            .Include(x => x.Produk)
                                            .Where(x => Id.Contains(x.Id)).ToList()
                                             select new ListProduk
                                             {
                                                 Produk_Id = lapp.Produk_Id,
                                                 Produk = lapp.Produk.Nama,
                                                 NamaProduk = lapp.NamaProduk,
                                                 ListBunga = _context.TransaksiTanggal
                                                            .Include(x => x.TransaksiDetail)
                                                            .Where(x => x.TransaksiDetail.TransaksiLaporan.Id == lapp.Id && x.TglSuratBunga == tanggal && x.Status == false)
                                                            .ToList()
                                                            .GroupBy(x => x.Detail_Id).Select(x => x.FirstOrDefault())
                                             }).ToList()
                           }).ToList();

            return View("ViewJoin", laporan);
        }

        public JsonResult GetListCheck()
        {
            var result = (from ctk in _context.TransaksiCetak
                          .Include(x => x.StatusCetak)
                          .Where(x => x.StatusCetak_Id == 7).ToList()
                          select new
                          {
                              Id = ctk.Id,
                              ctk.StatusCetak,
                              ctk.TglSurat,
                              TglJatuhTempo = TglDokBayarBunga(null, ctk.TglJatuhTempo),
                              Detail = (from det in _context.DetailCetak
                                        .Include(x => x.TransaksiLaporan.Perusahaan)
                                        .Include(x => x.TransaksiLaporan.Produk)
                                        .Include(x => x.TransaksiLaporan)
                                        .Where(x => x.TransaksiCetak_Id == ctk.Id).ToList()
                                        select new
                                        {
                                            Id = det.Id,
                                            Trans_Id = det.TransaksiLaporan.Id,
                                            Perusahaan = det.TransaksiLaporan.Perusahaan.Nama,
                                            Produk = det.TransaksiLaporan.Produk.Nama,
                                            NamaProduk = det.TransaksiLaporan.NamaProduk
                                        }).ToList()
                          }).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListApproval()
        {
            var result = (from ctk in _context.TransaksiCetak
                          .Include(x => x.StatusCetak)
                          .Where(x => x.StatusCetak_Id == 3).ToList()
                          select new
                          {
                              Id = ctk.Id,
                              ctk.StatusCetak,
                              ctk.TglSurat,
                              TglJatuhTempo = TglDokBayarBunga(null, ctk.TglJatuhTempo),
                              Detail = (from det in _context.DetailCetak
                                        .Include(x => x.TransaksiLaporan.Perusahaan)
                                        .Include(x => x.TransaksiLaporan.Produk)
                                        .Include(x => x.TransaksiLaporan)
                                        .Where(x => x.TransaksiCetak_Id == ctk.Id).ToList()
                                        select new
                                        {
                                            Id = det.Id,
                                            Trans_Id = det.TransaksiLaporan.Id,
                                            Perusahaan = det.TransaksiLaporan.Perusahaan.Nama,
                                            Produk = det.TransaksiLaporan.Produk.Nama,
                                            NamaProduk = det.TransaksiLaporan.NamaProduk
                                        }).ToList()
                          }).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistory()
        {
            var result = (from ctk in _context.TransaksiCetak
                          .Include(x => x.StatusCetak)
                          .Where(x => x.StatusCetak_Id == 3 || x.StatusCetak_Id == 4 || x.StatusCetak_Id == 5 || x.StatusCetak_Id == 6 || x.StatusCetak_Id == 7).ToList()
                          select new
                          {
                              Id = ctk.Id,
                              ctk.StatusCetak,
                              ctk.TglSurat,
                              ctk.TglJatuhTempo,
                              tgltempo = TglDokBayarBunga(null, ctk.TglJatuhTempo),
                              Detail = (from det in _context.DetailCetak
                                        .Include(x => x.TransaksiLaporan.Perusahaan)
                                        .Include(x => x.TransaksiLaporan.Produk)
                                        .Include(x => x.TransaksiLaporan)
                                        .Where(x => x.TransaksiCetak_Id == ctk.Id).ToList()
                                        select new
                                        {
                                            Id = det.Id,
                                            Trans_Id = det.TransaksiLaporan.Id,
                                            Perusahaan = det.TransaksiLaporan.Perusahaan.Nama,
                                            Produk = det.TransaksiLaporan.Produk.Nama,
                                            NamaProduk = det.TransaksiLaporan.NamaProduk
                                        }).ToList()
                          }).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Approval(Int64 Id, bool StatusCetak)
        {
            var lap = new TransaksiLaporan();
            var ctk = new TransaksiCetak();
            if (StatusCetak == true)
            {
                ctk = _context.TransaksiCetak.Single(x => x.Id == Id);
                ctk.StatusCetak_Id = 4;
            }
            else
            {
                ctk = _context.TransaksiCetak.Single(x => x.Id == Id);
                ctk.StatusCetak_Id = 6;
            }

            _context.Entry(ctk).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Checking(Int64 Id, bool StatusCetak)
        {
            var lap = new TransaksiLaporan();
            var ctk = new TransaksiCetak();
            if (StatusCetak == true)
            {
                ctk = _context.TransaksiCetak.Single(x => x.Id == Id);
                ctk.StatusCetak_Id = 3;
            }
            else
            {
                ctk = _context.TransaksiCetak.Single(x => x.Id == Id);
                ctk.StatusCetak_Id = 6;
            }

            _context.Entry(ctk).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult StatusPembayaran(Int64 Id, bool StatusCetak)
        {
            var tgl = new TransaksiTanggal();
            tgl = _context.TransaksiTanggal.Where(x => x.Id == Id && x.Status == false).FirstOrDefault();
            tgl.Status = true;

            _context.Entry(tgl).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Approve(Int64 Id, TransaksiCetak TransaksiCetak, DetailCetak DetailCetak)
        {

            var dbLap = _context.TransaksiLaporan.Where(x => x.Id == Id).FirstOrDefault();
            var dbTgl = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Trans_Id == Id).FirstOrDefault();
            TransaksiCetak.StatusCetak_Id = dbLap.StatusCetak_Id;
            TransaksiCetak.Status = dbTgl.Status;
            TransaksiCetak.TglSurat = TanggalCetak(dbLap.Id);
            _context.TransaksiCetak.Add(TransaksiCetak);
            _context.SaveChanges();

            var CetakId = TransaksiCetak.Id;
            DetailCetak.TransaksiCetak_Id = CetakId;
            DetailCetak.TransaksiLaporan_Id = dbLap.Id;
            _context.DetailCetak.Add(DetailCetak);
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SendOtorisasiGabungan(List<Int64> Id, bool StatusCetak)
        {
            var TransaksiCetak = new TransaksiCetak();

            var lap = new TransaksiLaporan();
            var trId = Convert.ToInt64(Id[0]);
            var trlap = _context.TransaksiLaporan.Where(x => x.Id == trId).FirstOrDefault();
            var trTgl = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Trans_Id == trId).FirstOrDefault();
            if (StatusCetak == true)
            {
                TransaksiCetak.StatusCetak_Id = 7;
                TransaksiCetak.Status = trTgl.Status;
                TransaksiCetak.TglSurat = TanggalCetak(trId);
                TransaksiCetak.TglJatuhTempo = TanggalSuratTerdekat(trId);
                _context.TransaksiCetak.Add(TransaksiCetak);
                _context.SaveChanges();

                var CetakId = TransaksiCetak.Id;

                foreach (var item in Id)
                {
                    var DetailCetak = new DetailCetak();
                    lap = _context.TransaksiLaporan.Single(x => x.Id == item);
                    lap.StatusCetak_Id = 7;
                    var dbTgl = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Trans_Id == item).FirstOrDefault();
                    DetailCetak.TransaksiCetak_Id = CetakId;
                    DetailCetak.TransaksiLaporan_Id = lap.Id;
                    _context.DetailCetak.Add(DetailCetak);
                    _context.SaveChanges();
                    var detailtrans = _context.TransaksiDetail.Where(x => x.Trans_Id == lap.Id).ToList();
                    foreach (var item2 in detailtrans)
                    {
                        var subDetail = new SubDetailCetak();
                        subDetail.DetailCetak_Id = DetailCetak.Id;
                        subDetail.TransDet_Id = item2.Id;
                        _context.SubDetailCetak.Add(subDetail);
                        _context.SaveChanges();
                        var transTgl = _context.TransaksiTanggal.Where(x => x.Detail_Id == item2.Id && x.Status == false).FirstOrDefault();
                        var subDetailTgl = new SubDetailTanggal();
                        if (_context.TransaksiTanggal.Where(x => x.Detail_Id == item2.Id && x.Status == false).Count() == 0)
                        {

                        }
                        else
                        {
                            subDetailTgl.SubDetail_Id = subDetail.Id;
                            subDetailTgl.TransTanggal_Id = transTgl.Id;
                            _context.SubDetailTanggal.Add(subDetailTgl);
                        }
                        _context.SaveChanges();
                    }


                }
            }

            _context.Entry(lap).State = EntityState.Modified;
            _context.SaveChanges();

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendOtorisasi(Int64 Id, bool StatusCetak)
        {
            var TransaksiCetak = new TransaksiCetak();
            var DetailCetak = new DetailCetak();
            var dbLap = _context.TransaksiLaporan.Where(x => x.Id == Id).FirstOrDefault();
            var dbTgl = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Trans_Id == Id).FirstOrDefault();
            var lap = new TransaksiLaporan();
            if (StatusCetak == true)
            {
                lap = _context.TransaksiLaporan.Single(x => x.Id == Id);

                lap.StatusCetak_Id = 7;
                TransaksiCetak.StatusCetak_Id = dbLap.StatusCetak_Id;
                TransaksiCetak.Status = dbTgl.Status;
                TransaksiCetak.TglSurat = TanggalCetak(dbLap.Id);
                TransaksiCetak.TglJatuhTempo = TanggalSuratTerdekat(dbLap.Id);
                _context.TransaksiCetak.Add(TransaksiCetak);
                _context.SaveChanges();

                var CetakId = TransaksiCetak.Id;
                DetailCetak.TransaksiCetak_Id = CetakId;
                DetailCetak.TransaksiLaporan_Id = lap.Id;
                _context.DetailCetak.Add(DetailCetak);
                _context.SaveChanges();
                var detalctk = _context.DetailCetak.Where(x => x.TransaksiCetak_Id == TransaksiCetak.Id);
                var detailtrans = _context.TransaksiDetail.Where(x => x.Trans_Id == Id).ToList();
                foreach (var item in detailtrans)
                {
                    var subDetail = new SubDetailCetak();
                    subDetail.DetailCetak_Id = DetailCetak.Id;
                    subDetail.TransDet_Id = item.Id;
                    _context.SubDetailCetak.Add(subDetail);
                    _context.SaveChanges();
                    var subDetailTgl = new SubDetailTanggal();
                    var transTgl = _context.TransaksiTanggal.Where(x => x.Detail_Id == item.Id && x.Status == false).FirstOrDefault();
                    if (transTgl == null)
                    {
                        continue;
                    }
                    subDetailTgl.SubDetail_Id = subDetail.Id;
                    subDetailTgl.TransTanggal_Id = transTgl.Id;
                    _context.SubDetailTanggal.Add(subDetailTgl);
                    _context.SaveChanges();
                }
            }

            _context.Entry(lap).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Lunas(List<Int64> Id)
        {

            var tr = new TransaksiLaporan();
            foreach (var item in Id)
            {
                tr = _context.TransaksiLaporan.Where(x => x.Id == item).FirstOrDefault();
                var kupon = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Trans_Id == item && x.Status == false).Select(x => x.NoKupon).FirstOrDefault();
                var db = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Trans_Id == item && x.NoKupon == kupon && x.Status == false).ToList();
                foreach (var i in db)
                {
                    i.Status = true;
                    _context.Entry(i).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                var count = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.Trans_Id == item && x.Status == false).Count();
                if (count > 1)
                {
                    tr.StatusCetak_Id = 1;
                }
                else
                {
                    tr.StatusCetak_Id = 6;
                }
            }
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private bool IsHoliday(DateTime date)
        {
            return _context.HariLibur.Select(x => x.TanggalLibur).Contains(date);
        }

        private bool IsWeekEnd(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;
        }

        //public DateTime TglDokBayarBunga(Int64 Id)
        //{
        //    var date = _context.TransaksiTanggal.Where(x => x.Id == Id && x.Status == false).Select(x => x.TglSuratBunga).FirstOrDefault();

        //    do
        //    {
        //        date = date.AddDays(1);
        //    } while (IsHoliday(date) || IsWeekEnd(date));

        //    return date;
        //}

        public DateTime TglDokBayarBunga(Int64? Id, DateTime? tanggal)
        {
            var date = _context.TransaksiTanggal.Where(x => x.Id == Id && x.Status == false).Select(x => x.TglSuratBunga).FirstOrDefault();

            if (tanggal != null)
            {
                date = tanggal ?? DateTime.Now;
            }

            if (IsHoliday(date) || IsWeekEnd(date))
            {
                do
                {
                    date = date.AddDays(1);
                }
                while (IsHoliday(date) || IsWeekEnd(date));
            }
            return date;
        }

        public DateTime TglDokPenyediaanDana(Int64? Id, DateTime? tanggal)
        {
            var date = _context.TransaksiTanggal.Where(x => x.Id == Id && x.Status == false).Select(x => x.TglSuratBunga).FirstOrDefault();

            if (tanggal != null)
            {
                date = tanggal ?? DateTime.Now;
            }

            do
            {
                date = date.AddDays(-1);
            } while (IsHoliday(date) || IsWeekEnd(date));

            return date;
        }

        public DateTime TanggalDokCetak(Int64? Id, DateTime? tanggal)
        {
            var date = _context.TransaksiTanggal.Where(x => x.Id == Id && x.TglSuratBunga == tanggal).Select(x => x.TglSuratBunga).FirstOrDefault();

            if (tanggal != null)
            {
                date = tanggal ?? DateTime.Now;
            }

            date = date.AddDays(-7);
            if (IsHoliday(date) || IsWeekEnd(date))
            {
                do
                {
                    date = date.AddDays(-1);
                } while (IsHoliday(date) || IsWeekEnd(date));
            }

            return date;
        }

        public DateTime TanggalCetak(Int64 Id)
        {
            var date = DateTime.Now;
            count = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.TransaksiLaporan.Id == Id && x.Status == false).Select(x => x.TglSuratBunga).Count();
            if (count == 0)
            {
                date = DateTime.Now.AddDays(1);
            }
            else
            {
                date = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.TransaksiLaporan.Id == Id && x.Status == false).Select(x => x.TglSuratBunga).FirstOrDefault();
            }

            date = date.AddDays(-15);
            if (IsHoliday(date) || IsWeekEnd(date))
            {
                do
                {
                    date = date.AddDays(-1);
                } while (IsHoliday(date) || IsWeekEnd(date));
            }

            return date;
        }

        public DateTime TanggalSuratTerdekat(Int64 Id)
        {
            var result = _context.TransaksiTanggal.Where(x => x.TransaksiDetail.TransaksiLaporan.Id == Id && x.Status == false).Select(x => x.TglSuratBunga).FirstOrDefault();
            return result;
        }

        public static string terbilang(decimal angka)
        {
            string[] bilangan = new string[] {"",
            "Satu",
            "Dua",
            "Tiga",
            "Empat",
            "Lima",
            "Enam",
            "Tujuh",
            "Delapan",
            "Sembilan",
            "Sepuluh",
            "Sebelas"};
            decimal hasil_bagi = 0;
            decimal hasil_mod = 0;

            if (angka < 12)
            {
                return bilangan[(int)angka];
            }
            else if (angka < 20)
            {
                return bilangan[(int)angka - 10] + " Belas ";
            }
            else if (angka < 100)
            {
                hasil_mod = angka % 10;
                hasil_bagi = (int)(angka / 10);
                return terbilang(hasil_bagi) + " Puluh " + terbilang(hasil_mod);
            }
            else if (angka < 200)
            {
                return "Seratus " + terbilang(angka - 100);
            }
            else if (angka < 1000)
            {
                hasil_mod = angka % 100;
                hasil_bagi = (int)(angka / 100);
                return terbilang(hasil_bagi) + " Ratus " + terbilang(hasil_mod);
            }
            else if (angka < 2000)
            {
                return " Seribu " + terbilang(angka - 1000);
            }
            else if (angka < 1000000)
            {
                hasil_mod = angka % 1000;
                hasil_bagi = (int)(angka / 1000);
                return terbilang(hasil_bagi) + " Ribu " + terbilang(hasil_mod);
            }
            else if (angka < 1000000000)
            {
                hasil_mod = angka % 1000000;
                hasil_bagi = (int)(angka / 1000000);
                return terbilang(hasil_bagi) + " Juta " + terbilang(hasil_mod);
            }
            else if (angka < 1000000000000)
            {
                hasil_mod = angka % 1000000000;
                hasil_bagi = (int)(angka / 1000000000);
                return terbilang(hasil_bagi) + " Milyar " + terbilang(hasil_mod);
            }
            else if (angka < 1000000000000000)
            {
                hasil_bagi = (int)(angka / 1000000000000);
                hasil_mod = angka % 1000000000000;
                return terbilang(hasil_bagi) + " Triliyun " + terbilang(hasil_mod);
            }
            else
            {
                return "Wooow...";
            }


        }

        public ActionResult Index_OJK()
        {
            return View();
        }


    }

    
}