using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    public class Status : BaseModel
    {
        public string Nama { get; set; }
        public string Warna { get; set; }
    }
}