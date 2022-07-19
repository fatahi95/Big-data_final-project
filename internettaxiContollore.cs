using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;
using System.Text;
using WebApplication2.Model;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class internettaxiContollore : Controller
    {
        intenettaxiContext _context;
        public internettaxiContollore(intenettaxiContext context)
        {
            _context = context;
        } 
        //Display the information of the last thousand trips
        [HttpGet(Name = "Getdata")]
        public ActionResult Get()
        {
            var data = _context.Data.Take(20);
            return new ObjectResult(data);
        }
        [HttpGet("GetAndDisplayThousendTrips", Name = "GetAndDisplayThousendTrips")]
        public ActionResult GetAndDisplayThousendTrips()
        { 
            var client = new RedisClient();
            client.FlushAll();
            var currentDate = DateTime.Now;
            double hours = 0;
            double minutes = 0;
            double day = 0;
            double Totaltime = 0;
            string key = "";
            string data= "";
            List<int> integers = new List<int>();
            double Division = new int();
            int count = 0;
            foreach (var item in _context.Data)
            {
                if (currentDate.Date.Month >= item.DateTime.Month )
                {
                    hours = item.DateTime.TimeOfDay.Subtract(currentDate.TimeOfDay).Hours;
                    minutes = item.DateTime.TimeOfDay.Subtract(currentDate.TimeOfDay).Minutes;
                    day = item.DateTime.DayOfYear - currentDate.DayOfYear;
                    Totaltime = minutes + (60 * hours) +(1440 * day);
                    Division = (double)Totaltime / 60.0;
                    if (Division < 0 && Division >= -2)
                    {
                        data = item.DateTime.ToString() + "#" + item.Base.ToString() + "#" + item.Lon.ToString() + "#" + item.Lat.ToString();
                        key = Math.Floor(Division).ToString();
                        if (count >= 6001) break;
                        client.SAdd(key, Encoding.UTF8.GetBytes(data));
                        
                        count++;

                        //client.Expire(key, 604800);
                    }
                    if (Division < -20 && Division >= -21)
                    {
                        data = item.DateTime.ToString() + "#" + item.Base.ToString() + "#" + item.Lon.ToString() + "#" + item.Lat.ToString();
                        key = Math.Floor(Division).ToString();

                        client.SAdd(key, Encoding.UTF8.GetBytes(data));
                        count++;
                        //client.Expire(key, 604800);
                    }
                }
            }
            count = 0;
            int i = 0;
            List<string> list = new List<string>();
            while (count <= 500)
            {
                var variable1 = client.SMembers(i.ToString());
                foreach (var item in variable1)
                {
                    data = Encoding.UTF8.GetString(item);
                    list.Add(data);
                    count++;
                    if (count > 500) break;
                }
                i--;
            }
            int j = list.Count;
            return Ok(list);
        }

        // Number of trips one hour before
        [HttpGet("TripsAnHourAgo", Name = "TripsAnHourAgo")]
        public IActionResult GetTripsAnHourAgo()
        {
            var client = new RedisClient();
            var currentDate = DateTime.Now;
            string data = "";
            List<int> integers = new List<int>();
            int i = -1;
            var variable1 = client.SMembers(i.ToString());
            List<string> list = new List<string>();
            foreach (var item in variable1)
            {
                data = Encoding.UTF8.GetString(item);
                list.Add(data);
            }
            int count = list.Count();
            return Ok(count);
        }

        // The number of trips in the previous hour and at a specific point 
        [HttpGet("TripsByTimeAndPlace", Name = "TripsByTimeAndPlace")]
        public IActionResult GetTripsByTimeAndPlace()
        {
            var client = new RedisClient();
            var currentDate = DateTime.Now;
            string data = "";
            List<int> integers = new List<int>();
            List<string> list = new List<string>();
            for (int i = -6; i < 0; i++)
            {
                var variable1 = client.SMembers(i.ToString());
                foreach (var item in variable1)
                {
                    data = Encoding.UTF8.GetString(item);
                    String[] passengerData = data.Split('#');
                    if (passengerData.Contains("B02617"))
                        list.Add(data);
                }
            }
            int counter = list.Count();
            return Ok(counter);
        }

        [HttpGet("GetTripsByIntervalTime", Name = "GetTripsByIntervalTime")]
        public ActionResult<string> GetTripsByTimeInterval()
        {
            var client = new RedisClient();
            int counter = 0;
            int length = 0;
            for (int i = -22; i < -20; i++)
            {
                length = client.SMembers(i.ToString()).Length;
                counter = length + counter;
            }
            return Ok(counter);
        }
    }
}
