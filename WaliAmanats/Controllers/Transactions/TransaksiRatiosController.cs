using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using System.Data.Entity;
using WaliAmanats.ViewModel;
using WaliAmanats.Models.Transaksi;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using System.Globalization;

namespace WaliAmanats.Controllers.Transactions
{
    public class TransaksiRatiosController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        private SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());
        // GET: TransaksiRatios
        public ActionResult Index()
        {

            var surat = _context.KopSurat.FirstOrDefault();
            RatiosVM result = new RatiosVM();
            result.Perusahaan = _context.Perusahaan.ToList();
            //result.DetailPerusahaan = _context.DetailPerusahaan.Include(x => x.KopSurat).Include(x => x.KopSurat.Perusahaan)
            //    .Include(x => x.KopSurat.StatusCetak).Include(x => x.Matching).Where(x => x.KopSurat_Id == surat.Id).ToList();
            //result.KopSurat = _context.KopSurat.Include(x => x.Perusahaan).Include(x => x.StatusCetak).FirstOrDefault();

            return View(result);
        }
        public JsonResult GetDataAll()
        {
            var result = _context.KopSurat.Include(x => x.Perusahaan).Include(x => x.StatusCetak).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InputRatio()
        {

            return View();
        }

        public ActionResult ViewDetail(Int64 Id)
        {
            var kop = _context.KopSurat.Include(x => x.Perusahaan).Include(x => x.StatusCetak).Where(x => x.Id == Id).FirstOrDefault();
            var det = _context.DetailPerusahaan.Include(x => x.KopSurat).Include(x => x.Matching).Include(x => x.Ratio).Include(x => x.Measurement)
                .Where(x => x.KopSurat_Id == Id).ToList();

            RatiosVM result = new RatiosVM();
            result.DetailPerusahaan = det;
            result.KopSurat = kop;
            return View(result);
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

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetById(Int64 Id)
        {
            var result = _context.KopSurat.Include(x => x.Perusahaan).Include(x => x.StatusCetak)
               .SingleOrDefault(x => x.Id == Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetByIdRatio(Int64 Id)
        {
            var result = (from lap in _context.KopSurat.Include(x => x.Perusahaan).ToList()
                          where lap.Id == Id
                          select new
                          {
                              lap.Perusahaan.Nama,
                              lap.Id,
                              lap.Perusahaan_Id,
                              lap.NoSurat,
                              lap.TanggalSurat,
                              lap.Periode,
                              Detail = (from det in _context.DetailPerusahaan.Include(x => x.Ratio).Include(x => x.Measurement).ToList()
                                        where det.KopSurat_Id == lap.Id
                                        select new
                                        {
                                            det.Id,
                                            det.Ratio_Id,
                                            rationama = det.Ratio.Nama,
                                            det.Target,
                                            det.Realisasi,
                                            det.Measurement_Id,
                                            measurementnama = det.Measurement.Nama

                                        }).ToList()
                          }).SingleOrDefault();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPerusahaan()
        {
            var result = _context.Perusahaan.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPerusahaanById(int Id)
        {
            var result = _context.Perusahaan.Where(x => x.Id == Id).ToList();
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
                        var detail = _context.KopSurat.Include(x => x.Perusahaan).SingleOrDefault(x => x.Id == idDetail);
                        if (detail.Path != null)
                        {
                            string paths = Server.MapPath("~/Files/Ratio/" + detail.Path);
                            System.IO.File.Delete(paths);
                        }

                        var ext = Path.GetExtension(file.FileName);

                        var pathfile = detail.Id + "-" + detail.Perusahaan.Nama + "" + ext;
                        string path = Server.MapPath("~//Files/Ratio/" + pathfile);
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
            var data = _context.KopSurat.SingleOrDefault(x => x.Id == Id);
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
            var delete = _context.KopSurat.SingleOrDefault(x => x.Id == Id);
            _context.KopSurat.Remove(delete);
            _context.SaveChanges();
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmiten(Int64 EmitenId)
        {
            List<RatVM> result = new List<RatVM>();
            result = _context.Perusahaan.Select(x => new RatVM { Id = x.Id, Nama = x.Nama }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSetting(Int64 EmitenId)
        {
            var result = _context.SettingRatio.Include(x => x.Ratio).Include(x => x.Measurement).Where(x => x.Perusahaan_Id == EmitenId).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditSurat(KopSurat surat, List<DetailPerusahaan> detail)
        {
            if (surat.Id == 0)
            {
                KopSurat kop = _context.KopSurat.FirstOrDefault(x => x.Perusahaan_Id == surat.Perusahaan_Id);
                kop.Perusahaan_Id = surat.Perusahaan_Id;
                kop.NoSurat = surat.NoSurat;
                if (surat.TanggalSurat == null)
                {
                    kop.TanggalSurat = null;
                }
                else
                {
                    kop.TanggalSurat = surat.TanggalSurat;

                }
                kop.Periode = surat.Periode;
                kop.ModifiedDate = DateTime.Now;
                kop.Status = false;
                kop.StatusCetak_Id = 2;
                //_context.KopSurat.Add(kop);
                _context.Entry(kop).State = EntityState.Modified;
                _context.SaveChanges();


                foreach (var phs in detail)
                {
                    DetailPerusahaan det = _context.DetailPerusahaan.FirstOrDefault(x => x.Id == phs.Id);
                    det.KopSurat_Id = kop.Id;
                    det.Ratio_Id = phs.Ratio_Id;
                    det.Measurement_Id = phs.Measurement_Id;
                    det.Target = phs.Target;
                    det.Realisasi = phs.Realisasi;
                    //var tra = det.Target;

                    String str = phs.Target;
                    Char value = ':';
                    Boolean resultt = str.Contains(value);


                    if (resultt == true)
                    {
                        int tra = 0;
                        int real = 0;

                        var s = phs.Target;
                        char[] separators = new char[] { ' ', ':' };

                        string[] subs = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);


                        tra = Convert.ToInt32(subs[0]);

                        var ss = phs.Realisasi;
                        char[] separatorss = new char[] { ' ', ':' };

                        string[] subss = ss.Split(separatorss, StringSplitOptions.RemoveEmptyEntries);


                        real = Convert.ToInt32(subss[0]);


                        if (det.Measurement_Id == 1 && real < tra)
                        {
                            det.Matching_Id = 2;
                        }
                        else if (det.Measurement_Id == 1 && real >= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real <= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real > tra)
                        {
                            det.Matching_Id = 2;
                        }
                    }
                    else
                    {
                        var tra = Convert.ToDecimal(phs.Target);
                        var real = Convert.ToDecimal(phs.Realisasi);
                        if (det.Measurement_Id == 1 && real < tra)
                        {
                            det.Matching_Id = 2;
                        }
                        else if (det.Measurement_Id == 1 && real >= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real <= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real > tra)
                        {
                            det.Matching_Id = 2;
                        }

                    }
                    det.ModifiedDate = DateTime.Now;
                    det.CreateDate = DateTime.Now;
                    //_context.DetailPerusahaan.Add(det);
                    _context.Entry(det).State = EntityState.Modified;
                    _context.SaveChanges();


                }

            }
            else
            {
                var kop = _context.KopSurat.SingleOrDefault(x => x.Id == surat.Id);
                kop.Perusahaan_Id = surat.Perusahaan_Id;
                kop.NoSurat = surat.NoSurat;
                if (surat.TanggalSurat == null)
                {
                    kop.TanggalSurat = null;
                }
                else
                {
                    kop.TanggalSurat = surat.TanggalSurat;

                }
                kop.Periode = surat.Periode;
                kop.ModifiedDate = DateTime.Now;
                kop.Status = false;
                kop.StatusCetak_Id = 2;
                _context.Entry(kop).State = EntityState.Modified;
                _context.SaveChanges();

                //_context.SaveChanges();
                foreach (var phs in detail)
                {
                    var det = _context.DetailPerusahaan.SingleOrDefault(x => x.Id == phs.Id);
                    det.KopSurat_Id = kop.Id;
                    det.Ratio_Id = phs.Ratio_Id;
                    det.Measurement_Id = phs.Measurement_Id;
                    det.Target = phs.Target;
                    det.Realisasi = phs.Realisasi;
                    String str = phs.Target;
                    Char value = ':';
                    Boolean resultt = str.Contains(value);


                    if (resultt == true)
                    {
                        int tra = 0;
                        int real = 0;

                        var s = phs.Target;
                        char[] separators = new char[] { ' ', ':' };

                        string[] subs = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);


                        tra = Convert.ToInt32(subs[0]);

                        var ss = phs.Realisasi;
                        char[] separatorss = new char[] { ' ', ':' };

                        string[] subss = ss.Split(separatorss, StringSplitOptions.RemoveEmptyEntries);


                        real = Convert.ToInt32(subss[0]);


                        if (det.Measurement_Id == 1 && real < tra)
                        {
                            det.Matching_Id = 2;
                        }
                        else if (det.Measurement_Id == 1 && real >= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real <= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real > tra)
                        {
                            det.Matching_Id = 2;
                        }
                    }
                    else
                    {
                        var tra = Convert.ToDecimal(phs.Target);
                        var real = Convert.ToDecimal(phs.Realisasi);
                        if (det.Measurement_Id == 1 && real < tra)
                        {
                            det.Matching_Id = 2;
                        }
                        else if (det.Measurement_Id == 1 && real >= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real <= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real > tra)
                        {
                            det.Matching_Id = 2;
                        }

                    }
                    det.ModifiedDate = DateTime.Now;
                    //  det.CreateDate = DateTime.Now;
                    //_context.DetailPerusahaan.Add(det);
                    _context.Entry(det).State = EntityState.Modified;
                    _context.SaveChanges();

                    //_context.SaveChanges();
                }
            }
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SaveSurat(KopSurat surat, List<DetailPerusahaan> detail)
        //{
        //    if (surat.Id == 0)
        //    {
        //        KopSurat kop = new KopSurat();
        //        kop.Perusahaan_Id = surat.Perusahaan_Id;
        //        kop.NoSurat = surat.NoSurat;
        //        if (surat.TanggalSurat == null)
        //        {
        //            kop.TanggalSurat = null;
        //        }
        //        else
        //        {
        //            kop.TanggalSurat = surat.TanggalSurat;

        //        }
        //        kop.Periode = surat.Periode;
        //        kop.ModifiedDate = DateTime.Now;
        //        kop.Status = false;
        //        kop.StatusCetak_Id = 2;
        //        _context.KopSurat.Add(kop);
        //        _context.SaveChanges();



        //        foreach (var phs in detail)
        //        {
        //            DetailPerusahaan det = new DetailPerusahaan();
        //            det.KopSurat_Id = kop.Id;
        //            det.Ratio_Id = phs.Ratio_Id;
        //            det.Measurement_Id = phs.Measurement_Id;
        //            det.Target = phs.Target;
        //            det.Realisasi = phs.Realisasi;
        //            var tra = det.Target;





        //            if (det.Measurement_Id == 1 && det.Realisasi < tra)
        //            {
        //                det.Matching_Id = 2;
        //            }
        //            else if (det.Measurement_Id == 1 && det.Realisasi >= tra)
        //            {
        //                det.Matching_Id = 1;
        //            }
        //            else if (det.Measurement_Id == 2 && det.Realisasi <= tra)
        //            {
        //                det.Matching_Id = 1;
        //            }
        //            else if (det.Measurement_Id == 2 && det.Realisasi > tra)
        //            {
        //                det.Matching_Id = 2;
        //            }
        //            det.ModifiedDate = DateTime.Now;
        //            det.CreateDate = DateTime.Now;
        //            _context.DetailPerusahaan.Add(det);
        //            _context.SaveChanges();
        //        }

        //    }
        //    else
        //    {
        //        var kop = _context.KopSurat.SingleOrDefault(x => x.Id == surat.Id);
        //        kop.Perusahaan_Id = surat.Perusahaan_Id;
        //        kop.NoSurat = surat.NoSurat;
        //        if (surat.TanggalSurat == null)
        //        {
        //            kop.TanggalSurat = null;
        //        }
        //        else
        //        {
        //            kop.TanggalSurat = surat.TanggalSurat;

        //        }
        //        kop.Periode = surat.Periode;
        //        kop.ModifiedDate = DateTime.Now;
        //        kop.Status = false;
        //        kop.StatusCetak_Id = 2;
        //        _context.Entry(kop).State = EntityState.Modified;
        //        _context.SaveChanges();
        //        foreach (var phs in detail)
        //        {
        //            var det = _context.DetailPerusahaan.SingleOrDefault(x => x.Id == phs.Id);
        //            det.KopSurat_Id = kop.Id;
        //            det.Ratio_Id = phs.Ratio_Id;
        //            det.Measurement_Id = phs.Measurement_Id;
        //            det.Target = phs.Target;
        //            det.Realisasi = phs.Realisasi;
        //            var tra = det.Target;
        //            if (det.Measurement_Id == 1 && det.Realisasi < tra)
        //            {
        //                det.Matching_Id = 2;
        //            }
        //            else if (det.Measurement_Id == 1 && det.Realisasi >= tra)
        //            {
        //                det.Matching_Id = 1;
        //            }
        //            else if (det.Measurement_Id == 2 && det.Realisasi <= tra)
        //            {
        //                det.Matching_Id = 1;
        //            }
        //            else if (det.Measurement_Id == 2 && det.Realisasi > tra)
        //            {
        //                det.Matching_Id = 2;
        //            }
        //            det.ModifiedDate = DateTime.Now;
        //            //  det.CreateDate = DateTime.Now;
        //            _context.DetailPerusahaan.Add(det);
        //            _context.SaveChanges();
        //        }
        //    }
        //    var result = _context.SaveChanges();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SaveSuratNew(KopSurat surat, List<DetailPerusahaan> detail)
        {
            if (surat.Id == 0)
            {
                KopSurat kop = new KopSurat();
                kop.Perusahaan_Id = surat.Perusahaan_Id;
                kop.NoSurat = surat.NoSurat;
                if (surat.TanggalSurat == null)
                {
                    kop.TanggalSurat = null;
                }
                else
                {
                    kop.TanggalSurat = surat.TanggalSurat;

                }
                kop.Periode = surat.Periode;
                kop.ModifiedDate = DateTime.Now;
                kop.Status = false;
                kop.StatusCetak_Id = 2;
                _context.KopSurat.Add(kop);
                _context.SaveChanges();



                foreach (var phs in detail)
                {
                    DetailPerusahaan det = new DetailPerusahaan();
                    det.KopSurat_Id = kop.Id;
                    det.Ratio_Id = phs.Ratio_Id;
                    det.Measurement_Id = phs.Measurement_Id;
                    det.Target = phs.Target;
                    det.Realisasi = phs.Realisasi;

                    String str = phs.Target;
                    Char value = ':';
                    Boolean resultt = str.Contains(value);
                   

                    if (resultt == true)
                    {
                       int  tra = 0;
                        int real = 0;

                        var s = phs.Target;
                        char[] separators = new char[] { ' ', ':' };

                        string[] subs = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                     
                        tra = Convert.ToInt32(subs[0]);

                        var ss = phs.Realisasi;
                        char[] separatorss = new char[] { ' ', ':' };

                        string[] subss = ss.Split(separatorss, StringSplitOptions.RemoveEmptyEntries);

                       
                        real = Convert.ToInt32(subss[0]);


                        if (det.Measurement_Id == 1 && real < tra)
                        {
                            det.Matching_Id = 2;
                        }
                        else if (det.Measurement_Id == 1 && real >= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real <= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real > tra)
                        {
                            det.Matching_Id = 2;
                        }
                    }
                    else
                    {
                        var tra = Convert.ToDecimal(phs.Target);
                        var real = Convert.ToDecimal(phs.Realisasi);
                        if (det.Measurement_Id == 1 && real < tra)
                        {
                            det.Matching_Id = 2;
                        }
                        else if (det.Measurement_Id == 1 && real >= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real <= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real > tra)
                        {
                            det.Matching_Id = 2;
                        }

                    }
                  
                    det.ModifiedDate = DateTime.Now;
                    det.CreateDate = DateTime.Now;
                    _context.DetailPerusahaan.Add(det);
                    _context.SaveChanges();
                }

            }
            else
            {
                var kop = _context.KopSurat.SingleOrDefault(x => x.Id == surat.Id);
                kop.Perusahaan_Id = surat.Perusahaan_Id;
                kop.NoSurat = surat.NoSurat;
                if (surat.TanggalSurat == null)
                {
                    kop.TanggalSurat = null;
                }
                else
                {
                    kop.TanggalSurat = surat.TanggalSurat;

                }
                kop.Periode = surat.Periode;
                kop.ModifiedDate = DateTime.Now;
                kop.Status = false;
                kop.StatusCetak_Id = 2;
                _context.Entry(kop).State = EntityState.Modified;
                _context.SaveChanges();


                foreach (var phs in detail)
                {
                    var det = _context.DetailPerusahaan.SingleOrDefault(x => x.Id == phs.Id);
                    det.KopSurat_Id = kop.Id;
                    det.Ratio_Id = phs.Ratio_Id;
                    det.Measurement_Id = phs.Measurement_Id;
                    det.Target = phs.Target;
                    det.Realisasi = phs.Realisasi;
                    //var tra = det.Target;


                    String str = phs.Target;
                    Char value = ':';
                    Boolean resultt = str.Contains(value);

                    if (resultt == true)
                    {
                        int tra = 0;
                        int real = 0;

                        var s = phs.Target;
                        char[] separators = new char[] { ' ', ':' };

                        string[] subs = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);


                        tra = Convert.ToInt32(subs[0]);

                        var ss = phs.Realisasi;
                        char[] separatorss = new char[] { ' ', ':' };

                        string[] subss = ss.Split(separatorss, StringSplitOptions.RemoveEmptyEntries);


                        real = Convert.ToInt32(subss[0]);


                        if (det.Measurement_Id == 1 && real < tra)
                        {
                            det.Matching_Id = 2;
                        }
                        else if (det.Measurement_Id == 1 && real >= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real <= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real > tra)
                        {
                            det.Matching_Id = 2;
                        }
                    }
                    else
                    {
                        var tra = Convert.ToDecimal(phs.Target);
                        var real = Convert.ToDecimal(phs.Realisasi);
                        if (det.Measurement_Id == 1 && real < tra)
                        {
                            det.Matching_Id = 2;
                        }
                        else if (det.Measurement_Id == 1 && real >= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real <= tra)
                        {
                            det.Matching_Id = 1;
                        }
                        else if (det.Measurement_Id == 2 && real > tra)
                        {
                            det.Matching_Id = 2;
                        }

                    }
                    det.ModifiedDate = DateTime.Now;
                    //  det.CreateDate = DateTime.Now;
                    _context.DetailPerusahaan.Add(det);
                    _context.SaveChanges();
                }
            }
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult Input(DetailPerusahaan transaksi)
        //{
        //    var trans = _context.DetailPerusahaan.Where(x => x.Id == transaksi.Id).SingleOrDefault();
        //    trans.Id = transaksi.Id;
        //    trans.KopSurat_Id = transaksi.KopSurat_Id;
        //    //     trans.NoSurat = transaksi.NoSurat;
        //    trans.Realisasi = transaksi.Realisasi;
        //    trans.Ratio_Id = trans.Ratio_Id;
        //    trans.Measurement_Id = trans.Measurement_Id;
        //    var tra = _context.DetailPerusahaan.Include(x => x.Ratio).Include(x => x.Measurement).Select(x => x.Target).FirstOrDefault();
        //    if (trans.Measurement_Id == 1 && trans.Realisasi < tra)
        //    {
        //        trans.Matching_Id = 2;
        //    }
        //    else if (trans.Measurement_Id == 1 && trans.Realisasi >= tra)
        //    {
        //        trans.Matching_Id = 1;
        //    }
        //    else if (trans.Measurement_Id == 2 && trans.Realisasi <= tra)
        //    {
        //        trans.Matching_Id = 1;
        //    }
        //    else if (trans.Measurement_Id == 2 && trans.Realisasi > tra)
        //    {
        //        trans.Matching_Id = 2;
        //    }
        //    //      trans.TanggalSurat = transaksi.TanggalSurat;

        //    trans.ModifiedDate = DateTime.Now;
        //    _context.Entry(trans).State = EntityState.Modified;
        //    _context.SaveChanges();

        //    var result = _context.SaveChanges();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public void CetakLaporanRatio(string TanggalCetak, string Perusahaan_Id, Int64 Idc)
        {

            var result = _con.Query<LaporanRatioVM>(
             "EXEC SP_GetLaporanRatio @TanggalCetak",
              new
              {
                  TanggalCetak = TanggalCetak,
              }).ToList();

            var result2 = _con.Query<LaporanRatioVM2>(
               "EXEC SP_GetRatioProduk @Perusahaan_Id",
                new
                {
                    Perusahaan_Id = Perusahaan_Id,
                }).ToList();

            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "Template_Rasio"); //mengambil path lokasi file
            WordDocument document = new WordDocument(_path, FormatType.Doc); // membaca isi file 
            var trsCetak = _context.KopSurat.Single(x => x.Id == Idc);
            trsCetak.StatusCetak_Id = 5;
            //_context.Entry(Lap).State = EntityState.Modified;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();
            //--simpan variable global
            string NamaGedung = "";
            string Gedung = "";
            string Jalan = "";
            string Kota = "";
            string Gender = "";
            string NamaPerwakilan = "";

            string NoSurat = "";
            string TanggalSurat = "";
            string TanggalInput = "";
            string NamaMeasurements = "";

            string NamaMatchings = "";
            string Target = "";

            string Realisasi = "";
            string Jabatan = "";
            string NamaAgen = "";
            DateTime? NoSurat2 = null;
            string NamaProduk = "";
            string Initial = "";

            //string NamRas = "";
            string NamRas2 = "";

            foreach (var item in result)
            {
                if (item.Gedung != null)
                {
                    Gedung = item.Gedung;

                }
                if (item.NoSurat != null)
                {
                    NoSurat = item.NoSurat;

                }
                else
                {
                    NoSurat = NoSurat2.ToString();
                }
                if (item.TanggalSurat != null)
                {
                    if (item.TanggalSurat.HasValue)
                    {
                        TanggalSurat = item.TanggalSurat.Value.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                    }
                    else
                    {
                        TanggalSurat = " ";
                    }


                }


                NamaGedung = item.Nama_Perusahaan;
                Jalan = item.Jalan;
                Kota = item.Kota;

                Gender = item.Gender;
                NamaPerwakilan = item.NamaPerwakilan;


                TanggalInput = item.TanggalInput.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                NamaMeasurements = item.NamaMeasurements;

                NamaMatchings = item.NamaMatchings;
                Target = item.Target;

                Realisasi = item.Realisasi;

                //NamRas += ", " + item.NamaRasio;
                if (NamaMatchings == "Tidak Memenuhi")
                {

                    NamRas2 += ", " + item.NamaRasio;
                }


            }

            document.Replace("%%Perusahaan%%", NamaGedung, false, true);
            document.Replace("%%Gedung%%", Gedung, false, true);
            document.Replace("%%Jalan%%", Jalan, false, true);
            document.Replace("%%Kota%%", Kota, false, true);

            document.Replace("%%Gender%%", Gender, false, true);
            document.Replace("%%NamaPerwakilan%%", NamaPerwakilan, false, true);

            document.Replace("%%NoSurat%%", NoSurat, false, true);
            if (TanggalSurat == "")
            {
                document.Replace("%%TanggalSurat%%", TanggalSurat, false, true);

            }
            else
            {

                document.Replace("%%TanggalSurat%%", "tgl" + " " + TanggalSurat, false, true);
            }
            document.Replace("%%TanggalInput%%", TanggalInput, false, true);

            document.Replace("%%NamaMeasurements%%", NamaMeasurements, false, true);
            document.Replace("%%NamaMatchings%%", NamaMatchings, false, true);

            document.Replace("%%Target%%", Target, false, true);
            document.Replace("%%Realisasi%%", Realisasi, false, true);

            //document.Replace("%%NamaRasio%%", NamRas, false, true);
            document.Replace("%%NamaRasio2%%", NamRas2, false, true);




            foreach (var item2 in result2)
            {

                Jabatan = item2.NamaJabatan;
                NamaAgen = item2.NamaAgen;

                NamaProduk = item2.NamaProduk;
                Initial = item2.Initial;

            }

            document.Replace("%%Jabatan%%", Jabatan, false, true);
            document.Replace("%%NamaAgen%%", NamaAgen, false, true);

            document.Replace("%%NamaProduk%%", NamaProduk, false, true);
            document.Replace("%%Initial%%", Initial, false, true);


            IWTextRange textRange;
            WSection section = document.Sections[0];
            WTable table = section.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row;
            WTableCell cell;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            var no = 1;

            foreach (var item3 in result)
            {
                //Adds row baru
                row = table.AddRow(true, false);
                //Specifies the row height
                row.Height = 18;
                //Specifies the row height type
                row.HeightType = TableRowHeightType.AtLeast;

                //Column nomor row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(' ' + no.ToString()).CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no++;

                //Column Rasio
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item3.NamaRasio);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                //Column Batas
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item3.NamaMeasurements + ' ' + item3.Target);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                //Column HAsil Audit
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item3.Realisasi);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                //Column Keterangan
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item3.NamaMatchings);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            }
            document.Save("LaporanRatio.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();

        }
    }
}