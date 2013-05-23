using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using BodyCount.Properties;
using FaceppSDK;
using Microsoft.Kinect;
using System.ComponentModel;
using WPFMediaKit;
using WPFMediaKit.DirectShow.Controls;

namespace BodyCount
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor;
        private List<Int32> oldTrackingIdList=new List<int>();
        private int BodyCount = 0;
        private ObservableCollection<TrackingTime> trackingTimeList = new ObservableCollection<TrackingTime>();
        private int trackingTimeIndex = 0;
        private WriteableBitmap writeableBitmap;
        private Int32Rect int32Rect;
        private int stride;
        private int offset = 0;
        private int frame = 0;
        private bool isStay = false;
        private string _fn;
        private readonly int croppedImageWidth;
        private BackgroundWorker backgroundWorker;
        public FaceService fs = new FaceService("2affcadaeddd18f422375adc869f3991", "EsU9hmgweuz8U-nwv6s4JP-9AJt64vhz");
        private string[] inputNames = MultimediaUtil.VideoInputNames;    

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            this.listBox.ItemsSource = trackingTimeList;
            croppedImageWidth = 300;
            videoElement.VideoCaptureSource = inputNames[0];
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            kinectSensor.Stop();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensor = KinectSensor.KinectSensors.FirstOrDefault();
            if (kinectSensor==null)
            {
                MessageBox.Show("This application requires a Kinect sensor.");
            }
            kinectSensor.SkeletonStream.Enable();
            kinectSensor.SkeletonFrameReady += kinectSensor_SkeletonFrameReady;
            kinectSensor.ColorStream.Enable();
            kinectSensor.ColorFrameReady +=kinectSensor_ColorFrameReady;
            this.writeableBitmap = new WriteableBitmap(kinectSensor.ColorStream.FrameWidth, kinectSensor.ColorStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null);
            stride = kinectSensor.ColorStream.FrameWidth * kinectSensor.ColorStream.FrameBytesPerPixel;
            int32Rect = new Int32Rect(0, 0, kinectSensor.ColorStream.FrameWidth,kinectSensor.ColorStream.FrameHeight);
            this.colorImage.Source = writeableBitmap;
            kinectSensor.Start();
            //detect faces on background
            backgroundWorker=new BackgroundWorker();
            backgroundWorker.DoWork += backgroundWorker_DoWork;
           // backgroundWorker.RunWorkerAsync();

        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            detectFace((string) e.Argument);
        }

        

        void kinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame ColorImageFrame = e.OpenColorImageFrame())
            {
                if (ColorImageFrame != null)
                {
                    var ColorImagePixel = new byte[this.kinectSensor.ColorStream.FramePixelDataLength];
                    ColorImageFrame.CopyPixelDataTo(ColorImagePixel);
                    writeableBitmap.WritePixels(int32Rect, ColorImagePixel, stride, offset);
                }

            }
        }

        void kinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            List<Int32> currentTrackingIDList=new List<int>();
            Skeleton[] skeletons = new Skeleton[0];
            bodyContainer.Children.Clear();
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                   
                    foreach (var skeleton in skeletons)
                    {
                        currentTrackingIDList.Add(skeleton.TrackingId);
                      
                        if (skeleton.TrackingState==SkeletonTrackingState.PositionOnly||
                            skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            if (!oldTrackingIdList.Contains(skeleton.TrackingId))
                            {
                                BodyCount++;
                                TrackingTime trackingTimeTemp=new TrackingTime();
                                trackingTimeTemp.StartTime = DateTime.Now;
                                trackingTimeTemp.TrackingID = skeleton.TrackingId;
                                trackingTimeTemp.Time = 0;
                                trackingTimeTemp.Index = trackingTimeIndex++;
                                trackingTimeList.Add(trackingTimeTemp);
                                listBox.SelectedItem = listBox.Items[listBox.Items.Count - 1];
                            }
                            UpdateTrackingTime(skeleton);
                        }
                    }

                    foreach (var bodytrackingID in oldTrackingIdList )
                    {
                        if (!currentTrackingIDList.Contains(bodytrackingID))
                        {
                            for (int i = trackingTimeList.Count - 1; i >= 0; i--)
                            {
                                if (bodytrackingID==trackingTimeList[i].TrackingID)
                                {
                                    TrackingTime trackingTime = trackingTimeList[i];
                                    trackingTime.EndTime = DateTime.Now;
                                    trackingTime.DewellTime = (trackingTime.EndTime - trackingTime.StartTime).TotalSeconds;
                                    trackingTimeList.RemoveAt(i);
                                    trackingTimeList.Add(trackingTime);
                                }
                            }
                        }    
                    }
                    //update oldTrackingIDList
                    oldTrackingIdList.Clear();
                    foreach (var trackID in currentTrackingIDList)
                    {
                       oldTrackingIdList.Add(trackID);
                    }
                    this.textBox1.Text = BodyCount.ToString();
                    
                }

            } 

        }

        private void UpdateTrackingTime(Skeleton skeleton)
        {
            frame++;
            ColorImagePoint position =
                              kinectSensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeleton.Position,
                                                                                         ColorImageFormat.RgbResolution640x480Fps30);
            BodyIDAndPostion  bodyIdAndPostion = new BodyIDAndPostion();;
            for (int i = trackingTimeList.Count - 1; i >= 0; i--)
            {
                if (skeleton.TrackingId == trackingTimeList[i].TrackingID)
                {
                    TrackingTime trackingTime = new TrackingTime();
                    trackingTime.StartTime = trackingTimeList[i].StartTime;
                    trackingTime.Time = trackingTimeList[i].Time + 1;
                    trackingTime.TrackingID = trackingTimeList[i].TrackingID;
                    trackingTime.Index = trackingTimeList[i].Index;
                    trackingTime.StayTime = trackingTimeList[i].StayTime;
                    trackingTime.CurrentSkeletonPoint = trackingTimeList[i].CurrentSkeletonPoint;
                    trackingTime.ShotCount = trackingTimeList[i].ShotCount;
                    trackingTime.TotalStayTime = trackingTimeList[i].TotalStayTime + 1;
                    float x = skeleton.Position.X - trackingTimeList[i].CurrentSkeletonPoint.X;
                    float y = skeleton.Position.Y - trackingTimeList[i].CurrentSkeletonPoint.Y;
                    float z = skeleton.Position.Z - trackingTimeList[i].CurrentSkeletonPoint.Z;
                    float distance = Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
                    bodyIdAndPostion.bodyIdTextblock.Text = skeleton.TrackingId.ToString();
                    if (frame>=10)
                    {
                        trackingTime.CurrentSkeletonPoint = skeleton.Position;
                        frame = 0;
                        if (distance<0.3)
                        {
                             trackingTime.StayTime = trackingTimeList[i].StayTime + 10;
                             isStay = true;
                        }
                        else
                        {
                            isStay = false;
                            trackingTime.StayTime = 0;
                        }
                    }
                    if (isStay)
                    {
                        bodyIdAndPostion.bodyIdTextblock.Background = new SolidColorBrush(new Color());
                    }
                    if (trackingTimeList[i].StayTime>=50)
                    {
                        trackingTime.StayTime = 0;
                        if (trackingTimeList[i].ShotCount<=0)
                        {
                            SaveImage(position, trackingTimeList[i]);
                            trackingTime.ShotCount = trackingTimeList[i].ShotCount+1;
                        }
                    }
                    Canvas.SetLeft(bodyIdAndPostion, position.X);
                    Canvas.SetTop(bodyIdAndPostion, position.Y);
                    bodyContainer.Children.Add(bodyIdAndPostion);
                   
                    trackingTimeList.RemoveAt(i);
                    trackingTimeList.Add(trackingTime);
                }
                break;
            }
        }

       
        private void SaveImage(ColorImagePoint position,TrackingTime trackingTime)
        {
            _fn = GetFilename(trackingTime);
            try
            {
               RenderTargetBitmap renderTargetBitmap=new RenderTargetBitmap(640,480,96,96,PixelFormats.Pbgra32);
                renderTargetBitmap.Render(imageCanvas);
                renderTargetBitmap.Freeze();
                int x = position.X - croppedImageWidth/2;
                if (x<0)
                {
                    x = 0;
                }
                int width = croppedImageWidth;
                if (x+width>renderTargetBitmap.Width)
                {
                    width = (int)renderTargetBitmap.Width - x;
                }
                CroppedBitmap croppedBitmap = new CroppedBitmap(renderTargetBitmap, new Int32Rect(x,0,width,(int)renderTargetBitmap.Height));
                string ext = System.IO.Path.GetExtension(_fn).ToLower();
                BitmapEncoder encoder = new PngBitmapEncoder();
                if (encoder == null) return;
                encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
               
                using (Stream stm = File.Create(_fn))
                {
                    encoder.Save(stm);
                }
               backgroundWorker.RunWorkerAsync(_fn);
            }
            catch (Exception x)
            {
                MessageBox.Show("Sorry, I had trouble saving that photo.\n\nError: " + x.Message);
                //IsAutoEnabled = false;
                // timer.Stop();
            }

        }

        private string GetFilename(TrackingTime trackingTime)
        {
            int num = 0;
            Settings s = Settings.Default;
            string fn = System.IO.Path.Combine(s.SaveLocation,
                                               trackingTime.StartTime.ToString("yyyyMMddHHmmss") + "_" + trackingTime.TrackingID + "_" +
                                               trackingTime.TotalStayTime +"_"+num.ToString()+
                                               ".png");
            while (File.Exists(fn))
            {
                num++;
                fn = System.IO.Path.Combine(s.SaveLocation,
                                               trackingTime.StartTime.ToString("yyyyMMddHHmmss") + "_" + trackingTime.TrackingID + "_" +
                                               trackingTime.TotalStayTime + "_" + num.ToString() +
                                               ".png");
            }
            return fn;
        }
        private string GetNextFilename(int trackingID)
        {
            int num = 0;
            Settings s = Settings.Default;
            string fn = System.IO.Path.Combine(s.SaveLocation, s.SaveBasename + trackingID +"_"+num.ToString()+ "." + s.SaveFormate);

            while (File.Exists(fn))
            {
                num++;
                fn = System.IO.Path.Combine(s.SaveLocation, s.SaveBasename + trackingID +"_"+num.ToString()+ "." + s.SaveFormate);
            }
            return fn;
        }

        private void detectFace(string path)
        {
            DetectResult detect = new DetectResult();
            DetectResult res = fs.Detection_DetectImg(path);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            WriteableBitmap wb = new WriteableBitmap(bi);

            List<Face> faceList = new List<Face>();
            for (int i = 0; i < res.face.Count; i++)
            {
                faceList.Add(res.face[i]);
                Int32Rect rect = new Int32Rect(maxInt((Int32)(res.face[i].center.x * wb.Width / 100.0 - res.face[i].width * wb.Width / 200.0), 0),
                                            maxInt((int)(res.face[i].center.y * wb.Height / 100.0 - res.face[i].height * wb.Height / 200.0), 0),
                                             (int)(res.face[i].width * wb.Width / 100.0), (int)((res.face[i].height * wb.Height) / 100.0));
                byte[] ColorData = { 0, 0, 255, 100 }; // B G R
                for (int j = 0; j < rect.Width; j++)
                {
                    Int32Rect rect1 = new Int32Rect(
                       j + rect.X,
                       rect.Y,
                       1,
                       1);

                    wb.WritePixels(rect1, ColorData, 4, 0);
                    Int32Rect rect2 = new Int32Rect(
                       j + rect.X,
                       rect.Y + rect.Height,
                       1,
                       1);

                    wb.WritePixels(rect2, ColorData, 4, 0);
                }
                for (int j = 0; j < rect.Height; j++)
                {
                    Int32Rect rect1 = new Int32Rect(
                       rect.X,
                        j + rect.Y,
                       1,
                       1);

                    wb.WritePixels(rect1, ColorData, 4, 0);
                    Int32Rect rect2 = new Int32Rect(
                        rect.X + rect.Width,
                       j + rect.Y,
                       1,
                       1);

                    wb.WritePixels(rect2, ColorData, 4, 0);
                }


            }
            var pngE1 = new PngBitmapEncoder();
            pngE1.Frames.Add(BitmapFrame.Create(wb));
            using (Stream stream = File.Create(path + ".jpg"))
            {
                pngE1.Save(stream);
            }
            // Insert code to set properties and fields of the object.
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<Face>));
            // To write to a file, create a StreamWriter object.
            
            StreamWriter myWriter = new StreamWriter("detectResult.xml", true);
            mySerializer.Serialize(myWriter, faceList);
            myWriter.Close();
        }

        private int maxInt(int x, int y)
        {
            return (x > y) ? x : y;
        }
       
    }
}
