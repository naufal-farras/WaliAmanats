using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.ViewModel;
using WaliAmanats.Models.Transaksi;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using Microsoft.AspNet.Identity;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;

namespace WaliAmanats.Controllers.Transactions
{ 
    public class TransaksiKeuanganController : Controller
    {
        private SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());

        ApplicationDbContext _context = new ApplicationDbContext();
         private int count;
        // GET: TransaksiKeuangan
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CetakLK()
        {
            return View();
        }
        public ActionResult ViewLK(Int64 Id)
        {
            var view = new DetailLpVM();

            var result = (from lap in _context.DetailKeuangan
                       .Include(x => x.TransaksiKeuangan)
                       .Include(x => x.TransaksiKeuangan.TransaksiLaporan)
                       .Include(x => x.TransaksiKeuangan.TransaksiLaporan.Produk)
                       .Include(x => x.TransaksiKeuangan.TransaksiLaporan.Perusahaan)
                       .Include(x => x.StatusCetak).Where(x => x.TanggalCetak.Year == DateTime.Now.Year && x.TanggalCetak.Month == DateTime.Now.Month
                       && x.TanggalCetak.Day == DateTime.Now.Day && x.Id == Id)
                       .ToList()
                              // where lap.Status == false && lap.StatusCetak_Id == 2 
                          select new DetailLpVM
                          {
                              IdDetail = lap.Id,
                              Perusahaan = lap.TransaksiKeuangan.TransaksiLaporan.Perusahaan.Nama,
                              Produk = lap.TransaksiKeuangan.TransaksiLaporan.Produk.Nama,
                              NamaProduk = lap.TransaksiKeuangan.TransaksiLaporan.NamaProduk,
                              TanggalSurat = lap.TanggalSurat,
                              TanggalCetak = lap.TanggalCetak,
                              Path = lap.Path,
                              Status = lap.Status,
                              StatusCetakId = lap.StatusCetak_Id
                              //StatusCetakNama = lap.StatusCetak.Nama
                              //DetailKeuangan = _context.DetailKeuangan.Where(x => x.TransaksiKeuangan_Id == lap.TransaksiKeuangan.Id).SingleOrDefault()
                          }).ToList();
            return View("ViewLK", result);
        }
        public ActionResult ViewLKAll(Int64 Id)
        {
            var view = new DetailLpVM();

            var result = (from lap in _context.DetailKeuangan
                       .Include(x => x.TransaksiKeuangan)
                       .Include(x => x.TransaksiKeuangan.TransaksiLaporan)
                       .Include(x => x.TransaksiKeuangan.TransaksiLaporan.Produk)
                       .Include(x => x.TransaksiKeuangan.TransaksiLaporan.Perusahaan)
                       .Include(x => x.StatusCetak).Where(x => x.Id == Id)
                       .ToList()
                              // where lap.Status == false && lap.StatusCetak_Id == 2 
                          select new DetailLpVM
                          {
                              IdDetail = lap.Id,
                              Perusahaan = lap.TransaksiKeuangan.TransaksiLaporan.Perusahaan.Nama,
                              Produk = lap.TransaksiKeuangan.TransaksiLaporan.Produk.Nama,
                              NamaProduk = lap.TransaksiKeuangan.TransaksiLaporan.NamaProduk,
                              TanggalSurat = lap.TanggalSurat,
                              TanggalCetak = lap.TanggalCetak,
                              Path = lap.Path,
                              Status = lap.Status,
                              StatusCetakId = lap.StatusCetak_Id
                              //StatusCetakNama = lap.StatusCetak.Nama
                              //DetailKeuangan = _context.DetailKeuangan.Where(x => x.TransaksiKeuangan_Id == lap.TransaksiKeuangan.Id).SingleOrDefault()
                          }).ToList();
            return View("ViewLK", result);
        }

