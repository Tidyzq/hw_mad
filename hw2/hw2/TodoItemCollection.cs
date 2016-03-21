using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace hw2 {
    public class TodoItemCollection : INotifyPropertyChanged {
        static private ObservableCollection<TodoItem> collection = new ObservableCollection<TodoItem>() {
            new TodoItem("test1", "", new DateTimeOffset(new DateTime(2016, 5, 3)), true, new Uri("ms-appx://hw2/Assets/background.jpg")),
            new TodoItem("test2", "", new DateTimeOffset(new DateTime(2016, 8, 1)), false, new Uri("ms-appx://hw2/Assets/background.jpg"))
        };
        public ObservableCollection<TodoItem> Collection {
            get { return collection; }
            set {
                if (collection != value) {
                    collection = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public TodoItemCollection() {
            collection.CollectionChanged += Collection_CollectionChanged;
        }

        ~TodoItemCollection() {
            collection.CollectionChanged -= Collection_CollectionChanged;
        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            NotifyPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
