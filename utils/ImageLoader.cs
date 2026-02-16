using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using nah_the_search.models;
namespace nah_the_search.utils;

public static class ImageLoader {
	static readonly ConcurrentQueue<(string path, Action<WallpaperItemModel> cb)> queue = new();
	static bool running;

	public static void Queue(string path, Action<WallpaperItemModel> callback) {
		queue.Enqueue((path, callback));
		Run();
	}

	static void Run() {
		if (running) return;
		running = true;

		Task.Run(async () => {
			while (queue.TryDequeue(out var job)) {
				var item = new WallpaperItemModel(job.path);

				_ = item.Thumbnail; // force generate cache

				job.cb(item);

				await Task.Delay(10); // throttle CPU
			}

			running = false;
		});
	}
}

