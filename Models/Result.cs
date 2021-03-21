using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flythrough.Models
{
    public class Result
    {
        public int id { get; set; }
        public string dataset { get; set; }
        public double? elevation { get; set; }
        public Location location { get; set; }
    }
}