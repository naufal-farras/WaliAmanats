using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Threading.Tasks;
using WaliAmanats.ViewModel;
using WaliAmanats.Models.Transaksi;
using WaliAmanats.Models;
using WaliAmanats.Models.Master;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WaliAmanats.Controllers
{
    public class TransaksiLaporansController : Controller
    {
        // GET: TransaksiLaporans
        private ApplicationDbContext _context;
        public TransaksiLaporansController()
        {
            _context = new ApplicationDbContext();
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru, int interval)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(interval))
                yield return day;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        public JsonResult GetAll()
        {
            var result = _context.TransaksiLaporan
                                    .Include(x => x.Perusahaan)
                                    .Include(x => x.Perwakilan)
                                    .Include(x => x.Produk)
                                    .Include(x => x.JenisTugas)
                                    .Include(x => x.MataUang)
                                    .Include(x => x.Status)
                                    .Where(x=>x.isDelete == false)
                                    .ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetByIdTgl(Int64 Id)
        {
            var result = _context.TransaksiTanggal.SingleOrDefault(x => x.Id == Id);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveTgl(TransaksiTanggal data)
        {
            var tanggalDb = _context.TransaksiTanggal.Single(x => x.Id == data.Id);
            tanggalDb.Id = data.Id;
            tanggalDb.NilaiBunga = data.NilaiBunga;
            tanggalDb.TglSuratBunga = data.TglSuratBunga;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult StatusSuratDone(Int64 Id)
        {
            var transt = _context.TransaksiTanggal.Single(x => x.Id == Id);
            transt.Status = true;
            _context.Entry(transt).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult StatusSuratUndone(Int64 Id)
        {
            var transt = _context.TransaksiTanggal.Single(x => x.Id == Id);
            transt.Status = false;
            _context.Entry(transt).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(Int64 Id)
        {

            var result = (from lap in _context.TransaksiLaporan
                          where lap.Id == Id
                          select new
                          {
                              lap.Id,
                              lap.Perusahaan_Id,
                              lap.Produk_Id,
                              lap.NamaProduk,
                              lap.NamaPerwakilan_Id,
                              lap.Jabatan_Id,
                              lap.Jabatan.NamaJabatan,
                              lap.JenisTugas_Id,
                              lap.MataUang_Id,
                              lap.MataUang.Satuan,
                              lap.JenisPembayaran,
                              lap.Fee,
                              lap.JenisPengiriman,
                              lap.NamaBank,
                              lap.NamaPenerima,
                              lap.NoRekening,
                              Detail = (from det in _context.TransaksiDetail
                                        where det.Trans_Id == lap.Id
                                        select new
                                        {
                                            det.Id,
                                            det.Seri,
                                            det.KodeEfek,
                                            det.Periode,
                                            det.Nominal,
                                            det.TglTerbit,
                                            det.Bunga,
                                            det.TglJatuhTempo
                                        }).ToList()
                          }).SingleOrDefault();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //bunga nominal
        public decimal GetBunga(Int64 Id, int Interval)
        {
            var db = _context.TransaksiDetail.Include(x => x.TransaksiLaporan).SingleOrDefault(td => td.Id == Id);
            decimal result = 0;
            decimal bunga = Convert.ToDecimal(db.Bunga);
            if (db.TransaksiLaporan.Produk_Id == 1)
            {
                result = db.Nominal * (bunga / 100) * Interval / 360;
            }
            else
            {
                decimal proceed = 1000000 * (bunga / 100) * Interval / 360;
                decimal NilaiEmisi = db.Nominal / 1000000;
                string str = proceed.ToString("N2");
                decimal proceed2 = Convert.ToDecimal(str);
                result = proceed2 * NilaiEmisi;
            }

            return result;

        }


        [HttpPost]
        public JsonResult Save(TransaksiLaporanVM data)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            bool status = false;
            var translap = new TransaksiLaporan();

            List<string> listtgl = new List<string>();

            if (data.Id == 0)
            {
                translap.Perusahaan_Id = data.Perusahaan_Id;
                translap.NamaPerwakilan_Id = data.NamaPerwakilan_Id;
                translap.Jabatan_Id = data.Jabatan_Id;
                translap.Produk_Id = data.Produk_Id;
                translap.NamaProduk = data.NamaProduk;
                translap.JenisTugas_Id = data.JenisTugas_Id;
                translap.MataUang_Id = data.MataUang_Id;
                translap.JenisPembayaran = data.JenisPembayaran;
                translap.Fee = data.Fee;
                translap.JenisPengiriman = data.JenisPengiriman;
                translap.NamaBank = data.NamaBank;
                translap.NamaPenerima = data.NamaPenerima;
                translap.NoRekening = data.NoRekening;
                translap.Status_Id = 1;
                translap.StatusCetak_Id = 1;
                translap.UserId = currentUser.Id;
                translap.ModifiedDate = DateTime.Now;

                _context.TransaksiLaporan.Add(translap);
                _context.SaveChanges();

                var idtrans = translap.Id;

                foreach (var det in data.Details)
                {
                    var transdet = new TransaksiDetail();

                    transdet.Trans_Id = idtrans;
                    transdet.Seri = det.Seri;
                    transdet.KodeEfek = det.KodeEfek;
                    transdet.Nominal = det.Nominal;
                    transdet.TglTerbit = det.TglTerbit;
                    transdet.Bunga = det.Bunga;
                    transdet.TglJatuhTempo = det.TglJatuhTempo;
                    transdet.Periode_Id = det.Periode_Id;
                    transdet.ModifiedDate = DateTime.Now;
                    _context.TransaksiDetail.Add(transdet);
                    _context.SaveChanges();

                    var iddet = transdet.Id;

                    var nokupon = 1;
                    var periode = _context.Periode.Where(x => x.Id == det.Periode_Id).Select(x => x.Hari).Single();
                    var interval = 90;

                    for (DateTime day = det.TglTerbit; day <= det.TglJatuhTempo; day = day.AddMonths(3))
                    {
                        //hitung tanggal
                        var transtgl = new TransaksiTanggal();
                        if (day == det.TglTerbit)
                            continue;

                        periode = periode - interval;
                        var nBunga = GetBunga(transdet.Id, interval);
                        if (periode == 10)
                        {
                            var last = day.AddDays(10);
                            day = last;
                            nBunga = GetBunga(transdet.Id, interval + 10);
                        }

                        if (periode <= 0)
                        {
                            break;
                        }
                        transtgl.NilaiBunga = nBunga;
                        transtgl.Detail_Id = iddet;
                        transtgl.TglSuratBunga = day;
                        transtgl.NoKupon = nokupon++;
                        transtgl.ModifiedDate = DateTime.Now;
                        _context.TransaksiTanggal.Add(transtgl);
                        _context.SaveChanges();
                    }

                    status = true;
                }

            }
            else
            {
                var trlap = _context.TransaksiLaporan.Single(x => x.Id == data.Id);

                trlap.Perusahaan_Id = data.Perusahaan_Id;
                trlap.NamaPerwakilan_Id = data.NamaPerwakilan_Id;
                trlap.Jabatan_Id = data.Jabatan_Id;
                trlap.Produk_Id = data.Produk_Id;
                trlap.NamaProduk = data.NamaProduk;
                trlap.JenisTugas_Id = data.JenisTugas_Id;
                trlap.MataUang_Id = data.MataUang_Id;
                trlap.JenisPembayaran = data.JenisPembayaran;
                trlap.Fee = data.Fee;
                trlap.JenisPengiriman = data.JenisPengiriman;
                trlap.NamaBank = data.NamaBank;
                trlap.NamaPenerima = data.NamaPenerima;
                trlap.NoRekening = data.NoRekening;
                trlap.Status_Id = 1;
                trlap.StatusCetak_Id = 1;
                translap.UserId = currentUser.Id;
                trlap.ModifiedDate = DateTime.Now;
                _context.Entry(trlap).State = EntityState.Modified;
                _context.SaveChanges();

                var trdet = _context.TransaksiDetail.Where(x => x.Trans_Id == data.Id);

                _context.TransaksiDetail.RemoveRange(trdet);
                _context.SaveChanges();

                foreach (var det in data.Details)
                {
                    var transdet = new TransaksiDetail();

                    transdet.Trans_Id = data.Id;
                    transdet.Seri = det.Seri;
                    transdet.KodeEfek = det.KodeEfek;
                    transdet.Nominal = det.Nominal;
                    transdet.TglTerbit = det.TglTerbit;
                    transdet.Bunga = det.Bunga;
                    transdet.TglJatuhTempo = det.TglJatuhTempo;
                    transdet.Periode_Id = det.Periode_Id;
                    transdet.ModifiedDate = DateTime.Now;
                    _context.TransaksiDetail.Add(transdet);
                    _context.SaveChanges();

                    var iddet = transdet.Id;

                    var nokupon = 1;
                    var periode = _context.Periode.Where(x => x.Id == det.Periode_Id).Select(x => x.Hari).Single();
                    var interval = 90;

                    for (DateTime day = det.TglTerbit; day <= det.TglJatuhTempo; day = day.AddMonths(3))
                    {
                        var transtgl = new TransaksiTanggal();
                        if (day == det.TglTerbit)
                            continue;

                        periode = periode - interval;
                        var nBunga = GetBunga(transdet.Id, interval);
                        if (periode == 10)
                        {
                            var last = day.AddDays(10);
                            day = last;
                            nBunga = GetBunga(transdet.Id, interval + 10);
                        }

                        if (periode <= 0)
                        {
                            break;
                        }
                        transtgl.NilaiBunga = nBunga;
                        transtgl.Detail_Id = iddet;
                        transtgl.TglSuratBunga = day;
                        transtgl.NoKupon = nokupon++;
                        transtgl.ModifiedDate = DateTime.Now;
                        _context.TransaksiTanggal.Add(transtgl);
                        _context.SaveChanges();
                    }

                    status = true;
                }
            }

            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult Delete(Int64 Id)
        {
            bool result = false;
            var b = _context.TransaksiLaporan.Where(x => x.Id == Id).FirstOrDefault();
            var d = _context.DetailCetak.Where(x => x.TransaksiLaporan_Id == Id).Select(x=>x.TransaksiCetak_Id).FirstOrDefault();
            var c = _context.TransaksiCetak.Where(x => x.Id == d).FirstOrDefault();
            if (b != null)
            {
                b.isDelete = true;
                _context.Entry(b).State = EntityState.Modified;
                _context.SaveChanges();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTglBunga(Int64 Id)
        {
            var result = _context.TransaksiTanggal.Where(x => x.Detail_Id == Id).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Otorisasi()
        {
            return View();
        }

        public JsonResult GetListApproval()
        {
            var result = _context.TransaksiLaporan
                                    .Include(x => x.Perusahaan)
                                    .Include(x => x.Perwakilan)
                                    .Include(x => x.Produk)
                                    .Include(x => x.JenisTugas)
                                    .Include(x => x.MataUang)
                                    .Include(x => x.Status)
                                    .Where(x => x.Status_Id == 1)
                                    .ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Approval(Int64 Id, bool status)
        {
            var lap = new TransaksiLaporan();
            if (status == true)
            {
                lap = _context.TransaksiLaporan.Single(x => x.Id == Id);

                lap.Status_Id = 2;
            }
            else
            {
                lap = _context.TransaksiLaporan.Single(x => x.Id == Id);

                lap.Status_Id = 3;
            }

            _context.Entry(lap).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListHistory()
        {
            var result = _context.TransaksiLaporan
                                    .Include(x => x.Perusahaan)
                                    .Include(x => x.Perwakilan)
                                    .Include(x => x.Produk)
                                    .Include(x => x.JenisTugas)
                                    .Include(x => x.MataUang)
                                    .Include(x => x.Status)
                                    .Where(x=>x.isDelete == false && ( x.Status_Id == 2 || x.Status_Id == 3))
                                    .ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        //list viewbag untuk drop down
        public JsonResult GetPerusahaan()
        {
            var result = _context.Perusahaan.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriode()
        {
            var result = _context.Periode.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTipeProduk()
        {
            var result = _context.Produk.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTipeTugas()
        {
            var result = _context.JenisTugas.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPerwakilan(int Id)
        {
            var result = _context.Perwakilan.Where(x => x.Perusahaan_Id == Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJabatan(int Id)
        {
            var result = _context.Perwakilan.Include(x => x.Jabatan).Single(x => x.Id == Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMataUang()
        {
            var result = _context.MataUang.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}