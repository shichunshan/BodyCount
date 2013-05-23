using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceppSDK;

namespace Face__
{
    public class DetectFaceInfo
    {
        private readonly int DepthImageFramePerSecond = 30;


        public string Time { get; set; }
        public FaceppSDK.Attribute FaceAttribute { get; set; }
        public int TrackingID { get; set; }
        public int TotalStayTime { get; set; }

        public DetectFaceInfo()
        {
        }

        public DetectFaceInfo(DetectResult detectResult, string path)
        {
            string[] strings = path.Split('_');

            Time = strings[0];

            TrackingID = Int32.Parse(strings[1]);

            int temp = Int32.Parse(strings[2]);
            TotalStayTime = temp/DepthImageFramePerSecond;

            FaceAttribute = detectResult.face.FirstOrDefault().attribute;

        }
    }
}
