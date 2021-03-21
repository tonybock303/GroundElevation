using Flythrough.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flythrough.Models
{
    public class WaypointGenerator
    {

        private string waypointText =
            @"{
                    duration: dur,
                    animate: function(phase)
        {
            var start = {0};
            var end = {1};
            var alt = [flightHeight, flightHeight];
            var target1 = {2};
            var target2 = {3};

            // interpolate both the camera position and target
            var position = lerp(start, end, phase);
            var altitude = lerp(alt[0], alt[1], phase);
            var target = lerp(target1, target2, phase);

            updateCameraPosition(position, altitude, target);
        }
    },
";
        public string GetWaypoints(List<CameraModel> cameras)
        {
            string waypoints = string.Empty;

            for (int i = 1; i < cameras.Count; i++)
            {
                var camera1LngLat = "[" + cameras[i - 1].Lng + "," + cameras[i - 1].Lat + "]";
                var camera2LngLat = "[" + cameras[i].Lng + "," + cameras[i].Lat + "]";
                var target1LngLat = "[" + cameras[i - 1].TargetLng + "," + cameras[i - 1].TargetLat + "]";
                var target2LngLat = "[" + cameras[i].TargetLng + "," + cameras[i].TargetLat + "]"; ;
                waypoints += @"
                {
                    duration: dur,
                    animate: function(phase)
                    {
                        var start = " + camera1LngLat + @";
                        var end = " + camera2LngLat + @";
                        var alt = [flightHeight, flightHeight];
                        var target1 = " + target1LngLat + @";
                        var target2 = " + target2LngLat + @";

                        var position = lerp(start, end, phase);
                        var altitude = lerp(alt[0], alt[1], phase);
                        var target = lerp(target1, target2, phase);

                        updateCameraPosition(position, altitude, target);
                    }
                },
                ";
            }

            return waypoints;

        }
    }
}