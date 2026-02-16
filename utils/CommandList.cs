using System;
using System.Collections.Generic; 

namespace nah_the_search.utils;

public class CommandList {
	private Dictionary<string, CommandItem> commandList { get; set; } = new Dictionary<string, CommandItem>();
	private string DefaultCommand { get; } = "__action_hidden__default_command_action_001";

	public void addDefaultCommand(Action action) {
		commandList[DefaultCommand] = new CommandItem(DefaultCommand, action);
	}

	public void addCommand(string commandName, Action action) {
		commandList[commandName] = new CommandItem(commandName, action);
	}

	public void addCommand(string commandName, Action action, Action<string> searchAction) {
		commandList[commandName] = new CommandItem(commandName, action, true, searchAction);
	}

	public CommandItem? findCommand(string commandName) {
		return commandList.GetValueOrDefault(commandName);
	}

	public CommandItem? getDefaultCommand() {
		return commandList.GetValueOrDefault(DefaultCommand);
	}
}
