using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
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
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace hw2 {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page {

        public MainPage() {
            this.InitializeComponent();
            this.ViewModel = new TodoItemCollection();
        }

        public TodoItemCollection ViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Frame root = Window.Current.Content as Frame;
            if (root.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            NewPage page = e.Content as NewPage;
            if (page != null) {
                page.Create += new CreateTodoItem(this.ViewModel.Collection.Add);
            }
        }

        private void NewPage_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(NewPage));
        }
    }

    public delegate void CreateTodoItem(TodoItem item);
}
