using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonXaml
{
    public class Rep : INotifyPropertyChanged
    {
        internal int Index { get; set; }
        internal TimerState ActivityState { get; set; }

        ObservableCollection<Activity> activities { get; set; }
        Activity currentActivity;

        public ObservableCollection<Activity> Activities
        {
            get
            {
                if (this.activities == null)
                {
                    this.activities = new ObservableCollection<Activity>();
                }
                return this.activities;
            }
            set
            {
                this.activities = value;
            }
        }

        public Activity CurrentActivity
        {
            get
            {
                return this.currentActivity;
            }
            set
            {
                this.currentActivity = value;
                this.OnPropertyChanged("CurrentActivity");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal event EventHandler<TimerStatusChangeEvent> StatusChanged;

        public string Name
        {
            get
            {
                return "Rep " + (this.Index+1).ToString();
            }
        }

        internal bool StartTimer()
        {
            this.CurrentActivity = this.Activities.OrderBy(x => x.Index).First(x => x.ActivityState == TimerState.Pending);
            Debug.WriteLine("Action: " + CurrentActivity.Name);
            this.CurrentActivity.PropertyChanged += CurrentActivity_PropertyChanged;
            this.CurrentActivity.StatusChanged += CurrentActivity_StatusChanged;

            return CurrentActivity.StartTimer();
        }

        private void CurrentActivity_StatusChanged(object sender, TimerStatusChangeEvent e)
        {
            if (e.Status == TimerState.Complete)
            {
                if (this.Activities.OrderBy(x => x.Index).Any(x => x.ActivityState == TimerState.Pending))
                {
                    StartTimer();
                }
                else
                {
                    this.ActivityState = TimerState.Complete;
                    this.OnStatusChanged(this.ActivityState);
                }
            }
        }

        internal bool PauseTimer()
        {
            return CurrentActivity.PauseTimer();
        }

        internal bool ResumeTimer()
        {
            return CurrentActivity.ResumeTimer();
        }

        private void CurrentActivity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }

        private void OnStatusChanged(TimerState status)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new TimerStatusChangeEvent(status));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
