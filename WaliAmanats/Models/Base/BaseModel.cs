using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaliAmanats.Models.Base
{
    public class BaseModel
    {
        [Key]
        public Int64 Id { get; set; }
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;
    }
}