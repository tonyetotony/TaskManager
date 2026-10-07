using System.IO;
using System.Windows.Media.Imaging;
using TaskManager.Common.Models;

namespace TaskManager.Client.Models.Extensions;

public static class CommonModelExtensions
{
    public static BitmapImage LoadImage(this CommonModel model)
    {
        if(model.Image == null || model.Image.Length == 0)
            return null;
            
        var image = new BitmapImage();
        using (var memSm = new MemoryStream(model.Image))
        {
            memSm.Position = 0;
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = null;
            image.StreamSource = memSm;
            image.EndInit();
        }
        image.Freeze();
        return image;
    }
}