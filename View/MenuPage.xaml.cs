﻿using MyInsta.Logic;
using MyInsta.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace MyInsta.View
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MenuPage : Page
    {
        public MenuPage()
        {
            this.InitializeComponent();

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            CoreApplicationViewTitleBar bar = CoreApplication.GetCurrentView().TitleBar;
            bar.ExtendViewIntoTitleBar = true;
        }
        User InstaUser { get; set; }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            InstaUser = e.Parameter as User;
            await InstaServer.GetUserData(InstaUser);
        }

        private async void NavigationViewControl_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                contentFrame.Navigate(typeof(SettingPage), InstaUser);
            }
            var ItemContent = args.InvokedItemContainer.Tag;
            if (ItemContent != null)
            {
                switch (ItemContent)
                {
                    case "Followers":
                        contentFrame.Navigate(typeof(FollowersPage), InstaUser);
                        break;
                    case "Unfollowers":
                        contentFrame.Navigate(typeof(UnfollowersPage), InstaUser);
                        break;
                    case "Friends":
                        contentFrame.Navigate(typeof(FriendsPage), InstaUser);
                        break;
                    case "Search":
                        contentFrame.Navigate(typeof(SearchPage), InstaUser);
                        break;
                    case "Sync":
                        await InstaServer.GetUserData(InstaUser);
                        break;
                    case "Saved":
                        contentFrame.Navigate(typeof(PostsPage), new object[] { InstaUser, 1 });
                        break;
                    case "Bookmarks":
                        contentFrame.Navigate(typeof(BookmarksPage), InstaUser);
                        break;
                    case "Stories":
                        contentFrame.Navigate(typeof(StoriesPage), InstaUser);
                        break;
                    case "Preview":
                        contentFrame.Navigate(typeof(PreviewPostsPage), InstaUser);
                        break;
                }
            }
        }

        private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (NavigationViewItemBase item in NavigationViewControl.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Followers")
                {
                    NavigationViewControl.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(FollowersPage), InstaUser);
        }

    }
}
