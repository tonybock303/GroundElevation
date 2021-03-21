using Flythrough.Data;
using Flythrough.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Flythrough.Controllers
{
    public class HomeController : Controller
    {
        private readonly double personHeight = 5;
        FlythroughContext context = new FlythroughContext();

        private double kmInDegree = 111;
        private double feetInMeter = 3.370079;
        public ActionResult Index()
        {
            FlythroughModel model = new FlythroughModel();

            object mod = Session["Settings"];
            if (mod != null)
            {
                string str = (string)mod;
                model = JsonConvert.DeserializeObject<FlythroughModel>(str);
            }

            return View("WayPoints", model);
        }

        public ActionResult WayPoints()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Index(FormCollection waypoints)
        {
            FlythroughModel model = new FlythroughModel();
            var keys = waypoints.AllKeys.ToList();
            var waypointCount = keys.Where(x => x.Contains("waypoint_")).Count();
            string URL = "https://api.opentopodata.org/v1/eudem25m";
            string urlParameters = @"?locations=";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("*/*"));
            for (int i = 0; i < waypointCount; i++)
            {
                string waypoint = Request.Form["waypoint_" + i];
                string target = Request.Form["target_" + i];
                string elevation = Request.Form["elevation_" + i];
                CameraModel cam = new CameraModel(waypoint, target, elevation);
                urlParameters += Math.Round(cam.Lat, 6) + "," + Math.Round(cam.Lng, 6) + (i != waypointCount - 1 ? "|" : "");
                model.Cameras.Add(cam);

            }
            URL += urlParameters;
            List<Result> resultList = new List<Result>();
            try
            {
                string response = client.GetStringAsync(URL).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response != null)
                {
                    Root elevationResults = JsonConvert.DeserializeObject<Root>(response);
                    for (int i = 0; i < elevationResults.results.Count(); i++)
                    {
                        double num = elevationResults.results[i].elevation == null ? 0 : (double)elevationResults.results[i].elevation;
                        model.Cameras[i].GroundElevation = Math.Round(num * feetInMeter, 1);
                    }
                    resultList = elevationResults.results;
                }
                else
                {
                    throw new HttpException();
                }
            }
            catch (Exception ex)
            {
                // I dunno
            }
            string mod = JsonConvert.SerializeObject(model);
            Session["Settings"] = mod;
            //System.IO.File.WriteAllText("settings.json", mod);

            return View("Fullscreen", model);
        }


        //[HttpPost]
        //public ActionResult Index(HomeViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        context.Entry(model).State = EntityState.Modified;
        //        context.SaveChanges();
        //    }

        //    string csvCoords = model.StartCoords.Trim() + "|" + model.FinishCoords.Trim() + "|";
        //    csvCoords += model.GroundClearanceRequired + "|" + model.ObstacleHeight + "|" + model.OstacleDistance;
        //    string startString = csvCoords.Split('|').ElementAt(0);
        //    string finishString = csvCoords.Split('|').ElementAt(1);
        //    int groundClearance = int.Parse(csvCoords.Split('|').ElementAt(2));
        //    int obstacleHeight = int.Parse(csvCoords.Split('|').ElementAt(3));
        //    int obstacleDistance = int.Parse(csvCoords.Split('|').ElementAt(4));
        //    double startX = double.Parse(startString.Split(',').First());
        //    double startY = double.Parse(startString.Split(',').Last());
        //    double finishX = double.Parse(finishString.Split(',').First());
        //    double finishY = double.Parse(finishString.Split(',').Last());
        //    model.Results = CallElevationApi2(startX, startY, finishX, finishY, groundClearance, obstacleHeight, obstacleDistance).ToList();


        //    return View("Display", model);
        //}

        //private IEnumerable<Result> CallElevationApi2()
        //{
        //    int intervals = 20;
        //    IEnumerable<Result> resultList = new List<Result>();
        //    string URL = "https://api.opentopodata.org/v1/eudem25m";
        //    string urlParameters = @"?locations=";
        //    List<double[]> coords = new List<double[]>();
        //    double[] intervalsArray = new double[intervals];
        //    for (int i = 0; i < intervals; i++)
        //    {
        //        double[] point = new double[2];
        //        point[0] = x1 + ((intervalDistance * i / totalDistance) * (x2 - x1));
        //        point[1] = y1 + ((intervalDistance * i / totalDistance) * (y2 - y1));
        //        coords.Add(point);
        //        urlParameters += Math.Round(point[0], 6) + "," + Math.Round(point[1], 6) + (i != intervals - 1 ? "|" : "");
        //        intervalsArray[i] = (int)Math.Round(i * intervalDistance * 1000, 0);
        //    }


        //    URL += urlParameters;
        //    HttpClient client = new HttpClient();
        //    client.BaseAddress = new Uri(URL);

        //    client.DefaultRequestHeaders.Accept.Add(
        //    new MediaTypeWithQualityHeaderValue("*/*"));
        //    double[] treeLineArray = new double[intervals];


        //    // List data response.

        //    while (resultList.Count() == 0)
        //    {
        //        try
        //        {
        //            string response = client.GetStringAsync(URL).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
        //            if (response != null)
        //            {
        //                Root elevationResults = JsonConvert.DeserializeObject<Root>(response);
        //                resultList = elevationResults.results;
        //                for (int i = 0; i < resultList.Count(); i++)
        //                {
        //                    resultList.ElementAt(i).elevation = resultList.ElementAt(i).elevation == null ? 0 : resultList.ElementAt(i).elevation;
        //                    resultList.ElementAt(i).elevation *= feetInMeter;
        //                    if (i * intervalDistance * 1000 < obstacleDistance)
        //                    {
        //                        if (i + 1 * intervalDistance * 1000 > obstacleDistance)
        //                        {
        //                            treeLineArray[i] = (double)resultList.ElementAt(i).elevation + obstacleHeight;
        //                        }
        //                        else
        //                        {
        //                            treeLineArray[i] = (double)resultList.ElementAt(i).elevation;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        treeLineArray[i] = (double)resultList.ElementAt(i).elevation + groundClearance;
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            System.Console.Beep();
        //            Thread.Sleep(10000);
        //        }
        //    }
        //    double peakAscend = -200;
        //    int indexOfPeak = intervals - 1;
        //    double startHeight = (double)resultList.First().elevation + personHeight;
        //    for (int i = 0; i < intervals; i++)
        //    {
        //        double currentHeight = (double)resultList.ElementAt(i).elevation + treeLineArray[i];
        //        double intervalHeight = currentHeight - startHeight;
        //        double rateOfAscend = intervalHeight / i;
        //        double distanceToCurrent = intervalDistance * 1000 * i;
        //        double clearance = groundClearance * 3;
        //        if (peakAscend < rateOfAscend )
        //        {
        //            var distanceToLast = distanceToCurrent - (intervalDistance * 1000);
        //            if ((distanceToLast < obstacleDistance) && distanceToCurrent >= obstacleDistance)
        //            {
        //                peakAscend = rateOfAscend;
        //                indexOfPeak = i;
        //            }
        //            else if (distanceToCurrent >= obstacleDistance)
        //            {
        //                peakAscend = rateOfAscend;
        //                indexOfPeak = i;
        //            }
        //        }
        //    }

        //    double startElevation = (double)resultList.ToList().ElementAt(0).elevation + personHeight;
        //    double peakElevation = (double)resultList.ElementAt(indexOfPeak).elevation;
        //    double peakClearanceElevation = peakElevation + groundClearance + 1; //Keep flight height above trees
        //    double elevationIncreaseToPeak = peakClearanceElevation - startElevation;
        //    double flightHeightIntervals = (elevationIncreaseToPeak) / indexOfPeak;
        //    double[] flightHeightArray = new double[intervals];
        //    for (int i = 0; i < intervals; i++)
        //    {
        //        flightHeightArray[i] = (double)(i * flightHeightIntervals) + startElevation;
        //    }
        //    List<double?[]> deadZones = new List<double?[]>();
        //    for (int loop = 1; loop < intervals; loop++)
        //    {
        //        var currentPeakElevation = (double)resultList.ElementAt(loop).elevation;
        //        double elevationIncreaseToDeadZone = currentPeakElevation - startElevation;
        //        double deadZoneHeightIntervals = (elevationIncreaseToDeadZone) / loop;
        //        double?[] deadZoneHeightArray = new double?[intervals];
        //        bool hitLand = false;
        //        int numberOfDeadzoneLInes = 20;
        //        for (int i = 0; i < numberOfDeadzoneLInes; i++) // number of deadzones
        //        {
        //            double height = (double)(i * deadZoneHeightIntervals) + startElevation;
        //            //calculate index
        //            int index = (resultList.Count() / numberOfDeadzoneLInes) * i;
        //            double currentNodeHeight = (double)(resultList.ElementAt(index).elevation != null ? resultList.ElementAt(index).elevation : 0);
        //            if (height < currentNodeHeight) //make it less fussy creating deadzones
        //            {
        //                hitLand = true;
        //            }
        //            if (hitLand && height > 0) deadZoneHeightArray[i] = height;
        //        }
        //        deadZones.Add(deadZoneHeightArray);
        //    }
        //    ViewBag.Intervals = intervalsArray;
        //    ViewBag.FlightHeight = flightHeightArray;
        //    ViewBag.TreeLineHeight = treeLineArray;
        //    ViewBag.IndexOfPoi = (int)(totalDistance / intervalDistance); 
        //    ViewBag.MultiArray = deadZones;


        //    // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
        //    client.Dispose();
        //    List<dbLocation> model = new List<dbLocation>();
        //    foreach (Result r in resultList)
        //    {
        //        dbLocation location = new dbLocation(r);
        //        var temp = context.dbLocations.Find(location.LatLong);
        //        if (temp == null)
        //        {
        //            temp = location;
        //            context.dbLocations.AddOrUpdate(x => x.LatLong, temp);
        //        }
        //    }

        //    context.SaveChanges();

        //    return resultList;

        //}
        public ActionResult Display()
        {
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult FullScreen(HomeViewModel model)
        {
            if (model == null)
            {
                return RedirectToAction("Index", "Home");

            }
            return View(model);
        }
    }
}