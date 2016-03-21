using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace hw3 {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailView : Page {

        public DetailView() {
            this.InitializeComponent();
        }

        private TodoItem item = null;
        public TodoItem Item {
            get {
                return item;
            }
            set {
                item = value;
                if (item != null) {
                    this.TitleTextBox.Text = item.Name;
                    this.DetailTextBox.Text = item.Detail;
                    this.DatePicker.Date = item.DueDate;
                    this.Image.Source = item.Image;
                    newItemImage = item.Image;
                } else {
                    this.TitleTextBox.Text = "";
                    this.DetailTextBox.Text = "";
                    this.DatePicker.Date = DateTimeOffset.Now;
                    this.Image.Source = new BitmapImage(new Uri("ms-appx://hw2/Assets/background.jpg"));
                    newItemImage = new BitmapImage(new Uri("ms-appx://hw2/Assets/background.jpg"));
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Frame root = Window.Current.Content as Frame;
            if (root.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            if (e.Parameter != null) {
                Item = (TodoItem)e.Parameter;
            }
        }

        private string check() {
            if (this.DatePicker.Date <= DateTimeOffset.Now) {
                return "Invalid Due Date!";
            } else if (this.TitleTextBox.Text == "") {
                return "Title Shouldn't Be Empty!";
            } else if (this.DetailTextBox.Text == "") {
                return "Title Shouldn't Be Empty!";
            }
            return null;
        }

        public void Confirm(Action callBack) {
            var err = check();
            if (err != null) {
                var i = new MessageDialog(err).ShowAsync();
            } else {
                if (item != null) {
                    item.Name = this.TitleTextBox.Text;
                    item.Detail = this.DetailTextBox.Text;
                    item.DueDate = this.DatePicker.Date;
                    item.Image = newItemImage;
                } else {
                    new TodoItemCollection().Add(new TodoItem(this.TitleTextBox.Text, this.DetailTextBox.Text, this.DatePicker.Date, false, newItemImage));
                }
                callBack();
            }
        }

        public void Delete(Action callBack) {
            new TodoItemCollection().Remove(item);
            callBack();
        }

        public void Cancel(Action callBack) {
            if (item != null) {
                this.TitleTextBox.Text = item.Name;
                this.DetailTextBox.Text = item.Detail;
                this.DatePicker.Date = item.DueDate;
                this.Image.Source = item.Image;
                newItemImage = item.Image;
            } else {
                this.TitleTextBox.Text = "";
                this.DetailTextBox.Text = "";
                this.DatePicker.Date = DateTimeOffset.Now;
                this.Image.Source = new BitmapImage(new Uri("ms-appx://hw2/Assets/background.jpg"));
                newItemImage = new BitmapImage(new Uri("ms-appx://hw2/Assets/background.jpg"));
            }
            callBack();
        }

        private BitmapImage newItemImage = new BitmapImage(new Uri("ms-appx://hw2/Assets/background.jpg"));

        private async void SelectBtn_Click(object sender, RoutedEventArgs e) {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null) {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read)) {
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    //bitmapImage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    this.newItemImage = bitmapImage;
                    this.Image.Source = bitmapImage;
                }
            }
        }
    }
}
