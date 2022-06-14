using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WaliAmanats.Models;
using System.Globalization;

namespace WaliAmanats.Controllers.Transactions
{
    public class OJKKejadianController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: OJKKejadian
        public ActionResult Index()
        {
            return View();
        }
        public void LaporanOJKBuyback(Int64 Aturan, Int64 Emiten, DateTime Tanggal)
        {
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "OJKBuyback");
            WordDocument document = new WordDocument(_path, FormatType.Docx);
            var data = _context.TransaksiKejadian
                .Include(x => x.TransaksiTanggal)
                .Include(x => x.TransaksiDetail)
                .Include(x => x.TransaksiDetail.TransaksiLaporan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Produk)
                .Where(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan_Id == Emiten && x.TanggalInput == Tanggal
                && x.TransaksiDetail.TransaksiLaporan.isDelete == false)
                .ToList()
                .GroupBy(x => x.TransaksiDetail_Id)
                .Select(x => x.FirstOrDefault());
            var aturan = _context.AturanOJK.Single(x => x.Id == Aturan);
            var alamatOJK = _context.OJK.FirstOrDefault();

            document.Replace("%%Aturan%%", aturan.Aturan, false, true);
            document.Replace("%%IsiAturan%%", aturan.IsiAturan, false, true);
            document.Replace("%%Gedung%%", alamatOJK.Alamat1, false, true);
            document.Replace("%%Jalan%%", alamatOJK.Alamat2, false, true);
            document.Replace("%%Kota%%", alamatOJK.Alamat3, false, true);
            

            IWTextRange textRange;
            WSection section = document.Sections[1];
            WTable table = section.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row;
            WTableCell cell;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            var no = 1;
            var tanggal = "";
            foreach (var item in data)
            {
                tanggal = item.TanggalInput.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                row = table.AddRow(true, false);
                //Specifies the row height
                row.Height = 18;
                //Specifies the row height type
                row.HeightType = TableRowHeightType.AtLeast;

                //Column nomor row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(no.ToString()).CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no++;
                //Column perusahaan
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TransaksiDetail.TransaksiLaporan.Perusahaan.Nama);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TanggalInput.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TransaksiDetail.TransaksiLaporan.Perusahaan.Nama + " " + "telah melakukan pembelian kembali sebagai berikut : " + item.TransaksiDetail.TransaksiLaporan.Produk.Nama + " " + item.TransaksiDetail.TransaksiLaporan.NamaProduk + " " + item.TransaksiDetail.Seri);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Keterangan);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

            }
            document.Replace("%%TanggalInput%%", tanggal, false, true);
            document.Save("LaporanOJKKejadianPenting.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        } 

        public void LaporanOJKGagalBayar(Int64 Id, Int64 Produk, DateTime Tanggal)
        {
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "OJKGagalBayar");
            WordDocument document = new WordDocument(_path, FormatType.Docx);
            var data = _context.TransaksiKejadian
                .Include(x => x.TransaksiTanggal)
                .Include(x => x.TransaksiDetail)
                .Include(x => x.TransaksiDetail.TransaksiLaporan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Produk)
                .Where(x =>x.Id == Id && x.TransaksiDetail.Trans_Id == Produk && x.TanggalCetak == Tanggal)
                .ToList()
                .GroupBy(x => x.TransaksiDetail_Id)
                .Select(x => x.FirstOrDefault());
            var aturan = _context.AturanOJK.Single(x => x.Id == 9);
            var alamatOJK = _context.OJK.FirstOrDefault();

            document.Replace("%%Aturan%%", aturan.Aturan, false, true);
            document.Replace("%%IsiAturan%%", aturan.IsiAturan, false, true);
            document.Replace("%%Gedung%%", alamatOJK.Alamat1, false, true);
            document.Replace("%%Jalan%%", alamatOJK.Alamat2, false, true);
            document.Replace("%%Kota%%", alamatOJK.Alamat3, false, true);


            IWTextRange textRange;
            WSection section = document.Sections[0];
            WTable table = section.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row;
            WTableCell cell;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            var no = 1;
            //var tanggal = "";
            var produk = "";
            var namaproduk = "";
            foreach (var item in data)
            {
               // tanggal = item.TanggalInput.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                produk = item.TransaksiDetail.TransaksiLaporan.Produk.Nama;
                namaproduk = item.TransaksiDetail.TransaksiLaporan.NamaProduk;
                row = table.AddRow(true, false);
                //Specifies the row height
                row.Height = 18;
                //Specifies the row height type
                row.HeightType = TableRowHeightType.AtLeast;

                //Column nomor row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(no.ToString()).CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no++;
                //Column perusahaan
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TransaksiDetail.TransaksiLaporan.Perusahaan.Nama);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TanggalInput.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Informasi);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Keterangan);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

            }
            //document.Replace("%%TanggalInput%%", tanggal, false, true);
            document.Replace("%%Produk%%", produk, false, true);
            document.Replace("%%NamaProduk%%", namaproduk, false, true);
            document.Save("Informasi Material ke OJK (Terbatas).doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }

        public void LaporanOJKBuybackUMUM(Int64 Id, Int64 Produk, DateTime Tanggal)
        {
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "OJKBuybackUMUM");
            WordDocument document = new WordDocument(_path, FormatType.Docx);
            var data = _context.TransaksiKejadian
                .Include(x => x.TransaksiTanggal)
                .Include(x => x.TransaksiDetail)
                .Include(x => x.TransaksiDetail.TransaksiLaporan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                .Include(x => x.TransaksiDetail.TransaksiLaporan.Produk)
                .Where(x => x.Id == Id && x.TransaksiDetail.Trans_Id == Produk && x.TanggalCetak == Tanggal
                && x.TransaksiDetail.TransaksiLaporan.isDelete == false)
                .ToList()
                .GroupBy(x => x.TransaksiDetail_Id)
                .Select(x => x.FirstOrDefault());
            //var aturan = _context.AturanOJK.Single(x => x.Id == Aturan);
            var alamatOJK = _context.OJK.FirstOrDefault();

            //document.Replace("%%Aturan%%", aturan.Aturan, false, true);
            //document.Replace("%%IsiAturan%%", aturan.IsiAturan, false, true);
            document.Replace("%%Gedung%%", alamatOJK.Alamat1, false, true);
            document.Replace("%%Jalan%%", alamatOJK.Alamat2, false, true);
            document.Replace("%%Kota%%", alamatOJK.Alamat3, false, true);


            IWTextRange textRange;
            WSection section = document.Sections[1];
            WTable table = section.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row;
            WTableCell cell;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
            var no = 1;
            var tanggal = "";
            foreach (var item in data)
            {
                tanggal = item.TanggalInput.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                row = table.AddRow(true, false);
                //Specifies the row height
                row.Height = 18;
                //Specifies the row height type
                row.HeightType = TableRowHeightType.AtLeast;

                //Column nomor row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(no.ToString()).CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no++;
                //Column perusahaan
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TransaksiDetail.TransaksiLaporan.Perusahaan.Nama);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TanggalInput.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TransaksiDetail.TransaksiLaporan.Perusahaan.Nama + " " + "telah melakukan pembelian kembali sebagai berikut : " + item.TransaksiDetail.TransaksiLaporan.Produk.Nama + " " + item.TransaksiDetail.TransaksiLaporan.NamaProduk + " " + item.TransaksiDetail.Seri);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Keterangan);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

            }
            document.Replace("%%TanggalInput%%", tanggal, false, true);
            document.Save("Laporan Kejadian Penting OJK (Umum).doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }

    }
}