using System;

namespace nah_the_search.utils;

public class CommandItem {
	 public string CommandName { get; }
	 public bool IsSearchable { get; }
	 
	 private readonly Action action;
	 private readonly Action<string>? searchableAction;

	 public CommandItem(string CommandName, Action action) {
		 this.CommandName = CommandName;
		 this.action = action;
		 this.IsSearchable = false;
	 }

	 public CommandItem(string CommandName, Action action, bool IsSearchable) {
		 this.CommandName = CommandName;
		 this.action = action;
		 this.IsSearchable = IsSearchable;
	 }

	 public CommandItem(string CommandName, Action action, bool IsSearchable, Action<string> searchableActionArg) {
		 this.CommandName = CommandName;
		 this.action = action;
		 this.IsSearchable = IsSearchable;
		 this.searchableAction = searchableAction;
	 }

	 public void Execute() {
		 action?.Invoke();
	 }

	 public void Search(string itemName) {
		 if(IsSearchable) searchableAction?.Invoke(itemName);
	 }
}
