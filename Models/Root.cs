using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroundElevation.Models
{
    public class Root
    {
        public int id { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }

    }
}