﻿using System;
using System.Collections.Generic;
using System.Linq;
using MyInsta.Logic;
using MyInsta.Model;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

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
            InitializeComponent();

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

        private async void NavigationViewControl_ItemInvoked(NavigationView sender,
            NavigationViewItemInvokedEventArgs args)
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
                        contentFrame.Navigate(typeof(PostsPage), new object[]
                            {
                                InstaUser,
                                1
                            });
                        break;
                    case "Bookmarks":
                        contentFrame.Navigate(typeof(BookmarksPage), InstaUser);
                        break;
                    case "Stories":
                        contentFrame.Navigate(typeof(StoriesPage), InstaUser);
                        break;
                    case "Direct":
                        contentFrame.Navigate(typeof(Direct), InstaUser);
                        break;
                    case "Preview":
                        contentFrame.Navigate(typeof(PreviewPostsPage), InstaUser);
                        break;
                }
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "Settings")
            {
                _page = typeof(SettingPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = contentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                contentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Followers")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(FollowersPage), InstaUser);
            contentFrame.Navigated += On_Navigated;
        }

        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("Followers", typeof(FollowersPage)),
            ("Unfollowers", typeof(UnfollowersPage)),
            ("Friends", typeof(FriendsPage)),
            ("Search", typeof(SearchPage)),
            ("Saved", typeof(PostsPage)),
            ("Bookmarks", typeof(BookmarksPage)),
            ("Stories", typeof(StoriesPage)),
            ("Preview", typeof(PreviewPostsPage))
        };

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = contentFrame.CanGoBack;

            if (contentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                if (item.Page != null)
                    NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>()
                                               .First(n => n.Tag.Equals(item.Tag));
            }
        }

        private void NavigationViewControl_BackRequested(NavigationView sender,
            NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (!contentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen)
                return false;

            contentFrame.GoBack();
            return true;
        }
    }
}
