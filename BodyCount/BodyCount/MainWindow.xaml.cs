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
using BodyCount.Properties;
using Microsoft.Kinect;

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

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            this.listBox.ItemsSource = trackingTimeList;
            croppedImageWidth = 200;
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


                    //Kinect only get the position
                    int noTrac = (from s in skeletons
                                  where s.TrackingState == SkeletonTrackingState.PositionOnly
                                  select s).Count();
                    
                    //kinect tracked
                    int getTrac = (from s in skeletons
                                   where s.TrackingState == SkeletonTrackingState.Tracked
                                   select s).Count();

                    int numberOfSkeletons = noTrac + getTrac;
                    //output the number in textbox
                    //Debug.WriteLine("Begin:");
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
                                trackingTimeTemp.TrackingID = skeleton.TrackingId;
                                trackingTimeTemp.Time = 0;
                                trackingTimeTemp.Index = trackingTimeIndex++;
                                trackingTimeList.Add(trackingTimeTemp);
                                listBox.SelectedItem = listBox.Items[listBox.Items.Count - 1];
                               
                            }
                          

                            UpdateTrackingTime(skeleton);
                        }
                        //update bodyID and time
                       
                    }
                   // UpdateTrackingTime(currentTrackingIDList);
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
                    trackingTime.Time = trackingTimeList[i].Time + 1;
                    trackingTime.TrackingID = trackingTimeList[i].TrackingID;
                    trackingTime.Index = trackingTimeList[i].Index;
                    trackingTime.StayTime = trackingTimeList[i].StayTime;
                    trackingTime.CurrentSkeletonPoint = trackingTimeList[i].CurrentSkeletonPoint;
                    trackingTime.ShotCount = trackingTimeList[i].ShotCount;
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
                        if (trackingTimeList[i].ShotCount==0)
                        {
                            SaveImage(position, trackingTimeList[i].TrackingID);
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

        private void UpdateTrackingTime(List<int> currentTrackingIDList)
        {
            if (currentTrackingIDList!=null)
            {
                foreach (var currentID in currentTrackingIDList)
                {
                    for (int i = trackingTimeList.Count-1; i >=0; i--)
                    {
                        if (currentID == trackingTimeList[i].TrackingID)
                        {
                            TrackingTime trackingTime = new TrackingTime();
                            trackingTime.Time = trackingTimeList[i].Time + 1;
                            trackingTime.TrackingID = trackingTimeList[i].TrackingID;
                            trackingTime.Index = trackingTimeList[i].Index;
                            
                            trackingTimeList.RemoveAt(i);
                            trackingTimeList.Add(trackingTime);
                            break;
                        }
                    }
                }
            }
        }
        private void SaveImage(ColorImagePoint position,int trackingID)
        {
            _fn = GetNextFilename(trackingID);
            try
            {
               RenderTargetBitmap renderTargetBitmap=new RenderTargetBitmap(640,480,96,96,PixelFormats.Pbgra32);
                renderTargetBitmap.Render(colorImage);
                renderTargetBitmap.Freeze();
                int x = position.X - croppedImageWidth/2;
                if (x<0)
                {
                    x = 0;
                }
                CroppedBitmap croppedBitmap = new CroppedBitmap(renderTargetBitmap, new Int32Rect(x,0,croppedImageWidth,(int)renderTargetBitmap.Height));
                string ext = System.IO.Path.GetExtension(_fn).ToLower();
                BitmapEncoder encoder = new PngBitmapEncoder();
                if (encoder == null) return;
                encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
               
                using (Stream stm = File.Create(_fn))
                {
                    encoder.Save(stm);
                }
               
            }
            catch (Exception x)
            {
                MessageBox.Show("Sorry, I had trouble saving that photo.\n\nError: " + x.Message);
                //IsAutoEnabled = false;
                // timer.Stop();
            }

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
       
    }
}
