﻿using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace CustomTabBar
{
    // TODO: setup tab click events
    // TODO: setup coordination of IsVisible properties between TabViews and TabContentViews
    // TODO: setup UI in TabContentView
    // TODO: make more generic by providing bindable properties, like ItemsSource
    // TODO: make more generic by allowing developer to set Type for both TabView and ContentView, much how ListView allows for custom cell types.
    // TODO: allow for tab width to be set to equidistant or content-determined width. This will probably involve a Grid for the tabs instead of a RelativeLayout.
    // TODO: make a (bindable) property that allows the tabs to be specified to exist at the top or bottom of the page

    /// <summary>
    /// A platform-agnostic tabbed page that presents tabs and content for a given collection.
    /// </summary>
    public class CustomTabbedPage : ContentPage
    {
        CustomTabbedContentViewModel ViewModel
        {
            get { return BindingContext as CustomTabbedContentViewModel; }
        }

        /// <summary>
        /// Keeps track of the tab views to coordinate tab clicks with the proper content view.
        /// </summary>
        readonly List<TabView> _TabViews = new List<TabView>();
        /// <summary>
        /// Keeps track of the content views to coordinate tab clicks with the proper content view.
        /// </summary>
        readonly List<TabContentView> _TabContentViews = new List<TabContentView>();

        public CustomTabbedPage(CustomTabbedContentViewModel viewModel)
        {
            BindingContext = viewModel;

            // Set this value to control tab height. The height of the content will be adjusted accordingly to take up the rest of the height.
            const double tabHeight = 60;

            // The only real purpose of this view is to allow padding on iOS
            ContentView mainContainer = new ContentView() { BackgroundColor = Color.Silver };
            Device.OnPlatform(iOS: () => mainContainer.Padding = new Thickness(0, 20, 0, 0)); // deal with iOS status bar

            // the layout container for the child layouts: tabBarLayout and contentViewLayout
            RelativeLayout mainLayout = new RelativeLayout();

            // the layout container for the tab views
            RelativeLayout tabBarLayout = new RelativeLayout();

            // the layout container for the content views
            RelativeLayout contentViewLayout = new RelativeLayout();

            // iterate over the ViewModel.Items colletcion and build up the tab and content views
            for (int i = 0; i < ViewModel.Items.Count; i++)
            {
                // This is necessary in order to maintain the value of i for the deferred execution of the Constraint calculation,
                // because the value of i seemes to return to 0 at the time of deferred execution.
                double j = i;

                // build a TabView
                TabView tabView = new TabView(ViewModel.Items[i], i) { BackgroundColor = Color.Teal };

                // store for reference
                _TabViews.Add(tabView);

                // add to layout
                tabBarLayout.Children.Add(
                    view: tabView,
                    xConstraint: Constraint.RelativeToParent(parent => parent.Width / (double)ViewModel.Items.Count * j), 
                    widthConstraint: Constraint.RelativeToParent(parent => parent.Width / (double)ViewModel.Items.Count),
                    heightConstraint: Constraint.Constant(tabHeight)
                );

                // build a TabContentView
                TabContentView tabContentView = new TabContentView(ViewModel.Items[i]);

                // store for reference
                _TabContentViews.Add(tabContentView);

                // TODO: Add tabContentView to contentViewLayout, similar to how TabViews are being added to the tabBarLayout
                contentViewLayout.Children.Add(
                    view: tabContentView,
                    widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
                    heightConstraint: Constraint.RelativeToParent(parent => parent.Height)
                );
            }

            // add the tabBarLayout to the mainLayout
            mainLayout.Children.Add(
                view: tabBarLayout,
                yConstraint: Constraint.RelativeToParent(parent => parent.Height - tabHeight),
                heightConstraint: Constraint.Constant(tabHeight),
                widthConstraint: Constraint.RelativeToParent(parent => parent.Width));

            // add the contentViewLayout to the mainLayout
            mainLayout.Children.Add(
                view: contentViewLayout,
                heightConstraint: Constraint.RelativeToView(tabBarLayout, (parent, view) => parent.Height - view.Height),
                widthConstraint: Constraint.RelativeToParent(parent => parent.Width));

            // assign mainLayout to mainContainer's Content (this is the wrapper needed for iOS mentioned above)
            mainContainer.Content = mainLayout;

            // assign mainContainer to Content
            Content = mainLayout;
        }
    }
}

