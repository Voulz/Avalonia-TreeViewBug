using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;


namespace AvaloniaTreeViewBug
{
    public class ParentObj : INotifyPropertyChanged
    {
        public string Name { get; }
        public ObservableCollection<ChildObj> Children { get; } = new ObservableCollection<ChildObj>();
        public string Description => $"Parent: `{Name}` with `{Children.Count}` children";

        public void AddChild(ChildObj child)
        {
            Children.Add(child);
            OnPropertyChanged(nameof(Description));
        }
        public void RemoveChild(ChildObj child)
        {
            Children.Remove(child);
            OnPropertyChanged(nameof(Description));
        }
        public ParentObj(string name)
        {
            Name = name;
        }

        public override string ToString() => Description;
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class ChildObj
    {
        public ParentObj ParentObj { get; }
        public string Name { get; }
        public string Description => $"-- Child: `{Name}` of parent `{ParentObj?.Name}`";

        public ChildObj(ParentObj parentObj, string name)
        {
            ParentObj = parentObj;
            Name = name;
        }
        public override string ToString() => Description;

    }

    public class MainWindow : Window, INotifyPropertyChanged
    {
        #region Default

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public TreeView TV { get; set; }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
            InitializeData();
        }

        #endregion

        /// <summary>Initializes the data to work on.</summary>
        public void InitializeData()
        {
            const string parent1 = "Parent 1";
            const string parent2 = "Parent 2";
            const string parent3 = "Parent 3";

            //set up the data
            _data = new List<(string parentName, string name)>
            {
                //(parent1, null),
                //(parent1, "Bob"),
                //(parent2, null),
                //(parent2, "Alice"),
                //(parent1, "Bernard"),
                //(parent2, "Anika"),
                //(parent2, "Anna"),
                (parent3, null),
                (parent3, "Leon"),
                (parent3, "Liam"),
            };


            //create the objects
            var orderedData = new List<object>();
            foreach (var (parentName, childName) in _data)
            {
                if (childName == null)
                {
                    var parent = new ParentObj(parentName);
                    orderedData.Add(parent);
                }
                else
                {
                    var parent = orderedData.OfType<ParentObj>().FirstOrDefault(p => p.Name == parentName);
                    if (parent == null) throw new Exception("Error in Data");

                    var child = new ChildObj(parent, childName);
                    orderedData.Add(child);
                }
            }
            foreach (var item in orderedData.ToArray().Reverse())
                _next.Push(item);



            OnPropertyChanged(nameof(NextItems));
            OnPropertyChanged(nameof(PreviousItems));
            OnPropertyChanged(nameof(CanMoveNext));
            OnPropertyChanged(nameof(CanMovePrevious));
        }

        private List<(string parentName, string childName)> _data;
        private readonly Stack<object> _next = new Stack<object>();
        private readonly Stack<object> _previous = new Stack<object>();


        public ObservableCollection<ParentObj> Items { get; } = new ObservableCollection<ParentObj>();

        public List<object> NextItems => _next.ToList();
        public List<object> PreviousItems => _previous.ToList();
        public bool CanMoveNext => _next.Any();
        public bool CanMovePrevious => _previous.Any();


        public void MoveNext()
        {
            var item = _next.Pop();
            _previous.Push(item);

            switch (item)
            {
                case ParentObj parent:
                    Items.Add(parent);
                    break;
                case ChildObj child:
                    child.ParentObj.AddChild(child);
                    break;
            }

            OnPropertyChanged(nameof(NextItems));
            OnPropertyChanged(nameof(PreviousItems));
            OnPropertyChanged(nameof(CanMoveNext));
            OnPropertyChanged(nameof(CanMovePrevious));
        }

        public void MovePrevious()
        {
            var item = _previous.Pop();
            _next.Push(item);

            switch (item)
            {
                case ParentObj parent:
                    Items.Remove(parent);
                    break;
                case ChildObj child:
                    child.ParentObj.RemoveChild(child);
                    break;
            }

            OnPropertyChanged(nameof(NextItems));
            OnPropertyChanged(nameof(PreviousItems));
            OnPropertyChanged(nameof(CanMoveNext));
            OnPropertyChanged(nameof(CanMovePrevious));
        }

        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
