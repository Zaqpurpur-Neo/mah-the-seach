using System;
using System.Windows;

namespace nah_the_search.utils;

public class CommandItem {
	 public string CommandName { get; }
	 public bool IsSearchable { get; }
	 
	 private readonly Action<string> action;
	 private readonly Action<string>? searchableAction;

	 public CommandItem(string CommandName, Action<string> action) {
		 this.CommandName = CommandName;
		 this.action = action;
		 this.IsSearchable = false;
	 }

	 public CommandItem(string CommandName, Action<string> action, bool IsSearchable) {
		 this.CommandName = CommandName;
		 this.action = action;
		 this.IsSearchable = IsSearchable;
	 }

	 public CommandItem(string CommandName, Action<string> action, 
			 bool IsSearchable, Action<string> searchableAction) {

		 this.CommandName = CommandName;
		 this.action = action;
		 this.IsSearchable = IsSearchable;
		 this.searchableAction = searchableAction;
	 }

	 public void Execute(string text) {
		 action?.Invoke(text);
	 }

	 public void Search(string itemName) {
		 if(IsSearchable) searchableAction?.Invoke(itemName);
	 }
}
