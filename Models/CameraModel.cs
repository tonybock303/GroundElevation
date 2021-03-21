using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Flythrough.Models
{
    public class CameraModel
    {
        [Key]
        public int Id { get; set; }
        public string TextLine { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double Elevation { get; set; } = -1;
        public double GroundElevation { get; set; } = 200;
        public double Duration { get; set; } = 20;
        public double TargetLat { get; set; }
        public double TargetLng { get; set; }
        public double TargetElevation { get; set; } = 500;
        public double TargetGroundElevation { get; set; }

        public CameraModel()
        { }
        public CameraModel(string waypoint, string target, string elevation)
        {
            if (string.IsNullOrEmpty(waypoint))
            {
                throw new System.ArgumentException();
            }
            TextLine = waypoint;
            
            Lng = double.Parse(waypoint.Split(',').First());
            Lat = double.Parse(waypoint.Split(',').Last());

            TargetLng = double.Parse(target.Split(',').First());
            TargetLat = double.Parse(target.Split(',').Last());
            Elevation = double.Parse(elevation);
        }
        public double[] GetCameraLngLat()
        {
            return new double[] { Lng, Lat };
        }
        public double[] GetTargetLngLat()
        {
            return new double[] { TargetLng, TargetLat };
        }


        //[NotMapped]
        //public double TotalDistance { get { return Math.Sqrt(Math.Pow(FinishCoordsLat - StartCoordsLat, 2) + Math.Pow(FinishCoordsLng - StartCoordsLng, 2)) * kmInDegree * feetInMeter; } }
        //[NotMapped]
        //public double MidCoordLat
        //{
        //    get
        //    {
        //        return StartCoordsLat + (TotalDistance / 2) / TotalDistance * (FinishCoordsLat - StartCoordsLat);
        //    }
        //}
        //[NotMapped]
        //public double MidCoordLng
        //{
        //    get
        //    {
        //        return StartCoordsLng + (TotalDistance / 2) / TotalDistance * (FinishCoordsLng - StartCoordsLng);
        //    }
        //}
    }
}