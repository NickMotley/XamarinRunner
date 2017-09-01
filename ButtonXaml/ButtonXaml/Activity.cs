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
    public class Activity : INotifyPropertyChanged
    {
        private TimeSpan totalDuration;
        private TimeSpan remainingDuration;

        private bool runUpdate;

        public int Index { get; set; }
        internal TimerState ActivityState { get; set; }
        public TimeSpan PausedDuration { get; set; }

        public ICommand IncreaseDurationCommand { get; set; }
        public ICommand DecreaseDurationCommand { get; set; }

        public Activity()
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
                return "Activity " + (this.Index + 1).ToString() + " Timer";
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
                    if (this.RemainingDuration > TimeSpan.FromSeconds(0))
                    {
                        if (this.RemainingDuration <= TimeSpan.FromSeconds(3))
                        {
                            PlaySounds();
                        }
                        this.RemainingDuration = this.RemainingDuration.Add(TimeSpan.FromSeconds(-1));
                    }
                    else
                    {
                        this.ActivityState = TimerState.Complete;
                        runUpdate = false;
                        this.OnStatusChanged(this.ActivityState);
                    }
                }
            }
        }

        async void PlaySounds()
        {
            //Play an effect sound. On Android the lenth is limeted to 5 seconds.
            await Audio.Manager.PlaySound("single-beep.mp3");
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnStatusChanged(TimerState status)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new TimerStatusChangeEvent(status));
            }
        }

        internal Activity Clone()
        {
            Activity clone = new Activity()
            {
                ActivityState = this.ActivityState,
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
