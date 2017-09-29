using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioManager;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class UserActivity : INotifyPropertyChanged
    {
        private TimeSpan totalDuration;
        private bool runUpdate;
        private const int timerResolution = 50;
        private int remainingSeconds;

        private DateTime startTime;
        private DateTime endTime;
        private TimeSpan totalTime;

        public int Index { get; set; }
        internal TimerState ActivityState { get; set; }
        public TimeSpan PausedDuration { get; set; }

        public ICommand IncreaseDurationCommand { get; set; }
        public ICommand DecreaseDurationCommand { get; set; }

        public UserActivity()
        {
            this.DecreaseDurationCommand = new Command(DecreaseDuration);
            this.IncreaseDurationCommand = new Command(IncreaseDuration);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal event EventHandler<TimerStatusChangeEvent> StatusChanged;

        public string Name
        {
            get
            {
                return "Interval " + (this.Index + 1).ToString();   // + " Timer";
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

        public TimeSpan RemainingDuration
        {
            get
            {
                return TimeSpan.FromSeconds(this.remainingSeconds);
            }
        }

        public int RemainingSeconds
        {
            get
            {
                return this.remainingSeconds;
            }

            set
            {
                if (this.remainingSeconds != value)
                {
                    this.remainingSeconds = value;

                    if (value < 4)
                    {
                        this.PlaySounds();
                    }
                    if (value == 0)
                    {
                        runUpdate = false;
                        this.EndTime = DateTime.Now;
                        this.ActivityState = TimerState.Complete;
                        this.OnStatusChanged(this.ActivityState);
                    }

                    this.OnPropertyChanged("RemainingDuration");
                }
            }
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

        //private bool TimerElapsed()
        //{
        //    //return true to keep timer reccuring
        //    //return false to stop timer

        //    if (runUpdate)
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            //put here your code which updates the view
        //            this.RemainingDuration = this.RemainingDuration.Add(TimeSpan.FromSeconds(-1));

        //            if (this.RemainingDuration.Minutes == 0 && this.RemainingDuration.Seconds == 0)
        //            {
        //                PlaySounds();
        //                this.ActivityState = TimerState.Complete;
        //                //runUpdate = false;
        //                this.OnStatusChanged(this.ActivityState);
        //            }
        //            else
        //            {
        //                if (this.RemainingDuration.Minutes == 0)
        //                {
        //                    if (this.RemainingDuration.Seconds < 4)
        //                    {
        //                        PlaySounds();
        //                    }
        //                }
        //            }

        //        });

        //        if (this.RemainingDuration.Minutes == 0 && this.RemainingDuration.Seconds == 0)
        //        {
        //            //PlaySounds();
        //            //this.ActivityState = TimerState.Complete;
        //            ////runUpdate = false;
        //            //this.OnStatusChanged(this.ActivityState);
        //            return false;
        //        }
        //        else
        //        {
        //            //if (this.RemainingDuration.Minutes == 0)
        //            //{
        //            //    if (this.RemainingDuration.Seconds < 4)
        //            //    {
        //            //        PlaySounds();
        //            //    }
        //            //}
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        private async void RunUpdateLoop()
        {
            while (runUpdate)
            {
                await Task.Delay(timerResolution);
                if (runUpdate)
                {
                    TimeSpan timeSpanSinceStart = DateTime.Now - this.StartTime;

                    this.RemainingSeconds = timeSpanSinceStart <= totalDuration ? (int)(totalDuration - timeSpanSinceStart).TotalSeconds + 1 : 0;

                    //if (timeSpanSinceStart < TotalDuration)
                    //{
                    //this.RemainingDuration = timeSpanSinceStart < TotalDuration ? TotalDuration - timeSpanSinceStart: TimeSpan.FromSeconds(0); 
                    //}
                    //else
                    //{
                    //    runUpdate = false;
                    //    this.EndTime = DateTime.Now;
                    //    this.ActivityState = TimerState.Complete;
                    //    this.OnStatusChanged(this.ActivityState);
                    //}
                }
            }
        }

        async void PlaySounds()
        {
            if (this.RemainingDuration.Seconds > 0)
            {
                //Play an effect sound. On Android the lenth is limeted to 5 seconds.
                await Audio.Manager.PlaySound("single-beep.mp3");
            }
            else
            {
                await Audio.Manager.PlaySound("double-beep.mp3");
            }
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnStatusChanged(TimerState status)
        {
            StatusChanged?.Invoke(this, new TimerStatusChangeEvent(status));
        }

        internal UserActivity Clone()
        {
            UserActivity clone = new UserActivity()
            {
                ActivityState = TimerState.Pending,
                TotalDuration = this.TotalDuration,
                Index = this.Index
            };
            
            return clone;
        }

        internal bool StartTimer()
        {
            this.startTime = DateTime.Now;
            //this.RemainingSeconds = (int)this.TotalDuration.TotalSeconds;
            //this.RemainingDuration = this.TotalDuration;
            this.runUpdate = true;
            this.RunUpdateLoop();
            return true;
        }

        internal bool PauseTimer()
        {
            this.runUpdate = false;
            return true;
        }

        internal bool ResumeTimer()
        {
            this.runUpdate = true;
            this.RunUpdateLoop();
            //Device.StartTimer(new TimeSpan(0, 0, 0, 0, 1000), TimerElapsed);
            return true;
        }
    }
}
