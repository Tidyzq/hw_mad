using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace hw3 {
    public class TodoItem : INotifyPropertyChanged {
        private BitmapImage image;
        public BitmapImage Image {
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
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TodoItem() {
            this.Name = "";
            this.Detail = "";
            this.DueDate = new DateTimeOffset(DateTime.Now);
            this.finished = false;
            this.Image = new BitmapImage(new Uri("ms-appx://hw2/Assets/background.jpg"));
        }

        public TodoItem(string name, string detail, DateTimeOffset dueDate, bool finished, Uri image) {
            this.Name = name;
            this.Detail = detail;
            this.DueDate = dueDate;
            this.finished = finished;
            this.Image = new BitmapImage(image);
        }

        public TodoItem(string name, string detail, DateTimeOffset dueDate, bool finished, BitmapImage image) {
            this.Name = name;
            this.Detail = detail;
            this.DueDate = dueDate;
            this.finished = finished;
            this.Image = image;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
