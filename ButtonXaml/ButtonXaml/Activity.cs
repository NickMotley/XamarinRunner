using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class Activity : INotifyPropertyChanged
    {
        private TimeSpan totalDuration;
        //private bool runUpdate;

        public int Index { get; set; }
        public ActivityState ActivityState { get; set; }
        public TimeSpan RemainingDuration { get; set; }
        public TimeSpan PausedDuration { get; set; }

        public ICommand IncreaseDurationCommand { get; set; }
        public ICommand DecreaseDurationCommand { get; set; }

        public Activity()
        {
            this.DecreaseDurationCommand = new Command(DecreaseDuration);
            this.IncreaseDurationCommand = new Command(IncreaseDuration);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Activity " + this.Index.ToString() + " Timer";
            }
        }

        private void IncreaseDuration(object obj)
        {
            this.TotalDuration = this.TotalDuration.Add(TimeSpan.FromSeconds(1));
        }

        private void DecreaseDuration(object obj)
        {
            this.TotalDuration = this.TotalDuration.Add(TimeSpan.FromSeconds(-1));
        }

        public TimeSpan TotalDuration
        {
            get
            {
                return this.totalDuration;
            }
            set
            {
                this.totalDuration = value;
                this.OnPropertyChanged("TotalDuration");
            }
        }
        
        //private async void RunUpdateLoop()
        //{
        //    while (runUpdate)
        //    {
        //        await Task.Delay(1000);
        //        if (runUpdate)
        //        {
        //            if (this.RemainingDuration > TimeSpan.FromSeconds(0))
        //                this.DecreaseSeg1Count(null);
        //            else if (this.RemainingDuration > TimeSpan.FromSeconds(0))
        //                this.DecreaseSeg2Count(null);
        //            else if (this.Interval > 0)
        //            {
        //                this.Interval--;
        //                ResetTimes();
        //                this.DecreaseSeg1Count(null);
        //            }
        //        }
        //    }
        //}

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public enum ActivityState
    {
        Pending,
        Active,
        Paused
    }
}
