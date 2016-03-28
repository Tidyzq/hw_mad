using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
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

namespace hw5 {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailView : Page {

        public DetailView() {
            this.InitializeComponent();
        }

        private TodoItem item = null;
        public TodoItem NewItem = new TodoItem();
        public TodoItem Item {
            get {
                return item;
            }
            set {
                item = value;
                if (item != null) {
                    NewItem.copy(item);
                } else {
                    NewItem.copy(new TodoItem());
                }
            }
        }

        private string check() {
            if (NewItem.DueDate <= DateTimeOffset.Now) {
                return "Invalid Due Date!";
            } else if (NewItem.Name == "") {
                return "Title Shouldn't Be Empty!";
            } else if (NewItem.Detail == "") {
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
                    item.copy(NewItem);
                } else {
                    new TodoItemCollection().Add(NewItem);
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
                NewItem.copy(item);
            } else {
                NewItem.copy(new TodoItem());
            }
            callBack();
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
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                    WriteableBitmap bitmapImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    //bitmapImage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    Debug.WriteLine(bitmapImage);
                    NewItem.Image = bitmapImage;
                }
            }
        }
    }
}
