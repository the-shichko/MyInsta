﻿using InstagramApiSharp.Classes.Models;
using MyInsta.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyInsta.Logic;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MyInsta.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookmarksPage : Page
    {
        public BookmarksPage()
        {
            InitializeComponent();

            //InstaServer.OnBookmarkUserLoaded += () => 
        } 

        public User InstaUser { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InstaUser = e.Parameter as User;
        }

        private void MainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is InstaUserShort user)
            {
                Frame.Navigate(typeof(PersonPage), new object[] { user, InstaUser });
            }
        }
    }
}
