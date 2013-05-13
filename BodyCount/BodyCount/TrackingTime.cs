using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyCount
{
    public class TrackingTime:INotifyPropertyChanged
    {
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
        private Int32 time;

        public Int32 Time
        {
            get { return time; }
            set {
                time = value;
                OnPropertyChanged("Time");
            }
        }

        public TrackingTime()
        {
            index = 0;
            trackingID = 0;
            time = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(info));
        }
    }
}
