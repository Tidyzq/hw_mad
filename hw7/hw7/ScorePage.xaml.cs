using System;
using System.Collections.ObjectModel;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace hw7 {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ScorePage : Page {

        private ObservableCollection<Model.CourseScore> Collection = new ObservableCollection<Model.CourseScore>();

        public ScorePage() {
            this.InitializeComponent();
            for (int year = 2012; year < DateTime.Now.Year; ++year) {
                YearPicker.Items.Add(string.Format("{0}-{1}", year, year + 1));
            }
            YearPicker.SelectedIndex = YearPicker.Items.Count - 1;
            for (int term = 1; term <= 3; ++term) {
                TermPicker.Items.Add(string.Format("{0}", term));
            }
            TermPicker.SelectedIndex = 0;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (Frame.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private async void getScore(string year, string term) {
            var collection = await Model.JWXT.getScore(year, term);
            Collection.Clear();
            for (int i = 0; i < collection.Count; ++i) {
                Collection.Add(collection[i]);
            }
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e) {
            getScore((string)YearPicker.SelectedItem, (string)TermPicker.SelectedItem);
        }
    }

}
