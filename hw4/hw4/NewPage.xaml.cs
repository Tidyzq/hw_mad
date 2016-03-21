using Newtonsoft.Json;
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
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace hw4 {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page {

        public NewPage() {
            this.InitializeComponent();
        }

        private DetailView DetailView = new DetailView();

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (Frame.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            if (e.Parameter != null) {
                DetailView.Item = new TodoItemCollection().Collection.ElementAt((int)e.Parameter);
                this.DeleteBtn.IsEnabled = true;
            }
            if (e.NavigationMode == NavigationMode.New) {
                // If this is a new navigation, this is a fresh launch so we can
                // discard any saved state
                ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
            } else {
                // Try to restore state if any, in case we were terminated
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TheWorkInProgress")) {
                    var composite = ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] as ApplicationDataCompositeValue;
                    DetailView.NewItem.copy(JsonConvert.DeserializeObject<TodoItem>((string)composite["Item"]));

                    // We're done with it, so remove it
                    ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            if (((App)App.Current).IsSuspending) {
                Debug.WriteLine("suspending");
                // Save volatile state in case we get terminated later on, then
                // we can restore as if we'd never been gone :) 
                var composite = new ApplicationDataCompositeValue();
                composite["Item"] = JsonConvert.SerializeObject(DetailView.NewItem);
                //var stream = (DetailView.NewItem.Image as WriteableBitmap).PixelBuffer.AsStream().AsOutputStream();
                //forma
                Debug.WriteLine(composite["Item"]);
                ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] = composite;
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e) {
            DetailView.Confirm(() => {
                this.Frame.GoBack();
            });
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) {
            DetailView.Cancel(() => { });
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e) {
            DetailView.Delete(() => {
                this.Frame.GoBack();
            });
        }
    }

    public static class BitMapConverter {
        public static async Task<byte[]> SaveToBytesAsync(this ImageSource imageSource) {
            byte[] imageBuffer;
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync("temp.jpg", CreationCollisionOption.ReplaceExisting);
            using (var ras = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None)) {
                WriteableBitmap bitmap = imageSource as WriteableBitmap;
                var stream = bitmap.PixelBuffer.AsStream();
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96.0, 96.0, buffer);
                await encoder.FlushAsync();

                var imageStream = ras.AsStream();
                imageStream.Seek(0, SeekOrigin.Begin);
                imageBuffer = new byte[imageStream.Length];
                var re = await imageStream.ReadAsync(imageBuffer, 0, imageBuffer.Length);

            }
            await file.DeleteAsync(StorageDeleteOption.Default);
            return imageBuffer;
        }

        public static async Task<ImageSource> SaveToImageSource(this byte[] imageBuffer) {
            ImageSource imageSource = null;
            using (MemoryStream stream = new MemoryStream(imageBuffer)) {
                var ras = stream.AsRandomAccessStream();
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, ras);
                var provider = await decoder.GetPixelDataAsync();
                byte[] buffer = provider.DetachPixelData();
                WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                await bitmap.PixelBuffer.AsStream().WriteAsync(buffer, 0, buffer.Length);
                imageSource = bitmap;
            }
            return imageSource;
        }
    }

}
