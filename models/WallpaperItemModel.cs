using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using nah_the_search.utils;

namespace nah_the_search.models;

public class WallpaperItemModel : INotifyPropertyChanged {
    public string Path { get; }

	public BitmapImage Thumbnail => ThumbnailCache.Get(Path);

    public WallpaperItemModel(string path) {
        Path = path;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

