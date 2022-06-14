using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    public class Rating : BaseModel
    {
        public string Nama { get; set; }
        public decimal Nilai { get; set; }
        public bool IsDelete { get; set; }
    }
}