using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioManager;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class IntervalSettings : INotifyPropertyChanged
    {
        private Program program;
        private buttonState buttonState;

        public event PropertyChangedEventHandler PropertyChanged;
        internal event EventHandler<TimerStatusChangeEvent> StatusChanged;

        public IntervalSettings()
        {
            this.StartTimerCommand = new Command(StartTimer);
            this.ResetTimerCommand = new Command(ResetTimer);

            this.buttonState = buttonState.Start;

            this.Program = new Program();
            this.Program.StatusChanged += Program_StatusChanged;

            if (!Application.Current.Properties.ContainsKey("RepCount"))
            {
                Application.Current.Properties.Add("RepCount", 1);
                Application.Current.SavePropertiesAsync();
            }

            if (!Application.Current.Properties.ContainsKey("ActivityCount"))
            {
                Application.Current.Properties.Add("ActivityCount", 1);
                Application.Current.SavePropertiesAsync();
            }

            this.Program.InitializeReps();
            this.Program.InitializeActivities();

        }

        private void Program_StatusChanged(object sender, TimerStatusChangeEvent e)
        {
            if (e.Status == TimerState.Complete)
            {
                this.buttonState = buttonState.Start;
                this.OnPropertyChanged("ButtonCaption");

                //Set or Get the state of the Effect sounds.
                //Audio.Manager.EffectsOn = true;

                //Set the volume level of the Effects from 0 to 1.
                //Audio.Manager.EffectsVolume = 0.95F;

                PlaySounds();

                this.OnStatusChanged(TimerState.Complete);

            }
        }

        async void PlaySounds()
        {
            //Play an effect sound. On Android the lenth is limeted to 5 seconds.
            await Audio.Manager.PlaySound("double-beep.mp3");
        }

        async void PlayHarley()
        {
            //Play an effect sound. On Android the lenth is limeted to 5 seconds.
            await Audio.Manager.PlaySound("harley-start.mp3");
        }

        public string CurrentTimer
        {
            get
            {
                return "This Time";
            }
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
            ResetTimes();
            this.buttonState = buttonState.Start;
            this.OnPropertyChanged("ButtonCaption");
        }

        private void StartTimer(object obj)
        {
            switch (this.buttonState)
            {
                case buttonState.Start:
                    PlayHarley();
                    this.CreateProgram();
                    this.StartProgram();
                    this.buttonState++;
                    break;
                case buttonState.Pause:
                    this.PauseProgram();
                    this.buttonState++;
                    break;
                case buttonState.Resume:
                    ResumeProgram();
                    this.buttonState = buttonState.Pause;
                    break;
            }
            this.OnPropertyChanged("ButtonCaption");
        }

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

        private void OnStatusChanged(TimerState status)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new TimerStatusChangeEvent(status));
            }
        }

        private void ResetTimes()
        {
            //using (Ringtone r = RingtoneManager.GetRingtone(ApplicationContext, RingtoneManager.GetDefaultUri(RingtoneType.Ringtone)))
            //{ r.Play(); }
        }

        private void CreateProgram()
        {
            foreach(Rep rep in this.Program.Reps.OrderBy(x => x.Index))
            {
                rep.Activities.Clear();
                foreach (Activity activity in this.Program.Activities.OrderBy(x => x.Index))
                {
                    rep.Activities.Add(activity.Clone());
                }
            }
        }

        private void StartProgram()
        {
            this.Program.StartTimer();
        }

        private void PauseProgram()
        {
            this.Program.PauseTimer();
        }

        private void ResumeProgram()
        {
            this.Program.ResumeTimer();
        }

    }

    enum buttonState
    {
        Start,
        Pause,
        Resume
    }
}
