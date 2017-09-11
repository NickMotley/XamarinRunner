using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioManager;
using MotleyRunner;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class IntervalSettings : INotifyPropertyChanged
    {
        private Program program;
        private ButtonState buttonState;

        private bool canReset;

        public event PropertyChangedEventHandler PropertyChanged;
        internal event EventHandler<TimerStatusChangeEvent> StatusChanged;

        public IntervalSettings()
        {
            this.StartTimerCommand = new RelayCommand((s) => StartTimer(s), null);
            this.ResetTimerCommand = new RelayCommand((s) => ResetTimer(s), () => CanReset);

            this.buttonState = ButtonState.Start;

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

            if (!Application.Current.Properties.ContainsKey("ActivityTimes"))
            {
                Application.Current.Properties.Add("ActivityTimes", "12,8");
                Application.Current.SavePropertiesAsync();
            }

            this.Program.InitializeReps();
            this.Program.InitializeActivities();

        }

        public ButtonState ButtonState
        {
            get
            {
                return this.buttonState;
            }
            set
            {
                if ( this.buttonState != value)
                {
                    this.buttonState = value;
                    this.OnPropertyChanged("ButtonState");
                    this.OnPropertyChanged("ButtonCaption");
                    this.CanReset = (value != ButtonState.Pause);
                }
            }
        }

        public string ButtonCaption
        {
            get
            {
                return this.buttonState.ToString();
            }
        }

        private void Program_StatusChanged(object sender, TimerStatusChangeEvent e)
        {
            if (e.Status == TimerState.Complete)
            {
                this.buttonState = ButtonState.Start;
                this.OnPropertyChanged("ButtonCaption");

                //Set or Get the state of the Effect sounds.
                //Audio.Manager.EffectsOn = true;

                //Set the volume level of the Effects from 0 to 1.
                //Audio.Manager.EffectsVolume = 0.95F;

                PlaySounds();

                this.OnStatusChanged(TimerState.Complete);

            }
        }

        public bool CanReset
        {
            get
            {
                return this.canReset;
            }
            set
            {
                if ( this.canReset != value)
                {
                    this.canReset = value;
                    ResetTimerCommand.RaiseCanExecuteChanged();
                    this.OnPropertyChanged("CanReset");
                }
            }
        }

        async void PlaySounds()
        {
            //Play an effect sound. On Android the lenth is limeted to 5 seconds.
            await Audio.Manager.PlaySound("double-beep.mp3");
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

        private void ResetTimer(object obj)
        {
            if ((this.buttonState == ButtonState.Start) || (this.buttonState == ButtonState.Resume))
            {
                ResetTimes();
                this.ButtonState = ButtonState.Start;
            }
        }

        private void StartTimer(object obj)
        {
            switch (this.buttonState)
            {
                case ButtonState.Start:
                    this.ButtonState = ButtonState.Pause;
                    this.CreateProgram();
                    this.StartProgram();
                    break;
                case ButtonState.Pause:
                    this.ButtonState = ButtonState.Resume;
                    this.PauseProgram();
                    break;
                case ButtonState.Resume:
                    this.ButtonState = ButtonState.Pause;
                    ResumeProgram();
                    break;
            }
        }

        public RelayCommand StartTimerCommand { get; set; }
        public RelayCommand ResetTimerCommand { get; set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnStatusChanged(TimerState status)
        {
            StatusChanged?.Invoke(this, new TimerStatusChangeEvent(status));
        }

        private void ResetTimes()
        {
            this.Program.ResetTimer();
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
            this.Program.StartCountDown();
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

    public enum ButtonState
    {
        Start,
        Pause,
        Resume
    }
}
