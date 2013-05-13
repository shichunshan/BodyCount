using System;
using System.Collections.Generic;
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
        private List<Int32> _trackingIdList=new List<int>();
        private int BodyCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

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
            kinectSensor.Start();
        }

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
                            if (!_trackingIdList.Contains(skeleton.TrackingId))
                            {
                                BodyCount++;
                            }
                        }
                        
                    }
                    _trackingIdList.Clear();
                    foreach (var trackID in currentTrackingIDList)
                    {
                       _trackingIdList.Add(trackID);
                    }
                    this.textBox1.Text = BodyCount.ToString();
                }

            } 

        }
    }
}
