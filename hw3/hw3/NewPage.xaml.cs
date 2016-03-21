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
    public sealed partial class NewPage : Page {

        public NewPage() {
            this.InitializeComponent();
        }

        private DetailView DetailView = new DetailView();

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Frame root = Window.Current.Content as Frame;
            if (root.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            if (e.Parameter != null) {
                DetailView.Item = (TodoItem)e.Parameter;
                this.DeleteBtn.IsEnabled = true;
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

}
