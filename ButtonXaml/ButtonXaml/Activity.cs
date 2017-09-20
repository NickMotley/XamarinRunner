using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioManager;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class UserActivity : INotifyPropertyChanged
    {
        private TimeSpan totalDuration;
        private TimeSpan remainingDuration;

        private bool runUpdate;

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
                return this.remainingDuration;
            }
            set
            {
                this.remainingDuration = value;
                this.OnPropertyChanged("RemainingDuration");
            }
        }

        private async void RunUpdateLoop()
        {
            while (runUpdate)
            {
                await Task.Delay(1000);
                if (runUpdate)
                {
                    this.RemainingDuration = this.RemainingDuration.Add(TimeSpan.FromSeconds(-1));

                    if (this.RemainingDuration.Minutes == 0 && this.RemainingDuration.Seconds == 0)
                    {
                        PlaySounds();
                        this.ActivityState = TimerState.Complete;
                        runUpdate = false;
                        this.OnStatusChanged(this.ActivityState);
                    }
                    else
                    {
                        if (this.RemainingDuration.Minutes == 0)
                        {
                            if (this.RemainingDuration.Seconds < 4)
                            {
                                PlaySounds();
                            }
                        }
                    }

                    //if (this.RemainingDuration.Minutes > 0 || this.RemainingDuration.Seconds > 0)
                    //{
                    //    if (this.RemainingDuration.Minutes < 1 && this.RemainingDuration.Seconds <= 3)
                    //    {
                    //        PlaySounds();
                    //    }
                    //    this.RemainingDuration = this.RemainingDuration.Add(TimeSpan.FromSeconds(-1));
                    //}
                    //else
                    //{
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
            this.RemainingDuration = this.TotalDuration;
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
            return true;
        }
    }
}
