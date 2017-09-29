using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using AudioManager;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class Program : INotifyPropertyChanged
    {
        ObservableCollection<Rep> reps;
        ObservableCollection<UserActivity> activities;

        private DateTime startTime;
        private DateTime endTime;
        private TimeSpan totalTime;
        private bool isDoingCountdown = true;

        private bool countDownIsVisible;

        internal TimerState ActivityState { get; set; }

        private int countDownRemaining;

        private int progressRatio;

        private Rep currentRep;

        public ICommand IncreaseRepsCommand { get; set; }
        public ICommand DecreaseRepsCommand { get; set; }

        public ICommand ActivitiesIncreaseCommand { get; set; }
        public ICommand ActivitiesDecreaseCommand { get; set; }
        
        public ICommand ProgressRatioIncreaseCommand { get; set; }
        public ICommand ProgressRatioDecreaseCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        internal event EventHandler<TimerStatusChangeEvent> StatusChanged;

        public Program()
        {
            this.IncreaseRepsCommand = new Command(IncreaseRepCount);
            this.DecreaseRepsCommand = new Command(DecreaseRepCount);

            this.ActivitiesIncreaseCommand = new Command(IncreaseActivities);
            this.ActivitiesDecreaseCommand = new Command(DecreaseActivities);

            this.ProgressRatioIncreaseCommand = new Command(IncreaseRatioDecrease);
            this.ProgressRatioDecreaseCommand = new Command(DecreaseRatioDecrease);
        }

        #region ProgressRatio

        private void DecreaseRatioDecrease(object obj)
        {
            this.ProgressRatio = this.ProgressRatio - 1;
        }

        private void IncreaseRatioDecrease(object obj)
        {
            this.ProgressRatio = this.ProgressRatio + 1;
        }

        public int ProgressRatio
        {
            get
            {
                return this.progressRatio;
            }

            set
            {
                this.progressRatio = value;
                this.OnPropertyChanged("ProgressRatio");
                Application.Current.Properties["ProgressRatio"] = value;
                Application.Current.SavePropertiesAsync();
            }
        }

        internal void InitializeProgressRatio()
        {
            this.ProgressRatio = (int)(Application.Current.Properties["ProgressRatio"] as int?);
        }

        #endregion

        #region Reps

        public ObservableCollection<Rep> Reps
        {
            get
            {
                if (this.reps == null)
                {
                    this.reps = new ObservableCollection<Rep>();
                    this.reps.CollectionChanged += Reps_CollectionChanged;
                }
                return this.reps;

            }
            set
            {
                this.reps = value;
                this.OnPropertyChanged("Reps");
            }
        }

        public Rep CurrentRep
        {
            get
            {
                return this.currentRep;
            }
            set
            {
                this.currentRep = value;
                this.OnPropertyChanged("CurrentRep");
            }
        }

        private void Reps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Rep item in e.NewItems)
                {
                    item.Index = e.NewStartingIndex;
                    item.StatusChanged += Rep_StatusChanged;
                }
            }
        }

        private void Rep_StatusChanged(object sender, TimerStatusChangeEvent e)
        {
            if (this.Reps.Any(x => x.ActivityState == TimerState.Pending))
            {
                this.StartTimer();
            }
            else
            {
                this.ActivityState = TimerState.Complete;
                this.EndTime = DateTime.Now;
                this.OnStatusChanged(this.ActivityState);
            }
        }

        internal void InitializeReps()
        {
            int? repCount = Application.Current.Properties["RepCount"] as int?;
            for (int i = 0; i < (int)repCount; i++)
            {
                this.IncreaseRepCount();
            }
        }

        internal void IncreaseRepCount()
        {
            Rep rep = new Rep();
            this.Reps.Add(rep);
            this.UpdateRepCount();
        }

        private void DecreaseRepCount(object obj)
        {
            this.Reps.RemoveAt(this.Reps.Count - 1);
            this.UpdateRepCount();
        }

        private void UpdateRepCount()
        {
            this.OnPropertyChanged("Reps");
            Application.Current.Properties["RepCount"] = this.Reps.Count;
            Application.Current.SavePropertiesAsync();
        }

        private void CurrentRep_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        #endregion

        #region Activities

        public ObservableCollection<UserActivity> Activities
        {
            get
            {
                if (this.activities == null)
                {
                    this.activities = new ObservableCollection<UserActivity>();
                    this.activities.CollectionChanged += Activities_CollectionChanged;
                }
                return this.activities;
            }
            set
            {
                this.activities = value;
                this.OnPropertyChanged("Activities");
            }
        }

        internal void InitializeActivities()
        {
            int? repCount = Application.Current.Properties["ActivityCount"] as int?;
            string actTimes = Application.Current.Properties["ActivityTimes"] as string;

            for (int i = 0; i < (int)repCount; i++)
            {
                this.IncreaseActivities();
                this.Activities[i].TotalDuration = TimeSpan.FromSeconds(int.Parse(actTimes.Split(',')[i]));
            }

            this.OnPropertyChanged("Activity1Duration");
            this.OnPropertyChanged("Activity2Duration");
        }

        internal void IncreaseActivities()
        {
            UserActivity activity = new UserActivity();
            activity.PropertyChanged += Activity_PropertyChanged;
            this.Activities.Add(activity);
            UpdateActivitiesCount();
        }

        private void DecreaseActivities(object obj)
        {
            this.Activities.RemoveAt(this.Activities.Count - 1);
            UpdateActivitiesCount();
        }

        private void UpdateActivitiesCount()
        {
            this.OnPropertyChanged("Activities");
            Application.Current.Properties["ActivityCount"] = this.Activities.Count;
            Application.Current.SavePropertiesAsync();
        }

        private void Activities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (UserActivity item in e.NewItems)
                {
                    item.Index = e.NewStartingIndex;
                    item.ActivityState = TimerState.Pending;
                    item.TotalDuration = TimeSpan.FromSeconds(10);
                }
            }
        }

        private void Activity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TotalDuration")
            {
                string times = string.Empty;
                foreach (UserActivity activity in this.Activities.OrderBy(x => x.Index))
                {
                    times += activity.TotalDuration.TotalSeconds.ToString() + ",";
                }
                times = times.Substring(0, times.Length - 1);
                Application.Current.Properties["ActivityTimes"] = times;
                Application.Current.SavePropertiesAsync();
            }
        }

        public TimeSpan Activity1Duration
        {
            get
            {
                TimeSpan ts;
                foreach (Rep rep in this.Reps)
                {
                    ts = ts.Add(rep.UserActivities[0].TotalDuration);
                }
                return ts;
            }
        }

        public TimeSpan Activity2Duration
        {
            get
            {
                TimeSpan ts;
                foreach (Rep rep in this.Reps)
                {
                    ts = ts.Add(rep.UserActivities[1].TotalDuration);
                }
                return ts;
            }
        }

        #endregion

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

        public int CountDownRemaining
        {
            get
            {
                return this.countDownRemaining;
            }
            set
            {
                if (this.countDownRemaining != value)
                {
                    this.countDownRemaining = value;
                    this.OnPropertyChanged("CountDownRemaining");
                }
            }
        }

        public bool CountDownIsVisible
        {
            get
            {
                return this.countDownIsVisible;
            }
            set
            {
                this.countDownIsVisible = value;
                this.OnPropertyChanged("CountDownIsVisible");
            }
        }

        internal void StartCountDown()
        {
            this.CountDownRemaining = 5;
            this.CountDownIsVisible = true;
            this.isDoingCountdown = true;
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 1000), TimerElapsed);
        }

        private bool TimerElapsed()
        {
            //return true to keep timer reccuring
            //return false to stop timer

            Device.BeginInvokeOnMainThread(() =>
            {
                //put here your code which updates the view
                if (isDoingCountdown)
                {
                    if (this.CountDownRemaining > 1)
                    {
                        this.CountDownRemaining = this.CountDownRemaining - 1;
                    }
                    else
                    {
                        this.CountDownIsVisible = false;
                        this.isDoingCountdown = false;
                    }
                }
                else
                {

                }
            });

            PlayCountDownSound();

            if (this.CountDownRemaining > 1)
            {
                return true;
            }
            else
            {
                this.CountDownIsVisible = false;
                this.startTime = DateTime.Now;
                StartTimer();
                return false;
            }
        }

        async void PlayCountDownSound()
        {
            if (this.CountDownRemaining > 1)
            {
                //Play an effect sound. On Android the lenth is limeted to 5 seconds.
                await Audio.Manager.PlaySound("single-beep.mp3");
            }
            else
            {
                //Play an effect sound. On Android the lenth is limeted to 5 seconds.
                await Audio.Manager.PlaySound("double-beep.mp3");
            }

        }
        
        private bool ProgramTimerElapsed()
        {

            return true;
        }

        internal bool StartTimer()
        {
            this.CurrentRep = this.Reps.OrderBy(x => x.Index).First(x => x.ActivityState == TimerState.Pending);
            this.CurrentRep.PropertyChanged += CurrentRep_PropertyChanged;

            return this.CurrentRep.StartTimer();
        }

        internal bool PauseTimer()
        {
            return this.CurrentRep.PauseTimer();
        }

        internal bool ResumeTimer()
        {
            return this.CurrentRep.ResumeTimer();
        }

        internal bool ResetTimer()
        {
            foreach (Rep rep in this.Reps.OrderBy(x => x.Index))
            {
                rep.ResetTimer();
            }
            this.CurrentRep = this.Reps.OrderBy(x => x.Index).First();
            return true;
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