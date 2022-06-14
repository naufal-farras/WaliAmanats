using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    [Table("Produk")]
    public class Produk : BaseModel
    {
        public string Nama { get; set; }
        public string Initial { get; set; }
    }
}