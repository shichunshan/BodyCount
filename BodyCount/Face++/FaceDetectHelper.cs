using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceppSDK;

namespace Face__
{
  public static class FaceDetectHelper
    {
      private static FaceService faceService=new FaceService("2affcadaeddd18f422375adc869f3991", "EsU9hmgweuz8U-nwv6s4JP-9AJt64vhz");
      public static readonly int DepthImageFramePerSecond=30;
      public static void faceDetect(string pngPath)
      {
          DetectResult detectResult = faceService.Detection_DetectImg(pngPath);
          DetectFaceInfo faceInfo=new DetectFaceInfo(detectResult,pngPath);
          
      }
    }
}
