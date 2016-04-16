using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace hw7
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            getCheckCode();
        }

        private async void getCheckCode() {
            var success = await Model.JWXT.getCookie();
            if (success) {
                var checkcode = await Model.JWXT.getCheckCode();
                CheckCode.Source = checkcode;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (Frame.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e) {
            var error = await Model.JWXT.login(UsernameTextBox.Text, PasswordTextBox.Password, CheckcodeTextBox.Text);
            if (error == "") {
                Frame.Navigate(typeof(ScorePage));
            } else {
                await new MessageDialog(error).ShowAsync();
                getCheckCode();
            }
        }

        private void CheckCode_Click(object sender, RoutedEventArgs e) {
            getCheckCode();
        }
    }
}
