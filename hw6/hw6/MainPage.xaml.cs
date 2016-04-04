using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Notifications;
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

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace hw6 {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page {

        public MainPage() {
            this.InitializeComponent();
        }

        private ListView ListView = new ListView();
        private DetailView DetailView = new DetailView();

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if (Frame.CanGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
            base.OnNavigatedFrom(e);
        }

        private void NewPageBtn_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(NewPage));
        }

        public void SelectItem(TodoItem item) {
            if (this.VisualStateGroup.CurrentState == this.VisualStateMin0) {
                int index = TodoItemCollection.Collection.IndexOf(item);
                this.Frame.Navigate(typeof(NewPage), index);
            } else {
                DetailView.Item = item;
                this.DeleteBtn.IsEnabled = true;
                this.ShareBtn.IsEnabled = true;
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e) {
            DetailView.Confirm(() => {
                DetailView.Item = null;
                this.DeleteBtn.IsEnabled = false;
                this.ShareBtn.IsEnabled = false;
            });
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) {
            DetailView.Cancel(() => { });
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e) {
            DetailView.Delete(() => {
                DetailView.Item = null;
                this.DeleteBtn.IsEnabled = false;
                this.ShareBtn.IsEnabled = false;
            });
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e) {
            var updator = TileUpdateManager.CreateTileUpdaterForApplication();
            string TileTemplateXml = @"
                <tile branding='name'> 
                  <visual version='3'>
                    <binding template='TileMedium'>
                      <text>{0}</text>
                      <text hint-style='captionsubtle' hint-wrap='true'>{1}</text>
                    </binding>
                    <binding template='TileWide'>
                      <text>{0}</text>
                      <text hint-style='captionsubtle' hint-wrap='true'>{1}</text>
                    </binding>
                    <binding template='TileLarge'>
                      <text>{0}</text>
                      <text hint-style='captionsubtle' hint-wrap='true'>{1}</text>
                    </binding>
                  </visual>
                </tile>";
            var item = TodoItemCollection.Collection.Last();
            var doc = new XmlDocument();
            var xml = string.Format(TileTemplateXml, item.Name, item.Detail);
            doc.LoadXml(WebUtility.HtmlDecode(xml), new XmlLoadSettings {
                ProhibitDtd = false,
                ValidateOnParse = false,
                ElementContentWhiteSpace = false,
                ResolveExternals = false
            });
            updator.Update(new TileNotification(doc));
        }

        void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args) {
            var request = args.Request;
            var item = DetailView.Item;
            request.Data.Properties.Title = item.Name;   //You MUST set a Title!
            request.Data.Properties.Description =item.Detail;
            request.Data.SetText(item.Detail + "\n" + item.DueDate.ToString());
        }

        private void ShareBtn_Click(object sender, RoutedEventArgs e) {
            DataTransferManager.ShowShareUI();
        }
    }
}
