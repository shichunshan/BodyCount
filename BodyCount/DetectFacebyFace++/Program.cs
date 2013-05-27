using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Xml.Serialization;
using FaceppSDK;
using System.Windows.Media.Imaging;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Shard;


namespace DetectFacebyFace__
{
    class Program
    {
       // public static FaceService fs ;
        private static string savepath;
        private static string detectpath;
        private static List<string> haveDetectedFilePath; 

      
           static void Main(string[] args)
           {
               DateTime centryBegin = new DateTime(2001, 1, 1);
               DateTime now = DateTime.Now;
               long ticks = now.Ticks-centryBegin.Ticks;
               TimeSpan timeSpan = new TimeSpan(ticks);
               Console.WriteLine(timeSpan);
               Console.WriteLine(centryBegin.Add(timeSpan));
               Console.ReadKey();
	        }
	    }
	 public class User
	    {
	        public string Id { get; set; }
	        public string Name { get; set; }
	    }
	  
	}
