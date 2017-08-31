using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class IntervalSettings : INotifyPropertyChanged
    {
        private Program program;

        private bool runUpdate;
        //private int interval, intervalFull;
        //private TimeSpan seg1, seg2;
        //private TimeSpan seg1Full, seg2Full;
        private buttonState buttonState;

        public event PropertyChangedEventHandler PropertyChanged;

        public IntervalSettings()
        {
            this.StartTimerCommand = new Command(StartTimer);
            this.ResetTimerCommand = new Command(ResetTimer);

            this.buttonState = buttonState.Start;

            this.Program = new Program()
            {
                Reps = new System.Collections.ObjectModel.ObservableCollection<Rep>()
                {
                    new Rep(), new Rep()
                },
                Activities = new System.Collections.ObjectModel.ObservableCollection<Activity>()
                { new Activity()
                    { Index = 1, ActivityState = ActivityState.Pending, TotalDuration = TimeSpan.FromSeconds(10)
                    },
                    new Activity()
                    { Index = 2, ActivityState = ActivityState.Pending, TotalDuration = TimeSpan.FromSeconds(5)
                    }
                }
            };

        }

        public Program Program
        {
            get
            {
                return this.program;
            }
            set
            {
                this.program = value;
                this.OnPropertyChanged("Program");
            }
        }

        public string ButtonCaption
        {
            get
            {
                return this.buttonState.ToString();
            }
        }

        private void ResetTimer(object obj)
        {
            runUpdate = false;
            ResetTimes();
            //interval = intervalFull;
            this.buttonState = buttonState.Start;
            this.OnPropertyChanged("ButtonCaption");
        }

        private void StartTimer(object obj)
        {
            switch (this.buttonState)
            {
                case buttonState.Start:
            //        seg1Full = seg1;
            //        seg2Full = seg2;
            //        intervalFull = interval;
                    this.buttonState++;
            //        runUpdate = true;
            //        RunUpdateLoop();
                    break;
                case buttonState.Pause:
            //        runUpdate = false;
                    this.buttonState++;
                    break;
                case buttonState.Resume:
            //        runUpdate = true;
            //        RunUpdateLoop();
                    this.buttonState = buttonState.Pause;
                    break;
            }
            this.OnPropertyChanged("ButtonCaption");
        }

        private async void RunUpdateLoop()
        {
            while (runUpdate)
            {
                await Task.Delay(1000);
                //if (runUpdate)
                //{
                //    if (this.seg1 > TimeSpan.FromSeconds(0))
                //        this.DecreaseSeg1Count(null);
                //    else if (this.seg2 > TimeSpan.FromSeconds(0))
                //        this.DecreaseSeg2Count(null);
                //    else if (this.Interval > 0)
                //    {
                //        this.Interval--;
                //        ResetTimes();
                //        this.DecreaseSeg1Count(null);
                //    }
                //}
            }
        }

        //public int Interval
        //{
        //    get {
        //        return this.interval;
        //    }
        //    set {
        //        this.interval = value;
        //        this.OnPropertyChanged("Interval");
        //    }
        //}

        public ICommand StartTimerCommand { get; set; }
        public ICommand PauseTimerCommand { get; set; }
        public ICommand ResetTimerCommand { get; set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ResetTimes()
        {
            //Seg1 = seg1Full;
            //Seg2 = seg2Full;
        }
    }

    enum buttonState
    {
        Start,
        Pause,
        Resume
    }
}
