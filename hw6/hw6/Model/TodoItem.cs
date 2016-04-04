using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace hw6 {
    public class TodoItem : INotifyPropertyChanged {
        private Int64 id;
        public Int64 Id {
            get {
                return id;
            }
        }
        public ImageSource image;
        public ImageSource Image {
            get {
                return image;
            }
            set {
                if (value != image) {
                    image = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string name;
        public string Name {
            get {
                return name;
            }
            set {
                if (value != name) {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string detail;
        public string Detail {
            get {
                return detail;
            }
            set {
                if (value != detail) {
                    detail = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DateTimeOffset dueDate;
        public DateTimeOffset DueDate {
            get {
                return dueDate;
            }
            set {
                if (value != dueDate) {
                    dueDate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? finished;
        public bool? Finished {
            get {
                return finished;
            }
            set {
                if (value != finished) {
                    finished = value;
                    TodoItemCollection.Collection.Update(this);
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TodoItem() {
            this.id = 0;
            this.Name = "";
            this.Detail = "";
            this.DueDate = new DateTimeOffset(DateTime.Now);
            this.finished = false;
            this.image = new BitmapImage(new Uri("ms-appx://hw2/Assets/background.jpg"));
        }

        public TodoItem(Int64 id, string name, string detail, DateTimeOffset dueDate, bool finished, Uri image) {
            this.id = id;
            this.Name = name;
            this.Detail = detail;
            this.DueDate = dueDate;
            this.finished = finished;
            this.image = new BitmapImage(image);
        }

        public TodoItem(Int64 id, string name, string detail, DateTimeOffset dueDate, bool finished) {
            this.id = id;
            this.Name = name;
            this.Detail = detail;
            this.DueDate = dueDate;
            this.finished = finished;
            this.image = new BitmapImage(new Uri("ms-appx://hw2/Assets/background.jpg"));
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public TodoItem copy(TodoItem other) {
            this.id = other.id;
            this.Name = other.name;
            this.Detail = other.detail;
            this.DueDate = other.dueDate;
            this.Finished = other.finished;
            this.Image = other.Image;
            return this;
        }
    }
}
