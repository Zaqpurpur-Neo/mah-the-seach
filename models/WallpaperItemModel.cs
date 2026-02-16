using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using raycast_copy.utils;

namespace raycast_copy.models;

public class WallpaperItemModel : INotifyPropertyChanged {
    public string Path { get; }

	public BitmapImage Thumbnail => ThumbnailCache.Get(Path);

    public WallpaperItemModel(string path) {
        Path = path;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

