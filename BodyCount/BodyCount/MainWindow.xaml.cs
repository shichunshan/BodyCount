using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
       
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            this.listBox.ItemsSource = trackingTimeList;
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
            kinectSensor.DepthStream.Enable();
            kinectSensor.DepthFrameReady += kinectSensor_DepthFrameReady;
            kinectSensor.Start();
        }

        //void kinectSensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        //{
        //    using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
        //    {
        //        if (depthImageFrame != null)
        //        {
        //            depthImagePixel = new short[depthImageFrame.PixelDataLength];
        //            depthImageFrame.CopyPixelDataTo(depthImagePixel);
        //            writeableBitmap.WritePixels(int32Rect, depthImagePixel, stride, offset);
        //        }

        //    }
        //}

        void kinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            List<Int32> currentTrackingIDList=new List<int>();
            Skeleton[] skeletons = new Skeleton[0];

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
                        }
                    }
                    UpdateTrackingTime(currentTrackingIDList);
                    oldTrackingIdList.Clear();
                    foreach (var trackID in currentTrackingIDList)
                    {
                       oldTrackingIdList.Add(trackID);
                    }
                    this.textBox1.Text = BodyCount.ToString();
                }

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
       
    }
}
