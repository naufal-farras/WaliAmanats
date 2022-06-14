using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.ViewModel;

namespace WaliAmanats.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            //today
            ViewBag.PerluDiCetak = (from tgl in _context.TransaksiTanggal
                          .Include(x => x.TransaksiDetail)
                          .Include(x => x.TransaksiDetail.TransaksiLaporan)
                          .Include(x => x.TransaksiDetail.TransaksiLaporan.StatusCetak)
                          .ToList()
                                    where (tgl.TglSuratBunga.AddDays(-15) <= DateTime.Now && tgl.TglSuratBunga >= DateTime.Now ) && tgl.TransaksiDetail.TransaksiLaporan.StatusCetak.Id == 1 &&
                                    tgl.TransaksiDetail.TransaksiLaporan.isDelete == false && tgl.Status == false
                                    select new
                                    {
                                    }).Count();
            ViewBag.CekInput = _context.TransaksiLaporan.Where(x => x.Status_Id == 1).Count();
            ViewBag.Emiten = _context.Perusahaan.Count();
            ViewBag.Total = _context.TransaksiCetak.Where(x => x.StatusCetak_Id == 5).Count();
            ViewBag.PerluApproval = _context.TransaksiCetak.Where(x => x.StatusCetak_Id == 3).Count();
            ViewBag.Done = _context.TransaksiCetak.Where(x => x.StatusCetak_Id == 5).Count();

            ViewBag.PerluDiCek = _context.TransaksiCetak.Where(x => x.StatusCetak_Id == 7).Count();

            ViewBag.CetakKeuangan = _context.DetailKeuangan.Where(x => x.TanggalCetak.Year == DateTime.Now.Year && x.TanggalCetak.Month == DateTime.Now.Month
            && x.TanggalCetak.Day == DateTime.Now.Day && x.StatusCetak_Id == 2).Count();
            ViewBag.CetakJaminan = _context.TransaksiJaminan.Where(x => x.TanggalCetak.Year == DateTime.Now.Year && x.TanggalCetak.Month == DateTime.Now.Month
            && x.TanggalCetak.Day == DateTime.Now.Day && x.StatusCetak_Id == 2).Count();
            ViewBag.CetakDana = _context.TransaksiPenggunaanDana.Where(x => x.TanggalCetak.Year == DateTime.Now.Year && x.TanggalCetak.Month == DateTime.Now.Month
            && x.TanggalCetak.Day == DateTime.Now.Day && x.StatusCetak_Id == 2).Count();
            ViewBag.CetakKejadian = _context.TransaksiKejadian.Where(x => x.TanggalCetak.Year == DateTime.Now.Year && x.TanggalCetak.Month == DateTime.Now.Month
            && x.TanggalCetak.Day == DateTime.Now.Day && x.StatusCetak_Id == 2).Count();

            ViewBag.SiapDiCetak = _context.TransaksiCetak.Where(x => x.StatusCetak_Id == 4).Count();
            var count = _context.SubDetailTanggal.Where(x => x.TransaksiTanggal.Status == false).Select(x => x.TransaksiTanggal.NilaiBunga).Count();
            if(count > 0)
            {
                ViewBag.sediadana = _context.SubDetailTanggal.Where(x => x.TransaksiTanggal.Status == false).Select(x => x.TransaksiTanggal.NilaiBunga).Sum();
            }
            else
            {
                ViewBag.sediadana = 0;
            }
            
            //week
            //ViewBag.PerluCetak7 = _context.TransaksiCetak.Where(x => x.StatusCetak_Id == 1 && (x.TglSurat >= DateTime.Now && x.TglSurat <= DateTime.Now.AddDays(7))).Count();
            return View();
        }
            

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}