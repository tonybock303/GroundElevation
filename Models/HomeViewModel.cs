using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Flythrough.Models
{
    public class HomeViewModel
    {
        private double kmInDegree = 111;
        private double feetInMeter = 3.370079;
        public int Id { get; set; }
        public string StartCoords { get; set; }
        public string FinishCoords { get; set; }
        public int GroundClearanceRequired { get; set; } = 100;
        public int OstacleDistance { get; set; }
        public int ObstacleHeight { get; set; }
        [NotMapped]
        public double StartCoordsLng { get { return StartCoords == null ? 0 : double.Parse(StartCoords.Split(',').Last()); } }
        [NotMapped]
        public double StartCoordsLat { get { return StartCoords == null ? 0 : double.Parse(StartCoords.Split(',').First()); } }
        [NotMapped]
        public double FinishCoordsLng { get { return FinishCoords == null ? 0 : double.Parse(FinishCoords.Split(',').Last()); } }
        [NotMapped]
        public double FinishCoordsLat { get { return FinishCoords == null ? 0 : double.Parse(FinishCoords.Split(',').First()); } }
        [NotMapped]
        public List<Result> Results { get; set; }
        [NotMapped]
        public double TotalDistance { get { return Math.Sqrt(Math.Pow(FinishCoordsLat - StartCoordsLat, 2) + Math.Pow(FinishCoordsLng - StartCoordsLng, 2)) * kmInDegree * feetInMeter; } }
        [NotMapped]
        public double MidCoordLat
        {
            get
            {
                return StartCoordsLat + (TotalDistance /2) / TotalDistance * (FinishCoordsLat - StartCoordsLat);
            }
        }
        [NotMapped]
        public double MidCoordLng
        {
            get
            {
                return StartCoordsLng + (TotalDistance / 2) / TotalDistance * (FinishCoordsLng - StartCoordsLng);
            }
        }
    }
}