using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace BodyCount
{
    public class TrackingTime:INotifyPropertyChanged
    {
        private DateTime startTime;

        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                OnPropertyChanged("StartTime");
            }
        }

        private DateTime endTime;
        public DateTime EndTime
        {
            get
            {
                return endTime;
            }
            set {
                endTime = value;
                OnPropertyChanged("EndTime");
            }
        }   

        private int index;
            
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged("Index");
            }
        }
        
        private Int32 trackingID;

        public Int32 TrackingID
        {
            get { return trackingID; }
            set { 
                trackingID = value; 
                OnPropertyChanged("TrackingID");
            }
        }
        private float time;

        public float Time
        {
            get { return (float)totalStayTime/30; }
            set {
                time = value;
                OnPropertyChanged("Time");
            }
        }

        private double  dewelltime;

        public double  DewellTime
        {
            get { return dewelltime; }
            set { dewelltime = value; }
        }
        

        private int shotCount;

        public int ShotCount
        {
            get { return shotCount; }
            set { shotCount = value; }
        }
        
        public TrackingTime()
        {
            index = 0;
            trackingID = 0;
            time = 0;
            staytime = 0;
            currentskeletonpoint=new SkeletonPoint();
            shotCount = 0;
            dewelltime = 0;
        }
        private int staytime;

        public int StayTime
        {
            get { return staytime; }
            set
            {
                staytime = value;
                 OnPropertyChanged("StayTime");
            }
        }

        private int totalStayTime;

        public int TotalStayTime
        {
            get { return totalStayTime; }
            set
            {
                totalStayTime = value;
                OnPropertyChanged("TotalStayTime");
            }
        }
        

        private SkeletonPoint currentskeletonpoint;

        public SkeletonPoint  CurrentSkeletonPoint
        {
            get { return currentskeletonpoint; }
            set
            {
                currentskeletonpoint = value;
                OnPropertyChanged("CurrentSkeletonPoint");
            }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(info));
        }
    }
}
