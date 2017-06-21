using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace StoryCanvas.UWP.View
{
    static class ViewUtil
    {
        /// <summary>
        /// ストリームからImageコントロールのImageSourceに変換する
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ImageSource ToImageSource(this Stream stream)
        {
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            var bmp = new BitmapImage();
            using (var rstream = new InMemoryRandomAccessStream())
            {
                Task.Run(async () =>
                {
                    await rstream.WriteAsync(data.AsBuffer());
                }).Wait();
                rstream.Seek(0);
                bmp.SetSource(rstream);
            }
            return bmp;
        }
    }
}
