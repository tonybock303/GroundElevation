using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GroundElevation.Models
{
    public class dbLocation
    {
        [Key]
        [Column(Order = 1)]
        public string LatLong { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double Elevation { get; set; }     
        
        public dbLocation(){}

        public dbLocation(Result result)
        {
            Elevation = result.elevation == null ? 0 : (double)result.elevation;
            LatLong = result.location.lng.ToString() + "," + result.location.lat;
            Lng = result.location.lng;
            Lat = result.location.lat;
        }
    }
}