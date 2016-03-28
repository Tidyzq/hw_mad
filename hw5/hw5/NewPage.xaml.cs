using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
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

namespace hw5 {
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
                this.ShareBtn.IsEnabled = true;
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
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
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
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
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

        void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args) {
            var request = args.Request;
            var item = DetailView.Item;
            request.Data.Properties.Title = item.Name;   //You MUST set a Title!
            request.Data.Properties.Description = item.Detail;
            request.Data.SetText(item.Detail + "\n" + item.DueDate.ToString());
        }

        private void ShareBtn_Click(object sender, RoutedEventArgs e) {
            DataTransferManager.ShowShareUI();
        }
    }

}
