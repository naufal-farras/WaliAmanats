using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WaliAmanats.Models;
using WaliAmanats.ViewModel;
using WaliAmanats.Models.Transaksi;
using WaliAmanats.Models.Master;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework; 
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using Syncfusion.DocIO.DLS;
using Dapper;
using Syncfusion.DocIO;
using System.Globalization;

namespace WaliAmanats.Controllers.Transactions
{
    public class TransaksiRatingsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        private SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());

        // GET: TransaksiRatings
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewRating(Int64 Id)
        {
            var res = _context.TransaksiRating.Include(x => x.Rating).Include(x => x.Perusahaan)
                .Include(x => x.Matching).Include(x => x.StatusCetak).Where(x => x.Id == Id).FirstOrDefault();
            RatingVM result = new RatingVM();
            result.TransaksiRating = res;
            return View(result);
        }
   
        public JsonResult GetFilter()
        {
            FilterVM result = new FilterVM();
            result.Emiten = _context.Perusahaan.Select(x => x.Nama).ToList();
            result.Jenis = _context.Produk.Select(x => x.Nama).ToList();
            result.Produk = _context.TransaksiLaporan.Include(x => x.Perusahaan).Select(x => x.NamaProduk).ToList();
            result.Batas = _context.Rating.Select(x => x.Nama).ToList();
            result.Keterangan = _context.Matching.Select(x => x.Nama).ToList();
            result.Jaminan = _context.Jaminan.Select(x => x.Nama).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPerusahaan()
        {
            var result = _context.Perusahaan.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProduk(Int64 EmitenId)
        {
            var result = _context.TransaksiLaporan.Include(x => x.Perusahaan).Where(x => x.Perusahaan_Id == EmitenId).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRating()
        {
            var result = _context.Rating.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetById(Int64 Id)
        {
            var result = _context.TransaksiRating.Include(x => x.Perusahaan).Include(x => x.Rating)
                .SingleOrDefault(x => x.Id == Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNilai(Int64 Rating)
        {
            Rating result = new Rating();
            var data = _context.Rating.FirstOrDefault(x => x.Id == Rating);
            if (data != null)
            {
                result = data;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDataAll()
        {
            var result = _context.TransaksiRating.Include(x => x.Rating).Include(x => x.Perusahaan).Include(x => x.Perusahaan.Rating)
                .Include(x => x.Matching).ToList();

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }
        public JsonResult SaveRating(TransaksiRating rating)
        {
            if (rating.Id == 0)
            {
                TransaksiRating rat = new TransaksiRating();
                rat.Perusahaan_Id = rating.Perusahaan_Id;
                rat.Rating_Id = rating.Rating_Id;
                rat.TanggalInput = rating.TanggalInput;
                rat.Nilai = rating.Nilai;
              
                var tran = _context.TransaksiRating.Include(x => x.Perusahaan).Select(x => x.Perusahaan.Nilai).FirstOrDefault();

                if (tran < rat.Nilai) 
                {
                    rat.Matching_Id = 2;
                }
                else
                {
                    rat.Matching_Id = 1;
                }

                rat.StatusCetak_Id = 2;
                _context.TransaksiRating.Add(rat);
                _context.SaveChanges();

                var rate = _context.TransaksiRating.SingleOrDefault(x => x.Id == rat.Id);

                rate.Perusahaan_Id = rating.Perusahaan_Id;
                rate.Rating_Id = rating.Rating_Id;
                rate.TanggalInput = rating.TanggalInput;
                rate.Nilai = rating.Nilai;
                var trans = _context.TransaksiRating.Include(x => x.Perusahaan).Select(x => x.Perusahaan.Nilai).FirstOrDefault();
                if (rate.Nilai < trans)
                {
                    rate.Matching_Id = 2;
                }
                else
                {
                    rate.Matching_Id = 1;
                }
                _context.Entry(rate).State = EntityState.Modified;
                _context.SaveChanges();
            }
            else
            {
                var rate = _context.TransaksiRating.SingleOrDefault(x => x.Id == rating.Id);

                rate.Perusahaan_Id = rating.Perusahaan_Id;
                rate.Rating_Id = rating.Rating_Id;
                rate.TanggalInput = rating.TanggalInput;
                rate.Nilai = rating.Nilai;
                var tran = _context.TransaksiRating.Include(x => x.Perusahaan).Select(x => x.Perusahaan.Nilai).FirstOrDefault();
                if (rate.Nilai < tran)
                {
                    rate.Matching_Id = 2;
                }
                else
                {
                    rate.Matching_Id = 1;
                }
                _context.Entry(rate).State = EntityState.Modified;
                _context.SaveChanges();
            }
            var result =  _context.SaveChanges();;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Done(Int64 Id)
        {
            bool result = false;
            //foreach (var Ids in Id)
            //{
            var data = _context.TransaksiRating.SingleOrDefault(x => x.Id == Id);
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
            var delete = _context.TransaksiRating.SingleOrDefault(x => x.Id == Id);
            _context.TransaksiRating.Remove(delete);
            _context.SaveChanges();
            result = true;
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
                        var detail = _context.TransaksiRating.Include(x => x.Perusahaan).SingleOrDefault(x => x.Id == idDetail);
                        if (detail.Path != null)
                        {
                            string paths = Server.MapPath("~/Files/Rating/" + detail.Path);
                            System.IO.File.Delete(paths);
                        }

                        var ext = Path.GetExtension(file.FileName);

                        var pathfile = detail.Id + "-" + detail.Perusahaan.Nama + "" + ext;
                        string path = Server.MapPath("~/Files/Rating/" + pathfile);
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

        //CETAK CetakLaporanRating
        public void CetakLaporanRating(string IdMatch, Int64 Idc, Int64 Perusahaan_Id)
        {

            var result = _con.Query<LaporanRatingVM>(
             "EXEC SP_GetLaporanRating @IdMatch, @Perusahaan_Id",
              new
              {
                  
                  IdMatch = IdMatch,
                  Perusahaan_Id = Perusahaan_Id,
              }).ToList();

            var result2 = _con.Query<LaporanMatchingVM>(
            "EXEC SP_GetLaporanMatching @IdMatch,@Perusahaan_Id,@Idc",
             new
             {
                 Idc = Idc,
                 IdMatch = IdMatch,
                 Perusahaan_Id = Perusahaan_Id,

             }).ToList();



            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "Template_Rating"); //mengambil path lokasi file
            WordDocument document = new WordDocument(_path, FormatType.Doc); // membaca isi file 
            var trsCetak = _context.TransaksiRating.Single(x => x.Id == Idc);
            trsCetak.StatusCetak_Id = 5;
            //_context.Entry(Lap).State = EntityState.Modified;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();
            var Gedung = "";
            foreach (var item in result)
            {
                if (item.Gedung!=null ) 
                {
                    Gedung = item.Gedung;
                   
                }
                
                document.Replace("%%Gedung%%", Gedung, false, true);
                document.Replace("%%Jalan%%", item.Jalan, false, true);
                document.Replace("%%Perusahaan%%", item.Nama_Perusahaan, false, true);
                document.Replace("%%Kota%%", item.Kota, false, true);


                document.Replace("%%Gender%%", item.Gender, false, true);
                document.Replace("%%NamaPerwakilan%%", item.NamaPerwakilan, false, true);

                document.Replace("%%Jabatan%%", item.NamaJabatan, false, true);
                document.Replace("%%NamaAgen%%", item.NamaAgen, false, true);

                for (int i = 0; i < item.NamaProduk.Length; i++)
                {
                    document.Replace("%%NamaProduk%%", item.NamaProduk, false, true);

                }
                document.Replace("%%Initial%%", item.Initial, false, true);

                document.Replace("%%TanggalInput%%", item.TanggalInput.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);
                document.Replace("%%TanggalInput2%%", item.TanggalInput.ToString("MMMM yyyy", new System.Globalization.CultureInfo("id-ID")), false, true);

            }
          


            IWTextRange textRange;
            WSection section = document.Sections[0];
            WTable table = section.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row;
            WTableCell cell;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            var no = 1;

            foreach (var item in result2)
            {
                var IdTarget = _context.Rating.FirstOrDefault(x => x.Id == item.Target);
                var IdRealisasi = _context.Rating.FirstOrDefault(x => x.Id == item.Realisasi);

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

                //Column Rating (Realisasi)
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(IdRealisasi.Nama);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                //Column Batas Ketentuan (Target)
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(IdTarget.Nama);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                //Column Keterangan (Matching)
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Keterangan);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;


            }
            document.Save("LaporanRating.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();

        }
    }
}