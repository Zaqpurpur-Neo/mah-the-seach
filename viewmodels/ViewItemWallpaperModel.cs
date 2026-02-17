using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

using nah_the_search.models;
using nah_the_search.utils;
using nah_the_search.interfaces; 

namespace nah_the_search.viewmodels;

public class ViewItemWallpaperModel : INotifyPropertyChanged, IVisibilityPanel {
	public string ViewName { get => "ListView_Wallpaper"; }

	public string wallpaperPath { get; } = 
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Wallpaper");

	public ObservableCollection<WallpaperItemModel> wallpaperItems { get; } = new ObservableCollection<WallpaperItemModel>();
	public ICollectionView itemsView { get; }

	readonly FileSystemWatcher watcher;

	private WallpaperItemModel? selectedItemVal;
	public WallpaperItemModel? selectedItem {
		get => selectedItemVal;
		set {
			selectedItemVal = value;
			OnPropertyChanged(nameof(selectedItemVal));
		}
	}

	public ICommand OpenItemCommand { get; set; }

	private void OpenItem(WallpaperItemModel item) {
		Process.Start(new ProcessStartInfo {
			FileName = item.Path,
			UseShellExecute = true
		});
	}

	public ViewItemWallpaperModel() {
		foreach (var file in Directory.GetFiles(wallpaperPath))
			TryQueue(file);

        watcher = new FileSystemWatcher(wallpaperPath);
        watcher.Created += OnCreated;
        watcher.EnableRaisingEvents = true;

		OpenItemCommand = new RelayCommand<WallpaperItemModel>(
			item => {
				if(selectedItem == null) return;
				OpenItem(selectedItem);
			});
	}

	async void OnCreated(object s, FileSystemEventArgs e) {
        await Task.Delay(200);
        TryQueue(e.FullPath);
    }

	void TryQueue(string path) {
		if (!IsImage(path))
			return;

		ImageLoader.Queue(path, item => {
            Application.Current.Dispatcher.Invoke(() => wallpaperItems.Add(item));
        });
    }

	 bool IsImage(string p) {
		 string ext = Path.GetExtension(p).ToLower();
		 return ext is ".jpg" or ".jpeg" or ".png" or ".webp";
	 }

	 /* --- Utils -- */

	 public Visibility _panelVisibility { get; set; } = Visibility.Collapsed;
	 public Visibility PanelVisibility { 
		get => _panelVisibility; 
		set {
			_panelVisibility = value;
			OnPropertyChanged(nameof(PanelVisibility));
		} 
	 }
	 public void HidePanel() { 
		if(PanelVisibility != Visibility.Collapsed)
			PanelVisibility = Visibility.Collapsed;
	 }
	 public void ShowPanel() { 
		if(PanelVisibility != Visibility.Visible)
			PanelVisibility = Visibility.Visible; 
	 }

	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string name) { 
		if(PropertyChanged != null) 
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
