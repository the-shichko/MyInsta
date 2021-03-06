﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyInsta.Logic;
using MyInsta.Model;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using InstagramApiSharp.Classes.Models;
using UserSettings = MyInsta.Logic.UserSettings;

namespace MyInsta.View
{
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
            Window.Current.SetTitleBar(BackgroundElement);

            InstaServer.OnErrorGetting += (error) =>
            {
                _ = new CustomDialog("Warning", error, "Close");
            };

            NavView.IsPaneOpen = UserSettings.IsMenuOpen;
            UserSettings.OnPaneModeChanged += () => Bindings.Update();
        }

        User InstaUser { get; set; }
        public NavigationViewPaneDisplayMode PaneMode => UserSettings.PaneMode;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
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
            var itemContent = args.InvokedItemContainer?.Tag;
            if (itemContent != null)
            {
                switch (itemContent)
                {
                    case "Followers":
                        contentFrame.Navigate(typeof(FollowersPage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Unfollowers":
                        contentFrame.Navigate(typeof(UnfollowersPage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Friends":
                        contentFrame.Navigate(typeof(FriendsPage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Search":
                        contentFrame.Navigate(typeof(SearchPage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Sync":
                        await InstaServer.GetUserData(InstaUser, true);
                        break;
                    case "Saved":
                        contentFrame.Navigate(typeof(PostsPage), new object[]
                            {
                                InstaUser,
                                1
                            }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Bookmarks":
                        contentFrame.Navigate(typeof(BookmarksPage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Stories":
                        contentFrame.Navigate(typeof(StoriesPage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Feed":
                        contentFrame.Navigate(typeof(FeedPage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Direct":
                        contentFrame.Navigate(typeof(Direct), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Preview":
                        contentFrame.Navigate(typeof(PreviewPostsPage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Explore":
                        contentFrame.Navigate(typeof(ExplorePage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "User":
                        var curt = await InstaServer.GetInstaUserShortById(InstaUser, InstaUser.UserData.Pk);
                        contentFrame.Navigate(typeof(PersonPage),
                            new object[] { curt, InstaUser }, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                        break;
                    case "Archive":
                        contentFrame.Navigate(typeof(ArchivePage), InstaUser, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
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

            var preNavPageType = contentFrame.CurrentSourcePageType;
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                contentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Feed")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(FeedPage), InstaUser);
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
            ("Feed", typeof(FeedPage)),
            ("User", typeof(PersonPage)),
            ("Archive", typeof(ArchivePage)),
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
            {
                return false;
            }

            contentFrame.GoBack();
            return true;
        }

        private void MainUser_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(AccountPage), InstaUser);
        }

        private void NavView_OnPaneOpened(NavigationView sender, object args)
        {
            UserSettings.IsMenuOpen = true;
        }

        private void NavView_OnPaneClosed(NavigationView sender, object args)
        {
            UserSettings.IsMenuOpen = false;
        }
    }
}
