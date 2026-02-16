using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;

namespace nah_the_search.utils;

public static class ThumbnailCache {
    static readonly string CacheDir =
        Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData),
            "RaycastCopy-GalleryCache");

    static ThumbnailCache() {
        Directory.CreateDirectory(CacheDir);
    }

    public static BitmapImage Get(string path, int size = 300) {
        string key = GetHash(path + File.GetLastWriteTimeUtc(path));
        string cachePath = Path.Combine(CacheDir, key + ".jpg");

        if (File.Exists(cachePath))
            return Load(cachePath);

        var bmp = CreateThumbnail(path, size);
        SaveJpeg(bmp, cachePath);
        return bmp;
    }

    static BitmapImage CreateThumbnail(string path, int size) {
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(path);
        bmp.DecodePixelWidth = size;
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
    }

    static BitmapImage Load(string path) {
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(path);
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
    }

    static void SaveJpeg(BitmapSource src, string path) {
        using var fs = new FileStream(path, FileMode.Create);
        var enc = new JpegBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(src));
        enc.QualityLevel = 85;
        enc.Save(fs);
    }

    static string GetHash(string text) {
        using var sha = SHA1.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(bytes);
    }
}

