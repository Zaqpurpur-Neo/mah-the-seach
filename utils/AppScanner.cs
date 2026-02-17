using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace nah_the_search.utils;

public class AppScanner {
	public class ShortcutInfo {
		public string name { get; set; }
		public string lnk { get; set; }
		public string target { get; set; }
		public string args { get; set; }
	}

    public void scannow() {
        var apps = new List<ShortcutInfo>();

        var roots = new[]
        {
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Microsoft\Windows\Start Menu\Programs"
            ),
            @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs"
        };

        foreach (var root in roots)
        {
            if (!Directory.Exists(root))
                continue;

            foreach (var file in Directory.EnumerateFiles(root, "*.lnk", SearchOption.AllDirectories))
            {
                try
                {
                    var (target, args) =  ShellLinkResolver.Resolve(file);;

                    apps.Add(new ShortcutInfo
                    {
                        name = Path.GetFileNameWithoutExtension(file),
                        lnk = file,
                        target = target,
                        args = args
                    });
                }
                catch {
                }
            }
        }

        var output = new { apps };

        Directory.CreateDirectory("Tools");

        File.WriteAllText(
            "Tools/apps.json",
            JsonSerializer.Serialize(output, new JsonSerializerOptions
            {
                WriteIndented = true
            })
        );
    }
}

