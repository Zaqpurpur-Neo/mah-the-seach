using System.Collections.ObjectModel;
using System.ComponentModel;

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;

using System.IO;
using System.Diagnostics;

using nah_the_search.viewmodels;
using nah_the_search.interfaces;
using nah_the_search.utils;
using nah_the_search.models;

namespace nah_the_search;

public class MainViewModel : INotifyPropertyChanged {
	public ViewItemAppModel FilteredItemAppView { get; } = new ViewItemAppModel();
	public ViewItemWallpaperModel ItemWallpaperView { get; } = new ViewItemWallpaperModel();

	public IVisibilityPanel? visibleElement { get; set; }

	public CommandList commandList { get; } = new CommandList();
	public CommandItem? selectedCommand { get; set; }
	
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

	public MainViewModel() {
		commandList.addDefaultCommand((_) => {
			ItemWallpaperView.HidePanel();
			FilteredItemAppView.HidePanel();
			visibleElement = null;
			selectedCommand = null;
		});

		commandList.addCommand("/exit", (_) => {
			if(RequestClose != null) {
				RequestClose.Invoke();
			}
		});

		commandList.addCommand("/chrome", (text) => {
			string[] splittedText = text.Split(" ");
			string? paramater = null;

			if(splittedText.Length > 1)
				paramater = String.Join(" ", splittedText[1..]);

			if(FilteredItemAppView.appItems.Count > 0) {
				ItemAppModel chromeItem = FilteredItemAppView.appItems.FirstOrDefault(item => item.title == "chrome");
				if(chromeItem != null) {
					Process.Start(new ProcessStartInfo {
						FileName = chromeItem.lnkPath,
						Arguments = paramater,
						UseShellExecute = true
					});			
				}
			}
		});

		commandList.addCommand("/google", (text) => {
			string[] splittedText = text.Split(" ");
			string googleUrl = "https://www.google.com";
			string? paramater = null;

			if(splittedText.Length > 1)
				paramater = String.Join(" ", splittedText[1..]);
			
			if(paramater != null)
				googleUrl += $"/search?q={Uri.EscapeDataString(paramater)}";

			if(FilteredItemAppView.appItems.Count > 0) {
				ItemAppModel chromeItem = FilteredItemAppView.appItems.FirstOrDefault(item => item.title == "chrome");
				if(chromeItem != null) {
					Process.Start(new ProcessStartInfo {
						FileName = chromeItem.lnkPath,
						Arguments = paramater,
						UseShellExecute = true
					});			
				}
			}
		});

		
		commandList.addCommand("/wallpaper", (_) => {
			ItemWallpaperView.ShowPanel();

			if(visibleElement != null && visibleElement != ItemWallpaperView)
				visibleElement.HidePanel();

			if(visibleElement != ItemWallpaperView) {
				visibleElement = ItemWallpaperView;
			}
		});

		commandList.addCommand("/app", 
			(_) => {
				FilteredItemAppView.ShowPanel();

				if(visibleElement != null && visibleElement != FilteredItemAppView)
					visibleElement.HidePanel();
				
				if(visibleElement != FilteredItemAppView) {
					visibleElement = FilteredItemAppView;
				}
			},
			(name) => {
				if(visibleElement == FilteredItemAppView) 
					FilteredItemAppView.filterText = name;
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
				selectedCommand = cmd;
				cmd.Execute(globalText);
			}
		}
	}

	private void TextChangedCommand(string text) {
		if(globalText.StartsWith("/")) {
			string[] args = text.Split(' ');
			if(selectedCommand != null) {
				if(args.Length > 0) {
					string resultValue = String.Join(" ", args[1..]);
					selectedCommand.Search(resultValue);
				}
			};
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
