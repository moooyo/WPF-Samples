using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGallery.Controls
{
    /// <summary>
    /// Interaction logic for TileGallery.xaml
    /// </summary>
    public partial class TileGallery : UserControl
    {
        public TileGallery()
        {
            InitializeComponent();
        }

        private void ScrollBackButton_Click(object sender, RoutedEventArgs e)
        {
            double newOffSet = RootScrollViewer.HorizontalOffset - 210;
            RootScrollViewer.ChangeView(newOffSet, null, null);
            UpdateScrollButtonsVisibility(newOffSet);
        }

        private void ScrollForwardButton_Click(object sender, RoutedEventArgs e)
        {
            double newOffSet = RootScrollViewer.HorizontalOffset + 210;
            RootScrollViewer.ChangeView(newOffSet, null, null);
            UpdateScrollButtonsVisibility(newOffSet);
        }

        private void UpdateScrollButtonsVisibility()
        {
            double offset = RootScrollViewer.HorizontalOffset;
            UpdateScrollButtonsVisibility(offset);
        }

        private void UpdateScrollButtonsVisibility(double newOffset)
        {
            ScrollBackButton.Visibility = Visibility.Visible;
            ScrollForwardButton.Visibility = Visibility.Visible;

            if (RootScrollViewer.ActualWidth < TilesPanel.ActualWidth)
            {
                if (newOffset == 0)
                {
                    ScrollBackButton.Visibility = Visibility.Collapsed;
                }
                else if (newOffset >= RootScrollViewer.ScrollableWidth)
                {
                    ScrollForwardButton.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ScrollBackButton.Visibility = Visibility.Collapsed;
                ScrollForwardButton.Visibility = Visibility.Collapsed;
            }
        }

        private void RootScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateScrollButtonsVisibility();
        }
    }
}
