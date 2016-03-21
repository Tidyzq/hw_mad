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

namespace hw2 {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page {

        public TodoItem Item;
        public event CreateTodoItem Create;

        public NewPage() {
            this.InitializeComponent();
            this.Item = new TodoItem("", "", DateTimeOffset.Now, false, new Uri("ms-appx://hw2/Assets/background.jpg"));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Frame root = Window.Current.Content as Frame;

            if (root.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private string check() {
            if (Item.DueDate <= DateTimeOffset.Now) {
                return "Invalid Due Date!";
            } else if (Item.Name == "") {
                return "Title Shouldn't Be Empty!";
            } else if (Item.Detail == "") {
                return "Title Shouldn't Be Empty!";
            }
            return null;
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e) {
            var err = check();
            if (err != null) {
                var i = new MessageDialog(err).ShowAsync();
            } else {
                if (Create != null) {
                    Create(this.Item);
                }
                if (this.Frame.CanGoBack) {
                    this.Frame.GoBack();
                }
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) {
            if (this.Frame.CanGoBack) {
                this.Frame.GoBack();
            }
        }

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
                    this.Item.Image = bitmapImage;
                    this.Image.Source = bitmapImage;
                }
            }
        }
    }

}
