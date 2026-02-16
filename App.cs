using System;
using System.IO;
using System.Diagnostics;

using System.Configuration;
using System.Data;
using System.Windows;

namespace nah_the_search;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
		string basedir = AppContext.BaseDirectory;
		string toolsPath = System.IO.Path.Combine(basedir, "Tools");
		string dataJson = System.IO.Path.Combine(toolsPath, "apps.json");

		string scannerApp = System.IO.Path.Combine(toolsPath, "scanner.exe");

		if(!File.Exists(dataJson)) {
			Process.Start(new ProcessStartInfo {
				FileName = scannerApp,
				CreateNoWindow = true,
				UseShellExecute = false 
			});
		}

    }
}

