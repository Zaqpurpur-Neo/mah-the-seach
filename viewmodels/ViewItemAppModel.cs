using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text.Json;
using raycast_copy.models;
using raycast_copy.models;
using raycast_copy.interfaces; 

namespace raycast_copy.viewmodels;

public class ViewItemAppModel : INotifyPropertyChanged, IVisibilityPanel {
	public string ViewName { get => "ListView_Applications"; }

	private string basedir { get => AppContext.BaseDirectory; }
	private string toolsPath { get => System.IO.Path.Combine(basedir, "Tools"); }
	private string dataJson { get => System.IO.Path.Combine(toolsPath, "apps.json"); }

	private class AppWrapperModel {
		public List<JsonAppModel>? apps { get; set; }
	}

	public ObservableCollection<ItemAppModel> appItems { get; } = new ObservableCollection<ItemAppModel>();

	public ICollectionView itemsView { get; }
	private string? filteredString;
	public string? filterText {
		get => filteredString;
		set {
			filteredString = value;
			OnPropertyChanged(nameof(filterText));
			itemsView.Refresh();
		}
	}

	private ItemAppModel? selectedItemVal;
	public ItemAppModel? selectedItem {
		get => selectedItemVal;
		set {
			selectedItemVal = value;
			OnPropertyChanged(nameof(selectedItemVal));
		}
	}

	public ICommand OpenItemCommand { get; set; }

	private void OpenItem(ItemAppModel item) {
		Process.Start(new ProcessStartInfo {
			FileName = item.lnkPath,
			UseShellExecute = true
		});
	}

	public ViewItemAppModel() {
		if(File.Exists(dataJson)) {
			string jsonString = File.ReadAllText(dataJson);

			if(jsonString != null) {
				AppWrapperModel model = JsonSerializer.Deserialize<AppWrapperModel>(jsonString);
				if(model != null) {
					if(model.apps != null) {
						model.apps.Sort((a,b) => a.name.ToLower().CompareTo(b.name.ToLower()));

						foreach (JsonAppModel item in model.apps) {
							string[] subTitleArr = item.lnk.Split('\\');
							appItems.Add(new ItemAppModel { title = item.name, subTitle = subTitleArr[subTitleArr.Length - 1], lnkPath = item.lnk} );	
						}
					} else {
						appItems.Add(new ItemAppModel { title = "None", 
								subTitle = (model.apps == null) ? "is null" : "not null" } );
					}
				}
			}
		}

		itemsView = CollectionViewSource.GetDefaultView(appItems);
		itemsView.Filter = FilterItem;

		OpenItemCommand = new RelayCommand<ItemAppModel>(
			item => {
				if(selectedItem == null) return;
				OpenItem(selectedItem);
			});
	}

	private bool FilterItem(object obj) {
		if(filterText == null) return true;
		if(string.IsNullOrWhiteSpace(filterText)) return true;

		if(filterText.StartsWith("/")) return false;

		if(obj is ItemAppModel item) {
			if(item == null || item.title == null) return false;
			return item.title.Contains(filterText, StringComparison.OrdinalIgnoreCase);
		}

		return false;
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string name) { 
		if(PropertyChanged != null) 
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

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
}
