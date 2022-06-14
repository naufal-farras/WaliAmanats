using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaliAmanats.ViewModel
{
    public class FilterVM
    {
        public List<string> Emiten { get; set; }
        public List<string> Jenis { get; set; }
        public List<string> Produk { get; set; }
        public List<string> Batas { get; set; }
        public List<string> Measure { get; set; }
        public List<string> Ratio { get; set; }
        public List<string> Keterangan { get; set; }
        public List<string> Jaminan { get; set; }
        public List<string> Kejadian { get; set; }
    }
}