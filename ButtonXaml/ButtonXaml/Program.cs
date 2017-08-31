using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace ButtonXaml
{
    public class Program : INotifyPropertyChanged
    {
        ObservableCollection<Rep> reps;
        ObservableCollection<Activity> activities;

        public ICommand IncreaseRepsCommand { get; set; }
        public ICommand DecreaseRepsCommand { get; set; }

        public ICommand ActivitiesIncreaseCommand { get; set; }
        public ICommand ActivitiesDecreaseCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Program()
        {
            this.IncreaseRepsCommand = new Command(IncreaseIntervalCount);
            this.DecreaseRepsCommand = new Command(DecreaseIntervalCount);

            this.ActivitiesIncreaseCommand = new Command(IncreaseActivities);
            this.ActivitiesDecreaseCommand = new Command(DecreaseActivities);

            this.Reps = new ObservableCollection<Rep>();
            //this.Reps.CollectionChanged += Reps_CollectionChanged;

            this.Activities = new ObservableCollection<Activity>();
            //this.Activities.CollectionChanged += Activities_CollectionChanged;
        }

        //private void Activities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null)
        //    {
        //        foreach (Activity newItem in e.NewItems)
        //        {
        //            ModifiedItems.Add(newItem);

        //            //Add listener for each item on PropertyChanged event
        //            newItem.PropertyChanged += this.OnItemPropertyChanged;
        //        }
        //    }

        //    if (e.OldItems != null)
        //    {
        //        foreach (Item oldItem in e.OldItems)
        //        {
        //            ModifiedItems.Add(oldItem);

        //            oldItem.PropertyChanged -= this.OnItemPropertyChanged;
        //        }
        //    }
        //}

        //private void Reps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        public ObservableCollection<Rep> Reps
        {
            get
            {
                return this.reps;
            }
            set
            {
                this.reps = value;
                this.OnPropertyChanged("Reps");
            }
        }

        public ObservableCollection<Activity> Activities
        {
            get
            {
                return this.activities;
            }
            set
            {
                this.activities = value;
                this.OnPropertyChanged("Activities");
            }
        }

        private void IncreaseActivities(object obj)
        {
            this.Activities.Add(new Activity() { Index = this.Activities.Count+1, ActivityState = ActivityState.Pending, TotalDuration = TimeSpan.FromSeconds(10) });
            this.OnPropertyChanged("Activities");
        }

        private void DecreaseActivities(object obj)
        {
            this.Activities.RemoveAt(this.Activities.Count - 1);
            this.OnPropertyChanged("Activities");
        }

        private void IncreaseIntervalCount()
        {
            this.Reps.Add(new Rep());
            this.OnPropertyChanged("Reps");
        }

        private void DecreaseIntervalCount(object obj)
        {
            this.Reps.RemoveAt(this.Reps.Count - 1);
            this.OnPropertyChanged("Reps");
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
