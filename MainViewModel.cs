using System.Collections.ObjectModel;
using System.ComponentModel;

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;

using nah_the_search.viewmodels;
using nah_the_search.interfaces;
using nah_the_search.utils;

namespace nah_the_search;

public class MainViewModel : INotifyPropertyChanged {
	public ViewItemAppModel FilteredItemAppView { get; } = new ViewItemAppModel();
	public ViewItemWallpaperModel ItemWallpaperView { get; } = new ViewItemWallpaperModel();

	public IVisibilityPanel? visibleElement { get; set; }

	public CommandList commandList { get; } = new CommandList();
	
	private string _globalText;
	public string globalText {
		get => _globalText;
		set {
			_globalText = value;
			OnPropertyChanged(nameof(globalText));

			TextChangedCommand((string) value);
		}
	}

	public event Action? RequestClose;
	public ICommand CloseCommand() => new RelayCommand<string>(act => { RequestClose?.Invoke(); });

	public MainViewModel() {
		commandList.addDefaultCommand(() => {
			ItemWallpaperView.HidePanel();
			FilteredItemAppView.HidePanel();
			visibleElement = null;
		});

		commandList.addCommand("/wallpaper", () => {
			ItemWallpaperView.ShowPanel();

			if(visibleElement != null && visibleElement != ItemWallpaperView)
				visibleElement.HidePanel();

			if(visibleElement != ItemWallpaperView) {
				visibleElement = ItemWallpaperView;
			}
		});

		commandList.addCommand("/app", 
			() => {
				FilteredItemAppView.ShowPanel();

				if(visibleElement != null && visibleElement != FilteredItemAppView)
					visibleElement.HidePanel();
				
				if(visibleElement != FilteredItemAppView) {
					visibleElement = FilteredItemAppView;
				}
			},
			(name) => {
				if(visibleElement == FilteredItemAppView) FilteredItemAppView.filterText = globalText;
			}
		);
	}

	public void CommandEnter() {
		if(globalText.StartsWith("/")) {
			string[] args = globalText.Split(' ');
			CommandItem cmd = commandList.findCommand(args[0]);
			if(cmd == null) 
				cmd = commandList.getDefaultCommand();

			if(cmd != null) {
				cmd.Execute();
			}
		}
	}

	private void TextChangedCommand(string text) {
		if(globalText.StartsWith("/")) {
			string[] args = text.Split(' ');
			CommandItem cmd = commandList.findCommand(args[0]);
			if(cmd != null) {
				string resultValue = String.Join(" ", args[1..]);
				cmd.Search(resultValue);
			};
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
