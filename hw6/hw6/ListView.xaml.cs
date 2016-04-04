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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace hw6 {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ListView : Page {
        public ListView() {
            this.InitializeComponent();
            this.ViewModel = TodoItemCollection.Collection;
            this.SearchBox.TextChanged += SearchBox_TextChanged;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {
            TodoItemCollection.Collection.Search(this.SearchBox.Text);
        }

        private TodoItemCollection ViewModel;

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = (TodoItem)e.ClickedItem;
            var root = Window.Current.Content as Frame;
            var rootPage = root.Content as MainPage;
            rootPage.SelectItem(item);
        }
    }

    public class BoolToVisibility : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, string language) {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
