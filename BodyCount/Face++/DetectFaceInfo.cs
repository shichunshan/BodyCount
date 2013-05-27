using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceppSDK;
using System.Windows;
using Point = System.Windows.Point;

namespace Face__
{
    public class DetectFaceInfo
    {
        private readonly int DepthImageFramePerSecond = 30;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DwellTime { get; set; }
        public TimeSpan TotalStayTime  { get; set; }
        public Point BodyPostion { get; set; }
        public int Age { get; set; }
        public int AgeRange { get; set; }
        public string  Gender { get; set; }
        public double GenderConfidence { get; set; }
        public string Race { get; set; }
        public double RaceConfidence { get; set; }

        public DetectFaceInfo()
        {
        }

        public DetectFaceInfo(DetectResult detectResult, string path)
        {
            string[] strings = path.Split('_');
            if (detectResult.face.Count>0)
            {
                Face face = detectResult.face.SingleOrDefault();
                Age = face.attribute.age.value;
                AgeRange = face.attribute.age.range;

                Gender = face.attribute.gender.value;
                GenderConfidence = face.attribute.gender.confidence;

                Race = face.attribute.race.value;
                RaceConfidence = face.attribute.race.confidence;
            }
           // long ticks = long.Parse(strings[0]);
           // TimeSpan timeSpan=new TimeSpan(ticks);
           // DateTime CentryBegin = new DateTime(2001,1,1);
           //// Time  = CentryBegin.Add(timeSpan);

           //// TrackingID = Int32.Parse(strings[1]);

           // int temp = Int32.Parse(strings[2]);
           //// TotalStayTime = temp/DepthImageFramePerSecond;
           // if (detectResult.face.Count>0)
           // {
           //   // FaceAttribute face = detectResult.face.FirstOrDefault().attribute;
           // }
        }
    }
}
