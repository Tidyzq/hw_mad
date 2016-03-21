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

namespace hw2 {
    public class TodoItem : INotifyPropertyChanged {
        public ImageSource Image;
        public string Name;
        public string Detail;
        public DateTimeOffset DueDate;
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

        public TodoItem(string name, string detail, DateTimeOffset dueDate, bool finished, Uri image) {
            this.Name = name;
            this.Detail = detail;
            this.DueDate = dueDate;
            this.finished = finished;
            this.Image = new BitmapImage(image);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
