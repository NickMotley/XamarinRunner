using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class Program : INotifyPropertyChanged
    {
        ObservableCollection<Rep> reps;
        ObservableCollection<Activity> activities;

        internal TimerState ActivityState { get; set; }

        Rep currentRep;

        public ICommand IncreaseRepsCommand { get; set; }
        public ICommand DecreaseRepsCommand { get; set; }

        public ICommand ActivitiesIncreaseCommand { get; set; }
        public ICommand ActivitiesDecreaseCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        internal event EventHandler<TimerStatusChangeEvent> StatusChanged;

        public Program()
        {
            this.IncreaseRepsCommand = new Command(IncreaseRepCount);
            this.DecreaseRepsCommand = new Command(DecreaseRepCount);

            this.ActivitiesIncreaseCommand = new Command(IncreaseActivities);
            this.ActivitiesDecreaseCommand = new Command(DecreaseActivities);
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
                this.OnStatusChanged(this.ActivityState);
            }
        }

        private void Activities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Activity item in e.NewItems)
                {
                    item.Index = e.NewStartingIndex;
                    item.ActivityState = TimerState.Pending;
                    item.TotalDuration = TimeSpan.FromSeconds(10);
                }
            }
        }

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

        public ObservableCollection<Activity> Activities
        {
            get
            {
                if (this.activities == null)
                {
                    this.activities = new ObservableCollection<Activity>();
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
            this.Reps.Add(new Rep());
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

        internal void InitializeActivities()
        {
            int? repCount = Application.Current.Properties["ActivityCount"] as int?;
            for (int i = 0; i < (int)repCount; i++)
            {
                this.IncreaseActivities();
            }
        }
        internal void IncreaseActivities()
        {
            this.Activities.Add(new Activity());
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

        private void CurrentRep_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
