using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class Rep : INotifyPropertyChanged
    {
        internal int Index { get; set; }
        internal TimerState ActivityState { get; set; }

        private DateTime startTime;
        private DateTime endTime;
        private TimeSpan totalTime;

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
            //this.CurrentActivity.PropertyChanged += CurrentActivity_PropertyChanged;
            this.CurrentActivity.StatusChanged += CurrentActivity_StatusChanged;

            return CurrentActivity.StartTimer();
        }

        internal bool ResetTimer()
        {
            foreach (UserActivity activity in this.UserActivities.OrderBy(x => x.Index))
            {
                activity.ActivityState = TimerState.Pending;
                //activity.RemainingDuration = activity.TotalDuration;
                activity.RemainingSeconds = (int)activity.TotalDuration.TotalSeconds;
            }
            
            this.CurrentActivity = this.UserActivities.OrderBy(x => x.Index).First();
            return true;
        }

        #region Times

        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }
            set
            {
                this.startTime = value;
                this.OnPropertyChanged("StartTime");
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }
            set
            {
                this.endTime = value;
                this.TotalTime = value - this.startTime;
                this.OnPropertyChanged("EndTime");
            }
        }

        public TimeSpan TotalTime
        {
            get
            {
                return this.totalTime;
            }
            set
            {
                this.totalTime = value;
                this.OnPropertyChanged("totalTime");
            }
        }

        #endregion

        public ContentView MyContent
        {
            get
            {
                ContentView cv = new ContentView();
                StackLayout stackLayout = new StackLayout();

                foreach (UserActivity ua in this.UserActivities)
                {

                    StackLayout slDuration = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    slDuration.Children.Add(new Label()
                    {
                        Text = ua.Name,
                        FontSize = 24,
                        HorizontalOptions = LayoutOptions.StartAndExpand
                    });
                    slDuration.Children.Add(new Label()
                    {
                        Text = String.Format("{0:mm\\:ss}", ua.TotalDuration),
                        FontSize = 24,
                        HorizontalOptions = LayoutOptions.End
                    });

                    StackLayout slStart = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    slStart.Children.Add(new Label()
                    {
                        Text = String.Format("{0:hh\\:mm\\:ss}", ua.StartTime),
                        HorizontalOptions = LayoutOptions.Start
                    });
                    slStart.Children.Add(new Label()
                    {
                        Text = String.Format("- {0:hh\\:mm\\:ss}", ua.EndTime),
                        HorizontalOptions = LayoutOptions.Start
                    });

                    stackLayout.Children.Add(slDuration);
                    stackLayout.Children.Add(slStart);
                }

                cv.Content = stackLayout;

                return cv;
            }
        }

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
                    this.ActivityState = TimerState.Complete;
                    this.EndTime = DateTime.Now;
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
