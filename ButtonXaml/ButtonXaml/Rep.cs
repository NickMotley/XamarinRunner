using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ButtonXaml
{
    public class Rep : INotifyPropertyChanged
    {
        internal int Index { get; set; }
        internal TimerState ActivityState { get; set; }

        private DateTime startTime;

        ObservableCollection<UserActivity> userActivities;
        UserActivity currentActivity;

        public ObservableCollection<UserActivity> UserActivities
        {
            get
            {
                if (this.userActivities == null)
                {
                    this.userActivities = new ObservableCollection<UserActivity>();
                }
                this.userActivities.CollectionChanged += Activities_CollectionChanged;
                return this.userActivities;
            }
            set
            {
                this.userActivities = value;
            }
        }

        private void Activities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach( UserActivity activity in e.NewItems)
                {
                    activity.PropertyChanged += Activity_PropertyChanged;
                }
            }
        }

        private void Activity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TotalDuration")
            {
                this.OnPropertyChanged("TotalDuration");
            }
        }

        public UserActivity CurrentActivity
        {
            get
            {
                if (this.currentActivity == null)
                {
                    this.currentActivity = new UserActivity();
                }
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
                return "Set " + (this.Index+1).ToString();
            }
        }

        internal bool StartTimer()
        {
            if (this.ActivityState == TimerState.Pending)
            {
                this.startTime = DateTime.Now;
                this.ActivityState = TimerState.Active;
            }

            this.CurrentActivity = this.UserActivities.OrderBy(x => x.Index).First(x => x.ActivityState == TimerState.Pending);
            Debug.WriteLine("Action: " + CurrentActivity.Name);
            this.CurrentActivity.PropertyChanged += CurrentActivity_PropertyChanged;
            this.CurrentActivity.StatusChanged += CurrentActivity_StatusChanged;

            return CurrentActivity.StartTimer();
        }

        internal bool ResetTimer()
        {
            foreach (UserActivity activity in this.UserActivities.OrderBy(x => x.Index))
            {
                activity.ActivityState = TimerState.Pending;
                activity.RemainingDuration = activity.TotalDuration;
            }
            
            this.CurrentActivity = this.UserActivities.OrderBy(x => x.Index).First();
            return true;
        }

        public TimeSpan TotalTime { get; set; }

        private void CurrentActivity_StatusChanged(object sender, TimerStatusChangeEvent e)
        {
            if (e.Status == TimerState.Complete)
            {
                if (this.UserActivities.OrderBy(x => x.Index).Any(x => x.ActivityState == TimerState.Pending))
                {
                    StartTimer();
                }
                else
                {
                    this.TotalTime = DateTime.Now - this.startTime;
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
            StatusChanged?.Invoke(this, new TimerStatusChangeEvent(status));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
