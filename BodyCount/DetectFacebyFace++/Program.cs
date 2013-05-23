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


namespace DetectFacebyFace__
{
    class Program
    {
        public static FaceService fs ;
        private static string savepath;
        private static string detectpath;
        private static List<string> haveDetectedFilePath; 

        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            
            string stringnow = now.ToString();
            //Console.WriteLine(Convert.ToDateTime(stringnow, String.));
            Console.ReadKey();
            // timer.Stop();
        }

        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<string> filePaths = ReadFilePath(savepath);

            foreach (var filePath in filePaths)
            {
                if (filePath != null)
                {
                    Detect(filePath);
                    MoveFile(filePath);
                }
            }
            Console.ReadKey();
        }

      
        private static void Init()
        {
            savepath = "C:\\Users\\Dev01\\Desktop\\SaveImage";
            detectpath = "C:\\Users\\Dev01\\Desktop\\DetectImage\\";
            fs = new FaceService("2affcadaeddd18f422375adc869f3991", "EsU9hmgweuz8U-nwv6s4JP-9AJt64vhz");
        }

        private static void Detect(string filePath)
        {
            try
            {
               // Console.WriteLine(filePath+" detected!");
                DetectResult detect = new DetectResult();
                DetectResult res = fs.Detection_DetectImg(filePath);
                List<Face> faceList = new List<Face>();
                for (int i = 0; i < res.face.Count; i++)
                {
                    faceList.Add(res.face[i]);
                }
                // Insert code to set properties and fields of the object.
                XmlSerializer mySerializer = new XmlSerializer(typeof(List<Face>));
                // To write to a file, create a StreamWriter object.
                StreamWriter myWriter = new StreamWriter("detectResult.xml", true);
                mySerializer.Serialize(myWriter, faceList);
                myWriter.Close();
                //filePath.Delete();
            }
            catch (Exception ex
                )
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
            
        }

       
        private static List<string> ReadFilePath(string path)
        {
            List<string> filePaths = new List<string>();
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                FileInfo[] fileInfos = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
                foreach (var fileInfo in fileInfos)
                {
                    filePaths.Add(directoryInfo +"\\"+ fileInfo.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            return filePaths;
        }

        

      
        private static int maxInt(int x, int y)
        {
            return (x > y) ? x : y;
        }

        private static void MoveFile(string filepath)
        {
            try
            {
                FileInfo file=new FileInfo(filepath);
                file.MoveTo(detectpath+file.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