        public JsonResult GetById(Int64 Id)
        {
            var result = _context.DetailKeuangan.Include(x => x.TransaksiKeuangan.TransaksiLaporan).Include(x => x.TransaksiKeuangan.TransaksiLaporan.Perusahaan).Include(x => x.StatusCetak)
              .SingleOrDefault(x => x.Id == Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru, int interval)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(interval))
                yield return day;
        }
        public JsonResult GetFilter()
        {
            FilterVM result = new FilterVM();
            result.Emiten = _context.Perusahaan.Select(x => x.Nama).ToList();
            result.Jenis = _context.Produk.Select(x => x.Initial).ToList();
            result.Produk = _context.TransaksiLaporan.Include(x => x.Produk).GroupBy(x => x.Produk.Nama).Select(x => x.Key).ToList();
            result.Batas = _context.Rating.Select(x => x.Nama).ToList();
            result.Keterangan = _context.Matching.Select(x => x.Nama).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDataAll()
        {
            var result = _context.TransaksiKeuangan.Include(x => x.TransaksiLaporan).Include(x => x.TransaksiLaporan.Perusahaan)
                 .Include(x => x.TransaksiLaporan.Produk).ToList();

             return Json(new { data = result }, JsonRequestBehavior.AllowGet);
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
                        var detail = _context.DetailKeuangan.Include(x => x.TransaksiKeuangan).Include(x => x.TransaksiKeuangan.TransaksiLaporan)
                            .Include(x => x.TransaksiKeuangan.TransaksiLaporan.Perusahaan)
                            .SingleOrDefault(x => x.Id == idDetail);
                        if (detail.Path != null)
                        {
                            string paths = Server.MapPath("~/Files/Keuangan/" + detail.Path);
                            System.IO.File.Delete(paths);
                        }

                        var ext = Path.GetExtension(file.FileName);

                        var pathfile = detail.Id + "-" + detail.TransaksiKeuangan.TransaksiLaporan.Perusahaan.Nama + "" + ext;
                        string path = Server.MapPath("~//Files/Keuangan/" + pathfile);
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
                
        public JsonResult GetLP()
        {
            var result = (from lap in _context.DetailKeuangan
                        .Include(x => x.TransaksiKeuangan)
                        .Include(x => x.TransaksiKeuangan.TransaksiLaporan)
                        .Include(x => x.TransaksiKeuangan.TransaksiLaporan.Produk)
                        .Include(x => x.TransaksiKeuangan.TransaksiLaporan.Perusahaan)
                        .Include(x => x.StatusCetak).Where(x => x.TanggalCetak.Year == DateTime.Now.Year && x.TanggalCetak.Month == DateTime.Now.Month
                        && x.TanggalCetak.Day == DateTime.Now.Day)
                        .ToList()
                         // where lap.Status == false && lap.StatusCetak_Id == 2 
                          select new
                          {
                              Id = lap.Id,
                              lap.TransaksiKeuangan.TransaksiLaporan.Perusahaan,
                              lap.TransaksiKeuangan.TransaksiLaporan.Produk,
                              lap.TransaksiKeuangan.TransaksiLaporan.NamaProduk,
                              TglSuratTerdekat = lap.TanggalSurat,
                              TglCetak = lap.TanggalCetak,
                              lap.StatusCetak,
                              Status = lap.Status,
                              TglHariIni = DateTime.Now
                          }).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLPAll()
        {
            var result = (from lap in _context.DetailKeuangan
                        .Include(x => x.TransaksiKeuangan)
                        .Include(x => x.TransaksiKeuangan.TransaksiLaporan)
                        .Include(x => x.TransaksiKeuangan.TransaksiLaporan.Produk)
                        .Include(x => x.TransaksiKeuangan.TransaksiLaporan.Perusahaan)
                        .Include(x => x.StatusCetak)
                        .ToList()
                          select new
                          {
                              Id = lap.Id,
                              lap.TransaksiKeuangan.TransaksiLaporan.Perusahaan,
                              lap.TransaksiKeuangan.TransaksiLaporan.Produk,
                              lap.TransaksiKeuangan.TransaksiLaporan.NamaProduk,
                              TglSuratTerdekat = lap.TanggalSurat,
                              TglCetak = lap.TanggalCetak,
                              lap.StatusCetak,
                              Status = lap.Status,
                              TglHariIni = DateTime.Now
                          }).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Done(Int64 Id)
        {
            bool result = false;
            //foreach (var Ids in Id)
            //{
            var data = _context.DetailKeuangan.SingleOrDefault(x => x.Id == Id);
            data.Status = true;
            _context.Entry(data).State = EntityState.Modified;
            _context.SaveChanges();
            result = true;
            //}

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPerusahaan()
        {
            var result = _context.Perusahaan.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProduk(Int64 EmitenId)
        {
            var result = _context.TransaksiLaporan.Include(x => x.Perusahaan).Where(x => x.Perusahaan_Id == EmitenId && x.isDelete == false).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save(TransaksiKeuangan trans)
        {
            List<DetailKeuangan> detail = new List<DetailKeuangan>();

            if (trans.Id == 0)
            {
                TransaksiKeuangan tra = new TransaksiKeuangan();
                tra.TransaksiLaporan_Id = trans.TransaksiLaporan_Id;
                tra.TanggalInput = trans.TanggalInput;
                tra.JatuhTempo = trans.JatuhTempo;
                tra.ModifiedDate = DateTime.Now;
                _context.TransaksiKeuangan.Add(tra);
                _context.SaveChanges();
                Int64 Id = tra.Id;
                var dbLap = _context.TransaksiKeuangan.Where(x => x.Id == Id).FirstOrDefault();

                for (DateTime day = tra.TanggalInput; day <= tra.JatuhTempo; day = day.AddMonths(3))
                {
                    DetailKeuangan de = new DetailKeuangan();
                    if (day == tra.TanggalInput)
                    {
                        continue;
                    }
                    else
                    {
                        de.TransaksiKeuangan_Id = tra.Id;
                        de.ModifiedDate = DateTime.Now;
                        de.Status = false;
                        de.TanggalSurat = day;
                        de.TanggalCetak = GetMinusNewDate(day, 15);
                        de.StatusCetak_Id = 2;
                        _context.DetailKeuangan.Add(de);
                        _context.SaveChanges();
                    }

                }

            }
            else
            {
                var transaksi = _context.TransaksiKeuangan.Single(x => x.Id == trans.Id);
                transaksi.Id = trans.Id;
                transaksi.TransaksiLaporan_Id = trans.Id;
                transaksi.TanggalInput = trans.TanggalInput;
                transaksi.JatuhTempo = trans.JatuhTempo;
                transaksi.ModifiedDate = DateTime.Now;
                _context.Entry(transaksi).State = EntityState.Modified;
                _context.SaveChanges();

                var trdet = _context.DetailKeuangan.Where(x => x.TransaksiKeuangan_Id == trans.Id);

                _context.DetailKeuangan.RemoveRange(trdet);
                _context.SaveChanges();

                Int64 Id = transaksi.Id;
                var dbLap = _context.TransaksiKeuangan.Where(x => x.Id == Id).FirstOrDefault();

                for (DateTime day = transaksi.TanggalInput; day <= transaksi.JatuhTempo; day = day.AddMonths(3))
                {

                    DetailKeuangan de = new DetailKeuangan();
                    if (day == transaksi.TanggalInput)
                    {
                        continue;
                    }
                    else
                    {
                        de.TransaksiKeuangan_Id = transaksi.Id;
                        de.ModifiedDate = DateTime.Now;
                        de.Status = false;
                        de.TanggalSurat = day;
                        de.TanggalCetak = GetMinusNewDate(day, 15);
                        de.StatusCetak_Id = 2;
                        _context.DetailKeuangan.Add(de);
                        _context.SaveChanges();
                    }
                }
            }
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
 
        public JsonResult Delete(Int64 Id)
        {
            bool result = false;
            var delete = _context.TransaksiKeuangan.SingleOrDefault(x => x.Id == Id);
            _context.TransaksiKeuangan.Remove(delete);
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
            return _context.HariLibur.Select(x => x.TanggalLibur).Contains(date);
        }

        private bool IsWeekEnd(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;
        }
        public DateTime TanggalCetak(Int64 Id)
        {
            var date = DateTime.Now;
            count = _context.DetailKeuangan.Where(x => x.TransaksiKeuangan_Id == Id && x.Status == false).Select(x => x.TanggalSurat).Count();
            if (count == 0)
            {
                date = DateTime.Now.AddDays(1);
            }
            else
            {
                date = _context.DetailKeuangan.Where(x => x.TransaksiKeuangan_Id == Id && x.Status == false).OrderByDescending(x=> x.TanggalSurat).Select(x => x.TanggalSurat).FirstOrDefault();
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
            var result = _context.DetailKeuangan.Where(x => x.TransaksiKeuangan_Id == Id && x.Status == false).Select(x => x.TanggalSurat).FirstOrDefault();
            return result;
        }

        //Cetak Laporan Keuangan
        public void CetakLaporanKeuangan(string TanggalCetak, Int64 Idc)
        {

            var result = _con.Query<LaporanKeuanganVM>(
               "EXEC SP_GetLaporanKeuangan @TanggalCetak, @Idc",
                new
                {
                    TanggalCetak = TanggalCetak,
                    Idc = Idc
                }).ToList();
            var result2 = _context.DetailKeuangan.FirstOrDefault(x => x.Id == Idc);
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "Template_LK"); //mengambil path lokasi file
            WordDocument document = new WordDocument(_path, FormatType.Doc); // membaca isi file 
            var trsCetak = _context.DetailKeuangan.Single(x => x.Id == Idc);
            trsCetak.StatusCetak_Id = 5;
            //_context.Entry(Lap).State = EntityState.Modified;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();
            string Gedung = "";

            foreach (var item in result)
            {
                if (item.Gedung != null)
                {
                    Gedung = item.Gedung;

                }
                document.Replace("%%Perusahaan%%", item.Nama_Perusahaan, false, true);
                document.Replace("%%Gedung%%", Gedung, false, true);
                document.Replace("%%Jalan%%", item.Jalan, false, true);
                document.Replace("%%Kota%%", item.Kota, false, true);

                document.Replace("%%Gender%%", item.Gender, false, true);
                document.Replace("%%NamaPerwakilan%%", item.NamaPerwakilan, false, true);
                document.Replace("%%Jabatan%%", item.NamaJabatan, false, true);

                document.Replace("%%JenisTugas%%", item.NamaAgen, false, true);

              
            }
            document.Replace("%%JatuhTempo%%", result2.TanggalSurat .ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);
            
            var jatuhtempo2 = result2.TanggalSurat.AddYears(-1);
            document.Replace("%%TanggalInput%%", jatuhtempo2.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);

            document.Save("LaporanKeuangan.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();



        }
    }
}