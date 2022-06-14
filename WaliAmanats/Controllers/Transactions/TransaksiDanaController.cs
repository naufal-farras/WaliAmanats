using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.Models.Transaksi;
using WaliAmanats.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO; 
using System.Globalization;
using System.Text.RegularExpressions;

namespace WaliAmanats.Controllers.Transactions
{
    public class TransaksiDanaController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        // GET: TransaksiDana
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CetakDana()
        {
            return View();
        }
        public ActionResult ViewDana(Int64 Id)
        {
            var cetak = _context.TransaksiPenggunaanDana.Include(x => x.Perusahaan).Where(x => x.Id == Id).Select(x => x.Perusahaan_Id).ToList();
            var det = _context.TransaksiPenggunaanDana.Include(x => x.Perusahaan).Where(x => x.Id == Id).FirstOrDefault();
            var result = (from emiten in _context.Perusahaan
                          .Where(x => x.Id == cetak.FirstOrDefault()).ToList()
                              // where emiten.Id == Id
                          select new ViewDanaVM
                          {
                              Iddana = det.Id,
                              IdPerusahaan = emiten.Id,
                              NamaPerusahaan = emiten.Nama,
                              //Perusahaan = _context.Perusahaan.FirstOrDefault(x => x.Id == emiten.Id),
                              TransaksiPenggunaanDana = _context.TransaksiPenggunaanDana.Where(x => x.Perusahaan_Id == emiten.Id).FirstOrDefault(),
                              ProdukVM = (from produk in _context.TransaksiLaporan.Include(x => x.Produk).Where(x => x.Perusahaan_Id == emiten.Id && x.isDelete == false).ToList()
                                              //where produk.isDelete == false
                                          select new ProdukVM
                                          {
                                              IdLaporan = produk.Id,
                                              IdProduk = produk.Produk.Id,
                                              Produk = produk.Produk.Nama,
                                              NamaProduk = produk.NamaProduk,
                                              //  TransaksiDetail = _context.TransaksiDetail.Include(x => x.TransaksiLaporan).Where(x => x.Trans_Id == produk.Id).ToList()
                                              DetailProdukVM = (from detail in _context.TransaksiDetail.Include(x => x.TransaksiLaporan).Where(x => x.Trans_Id == produk.Id).ToList()
                                                                select new DetailProdukVM
                                                                {
                                                                    //TransaksiDetail = _context.TransaksiDetail.Include(x => x.)
                                                                    IdDetail = detail.Id,
                                                                    Nominal = detail.Nominal,
                                                                    Seri = detail.Seri
                                                                }
                                              ).ToList()
                                          }
                                          ).ToList()
                          }

                ).ToList();
            return View("ViewDana", result);
        }
        public JsonResult GetAll()
        {
            var result = _context.TransaksiPenggunaanDana.Include(x => x.Perusahaan).Include(x => x.StatusCetak).ToList();

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }
        public JsonResult GetDanaCetak()
        {
            var result = _context.TransaksiPenggunaanDana.Include(x => x.Perusahaan)
            .Include(x => x.StatusCetak).Where(x => x.TanggalCetak.Year == DateTime.Now.Year
            && x.TanggalCetak.Month == DateTime.Now.Month && x.TanggalCetak.Day == DateTime.Now.Day).ToList();

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }
        public JsonResult GetDanaCetakAll()
        {
            var result = _context.TransaksiPenggunaanDana.Include(x => x.Perusahaan)
            .Include(x => x.StatusCetak).ToList();

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }
        public JsonResult GetDetail(Int64 detail)
        {
            TransaksiDetail result = new TransaksiDetail();
            var data = _context.TransaksiDetail.Include(x => x.TransaksiLaporan).FirstOrDefault(x => x.Trans_Id == detail);
            if (data != null)
            {
                result = data;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetById(Int64 Id)
        {
            var result = _context.TransaksiPenggunaanDana.Include(x => x.Perusahaan).Where(x => x.Id == Id).SingleOrDefault();
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
                        var detail = _context.TransaksiPenggunaanDana.Include(x => x.Perusahaan).Include(x => x.StatusCetak)
                            .SingleOrDefault(x => x.Id == idDetail);
                        if (detail.Path != null)
                        {
                            string paths = Server.MapPath("~/Files/Dana/" + detail.Path);
                            System.IO.File.Delete(paths);
                        }

                        var ext = Path.GetExtension(file.FileName);

                        var pathfile = detail.Id + "-" + detail.Perusahaan.Nama + "" + ext;
                        string path = Server.MapPath("~//Files/Dana/" + pathfile);
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
        public JsonResult GetDataProduk()
        {
            var result = _context.Produk.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void CetakDanaa(Int64 Idc, DateTime Tanggal, Int64 Produk, Int64 Id)
        {
            //--simpan variable global
            string NamaGedung = "";
            string Gedung = "";
            string Jalan = "";
            string Kota = "";
            string Gender = "";
            string NamaPerwakilan = "";


            string Jabatan = "";
            string NamaAgen = "";
            string Produk1 = "";
           // string MataUang = "";

            string Initial = "";
            var result = (from data in _context.TransaksiLaporan
                         .Include(x => x.Perusahaan)
                         .Include(x => x.Perwakilan)
                         .Include(x => x.Jabatan)
                         .Include(x => x.JenisTugas)
                         .Include(x => x.Produk)
                         .Include(x => x.MataUang)
                         .Where(x => x.Perusahaan_Id == Id && x.Produk_Id == Produk && x.isDelete == false).ToList()
                          select new
                          {
                              Produk = data.Produk.Nama,
                              NamaProduk = data.NamaProduk,
                              Initial = data.Produk.Initial,

                              MataUang = data.MataUang.Nama,
                              Nama_Perusahaan = data.Perusahaan.Nama,
                              Gedung = data.Perusahaan.Gedung,
                              Jalan = data.Perusahaan.Jalan,
                              Kota = data.Perusahaan.Kota,


                              NamaPerwakilan = data.Perwakilan.Nama,
                              Gender = data.Perwakilan.Gender,

                              NamaJabatan = data.Jabatan.NamaJabatan,
                              JenisTugas = data.JenisTugas.Nama,

                              NominalOutStanding = Outstanding(data.Id)

                          }).ToList();

            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "TemplateDana");
            WordDocument document = new WordDocument(_path, FormatType.Docx);
            var trsCetak = _context.TransaksiPenggunaanDana.Single(x => x.Id == Idc);
            trsCetak.StatusCetak_Id = 5;
            //_context.Entry(Lap).State = EntityState.Modified;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();
            if (result.Count != 0)
            {
                NamaGedung = result.FirstOrDefault().Nama_Perusahaan;
                Gedung = result.FirstOrDefault().Gedung;
                Jalan = result.FirstOrDefault().Jalan;
                Kota = result.FirstOrDefault().Kota;
                Gender = result.FirstOrDefault().Gender;
                NamaPerwakilan = result.FirstOrDefault().NamaPerwakilan;
                Jabatan = result.FirstOrDefault().NamaJabatan;
                NamaAgen = result.FirstOrDefault().JenisTugas;
                Produk1 = result.FirstOrDefault().Produk;
                Initial = result.FirstOrDefault().Initial;

            }

            document.Replace("%%Perusahaan%%", NamaGedung, false, true);
            if(Gedung == null)
            {
                document.Replace("%%Gedung%%", " ", false, true);
            }
            else
            {
                document.Replace("%%Gedung%%", Gedung, false, true);
            }
            
            document.Replace("%%Jalan%%", Jalan, false, true);
            document.Replace("%%Kota%%", Kota, false, true);
            if (Gender == "Bapak")
            {
                document.Replace("%%Gender%%", "Bpk.", false, true);
            }
            else
            {
                document.Replace("%%Gender%%",Gender, false, true);
            }
           // document.Replace("%%Gender%%", Gender, false, true);
            document.Replace("%%NamaPerwakilan%%", NamaPerwakilan, false, true);
            document.Replace("%%Produk%%", Produk1, false, true);

            document.Replace("%%Jabatan%%", Jabatan, false, true);
            document.Replace("%%Initial%%", Initial, false, true);
            document.Replace("%%JenisTugas%%", NamaAgen, false, true);

            



            string Cetak = "";
            foreach (var item in result)
            {
                if (item.MataUang == "Rupiah")
                {
                    Cetak += " • " + item.Initial + " " + item.NamaProduk + " " + "sebesar" + " " + "Rp." + item.NominalOutStanding.ToString("N2") + "\n";
                }else if(item.MataUang == "US Dollar")
                {
                    Cetak += " • " + item.Initial + " " + item.NamaProduk + " " + "sebesar" + " " + "USD" + " " + item.NominalOutStanding.ToString("N2") + "\n";
                }
                

            }
            document.Replace("%%Cetak%%", Cetak, false, true);

            document.Save("LaporanPenggunaanDana.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }

        public Decimal Outstanding(Int64 Id)
        {
            decimal total = 0;
            var data = _context.TransaksiDetail.Where(x => x.Trans_Id == Id);
            foreach (var item in data)
            {
                total += item.Nominal;
            }
            return total;
        }

        public void CetakDanaGabung(Int64 Idc, Int64 Id, String index)
        {

         
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "TemplateDanaGabung");
            WordDocument document = new WordDocument(_path, FormatType.Docx);
            var trsCetak = _context.TransaksiPenggunaanDana.Single(x => x.Id == Idc);
            trsCetak.StatusCetak_Id = 5;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();

            int increment = 0;
            var data = (from lap in _context.TransaksiLaporan.Include(x => x.Produk).Include(x => x.MataUang)
                        .Include(x => x.Perusahaan).Include(x => x.Perwakilan).Include(x => x.Jabatan)
                        .Include(x => x.JenisTugas).Where(x => x.isDelete == false && x.Perusahaan_Id == Id).ToList()
                        select new
                        {
                              MataUang = lap.MataUang.Nama,
                            Index = increment++,
                            NamaPerusahaan = lap.Perusahaan.Nama,
                            NamaProduk = lap.NamaProduk,
                            Produk = lap.Produk.Nama,
                            Initial = lap.Produk.Initial,
                            NominalOutstanding = Outstanding(lap.Id),
                            Kota = lap.Perusahaan.Kota,
                            Jalan = lap.Perusahaan.Jalan,
                            Gedung = lap.Perusahaan.Gedung,
                            Gender = lap.Perwakilan.Gender,
                            Perwakilan = lap.Perwakilan.Nama,
                            Jabatan = lap.Jabatan.NamaJabatan,
                            Tugas = lap.JenisTugas.Nama
                        }
                        ).ToList();

            string pattern = @"[0-9]+(,[0-9]+)*";
            string input = index;
            Match m = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                string indexx = m.Value;
                var list = indexx.Split(',').ToList();

                data = data.Where(x => list.Contains(x.Index.ToString())).ToList();

            }

            document.Replace("%%Produk%%", data.FirstOrDefault().Produk.ToString(), false, true);

            document.Replace("%%Perusahaan%%", data.FirstOrDefault().NamaPerusahaan.ToString(), false, true);
            document.Replace("%%NamaPerwakilan%%", data.FirstOrDefault().Perwakilan.ToString(), false, true);
            document.Replace("%%Jabatan%%", data.FirstOrDefault().Jabatan.ToString(), false, true);
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
            document.Replace("%%JenisTugas%%", data.FirstOrDefault().Tugas.ToString(), false, true);
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
            IWParagraphStyle style = document.AddParagraphStyle("User");
            style.CharacterFormat.FontName = "Arial";
            IWParagraph paragraph = section.AddParagraph();

            //CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            // var no = 1;

            string Cetak = "";
            //foreach (var item in data)
            //{

            //    Cetak += " • " + item.Initial + " " + item.NamaProduk + " " + "sebesar" + " " + "Rp." + item.NominalOutstanding.ToString("N2") + "\n";

            //}
            foreach (var item in data)
            {
                if (item.MataUang == "Rupiah")
                {
                    Cetak += " • " + item.Initial + " " + item.NamaProduk + " " + "sebesar" + " " + "Rp." + item.NominalOutstanding.ToString("N2") + "\n";
                }
                else if (item.MataUang == "US Dollar")
                {
                    Cetak += " • " + item.Initial + " " + item.NamaProduk + " " + "sebesar" + " " + "USD" + " " + item.NominalOutstanding.ToString("N2") + "\n";
                }


            }


            document.Replace("%%Cetak%%", Cetak, false, true);

            document.Save("LaporanPenggunaanDana.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }
        public JsonResult Save(TransaksiPenggunaanDana transaksi)
        {
            if (transaksi.Id == 0)
            {
                TransaksiPenggunaanDana dana = new TransaksiPenggunaanDana();
                dana.Perusahaan_Id = transaksi.Perusahaan_Id;
                dana.TanggalInput = transaksi.TanggalInput;
                dana.TanggalCetak = GetMinusNewDate(transaksi.TanggalInput, 15);
                dana.Status = false;
                dana.StatusCetak_Id = 2;
                dana.ModifiedDate = DateTime.Now;
                _context.TransaksiPenggunaanDana.Add(dana);
                _context.SaveChanges();
            }
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Done(Int64 Id)
        {
            bool result = false;
            //foreach (var Ids in Id)
            //{
            var data = _context.TransaksiPenggunaanDana.SingleOrDefault(x => x.Id == Id);
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
            var delete = _context.TransaksiPenggunaanDana.SingleOrDefault(x => x.Id == Id);
            _context.TransaksiPenggunaanDana.Remove(delete);
            _context.SaveChanges();
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private DateTime GetMinusNewDate(DateTime startdate, int bil)
        {
            var nextFewJobDays = new List<DateTime>();
            var endDate = startdate.Date;

            while (nextFewJobDays.Count() < bil)
            {
                if (!IsHoliday(endDate) && !IsWeekEnd(endDate))
                {
                    nextFewJobDays.Add(endDate);
                }

                endDate = endDate.AddDays(-1);
            }

            if (bil == 0)
            {
                return startdate;
            }
            else
            {
                return nextFewJobDays.Last();
            }
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