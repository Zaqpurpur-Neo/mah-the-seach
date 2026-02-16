using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;

using System.Collections;
using System.Collections.ObjectModel;

using System;
using System.IO;
using System.Diagnostics;

using nah_the_search.models;
using nah_the_search.utils;
using nah_the_search.viewmodels;

using System.Windows.Input;

namespace nah_the_search;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
	private HotkeyManager hotkey;
	private TrayIconManager tray;

    public MainWindow() {
		InitializeComponent();
		Loaded += OnLoaded;

    }

	private void ToggleWindow() {
		Dispatcher.Invoke(() => {
			if (IsVisible) {
				Hide();
			} else {
				Show();
				Activate();
				SearchInput.Focus();
			}
		});
    }

	protected override void OnClosed(EventArgs e) {
		hotkey.Dispose();
		base.OnClosed(e);
    }

	protected override void OnKeyDown(KeyEventArgs e) {
		switch (e.Key) {
			case Key.Escape:
				Hide();
				break;
		    default:
				break;
		}
    }

	private void OnLoaded(object sender, RoutedEventArgs e) {
		var helper = new WindowInteropHelper(this);
		hotkey = new HotkeyManager(helper.Handle);
		hotkey.HotkeyPressed += ToggleWindow;

		tray = new TrayIconManager(this, "Raycast Copy");
		tray.ShowRequested += () => {
			Dispatcher.Invoke(() => {
				Show();
				Activate();
			});
		};

		tray.ExitRequested += () => {
			Dispatcher.Invoke(() => {
				Application.Current.Shutdown();
			});
		};
		this.Hide();
    }

	private void SearchInput_KeyDown(object sender, KeyEventArgs e) {
		if(e.Key == Key.Enter) {
			if(DataContext is MainViewModel mvm) {
				mvm.CommandEnter();
			}
		}
	}

	/* -- */

	private void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
    	if (e.OriginalSource == sender)
        	this.DragMove();
	}

	private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
		if(InnerScrollViewer == null) return;

		InnerScrollViewer.ScrollToVerticalOffset(
			InnerScrollViewer.VerticalOffset - e.Delta
		);

		e.Handled = true;
	}

	private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
		var item = sender as ListViewItem;

		if (item != null) {
			var dataItem = item.DataContext as ItemAppModel;
			if(dataItem == null) return;
			// MessageBox.Show($"Double clicked on item: {dataItem.subTitle}");
			Process.Start(new ProcessStartInfo {
				FileName = dataItem.lnkPath,
				UseShellExecute = true
			});
		}
	}

	private void ListViewWallpaperItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
		var item = sender as ListViewItem;
		if (item != null) {
			var dataItem = item.DataContext as WallpaperItemModel;
			if(dataItem == null) return;

			Process.Start(new ProcessStartInfo {
				FileName = dataItem.Path,
				UseShellExecute = true
			});
		}
	}

	private void ResultsList_Loaded(object sender, RoutedEventArgs e) {
    	if(ListView_Applications.Visibility == Visibility.Visible)
			ListView_Applications.Focus();
	}

}
