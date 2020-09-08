using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Roller
{
    class Program
    {
        static int Main(string[] args)
        {
            SysCall.EnableDpiAwareness();

            Console.WriteLine("Hello in Roller v0.2 I will choose proper wallpaper for you (in terms of your primary screen resolution)");
;
            // args = new[] {"https://picsum.photos/1980/1200"};
            // Check param
            string wallPath;
            if (args.Length == 1)
            {
                wallPath = args[0];
            }
            else
            {
                Console.WriteLine("You must url to wallpaper as first param! Exiting...");
                return 1;
            }

            Console.WriteLine($"Downloading wallpaper from { wallPath }");
            var downloadTask = DownloadWallpaper(wallPath);
            while (!downloadTask.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }

            Console.WriteLine();

            var photo = downloadTask.Result;
            // Setting wallpaper
            if (Roller.PaintWall(photo, Roller.Style.Center))
            {
                Console.WriteLine("Successfully changed wallpaper to https://picsum.photos/" + downloadTask.Result.PicsumId + "!");
            }
            else
            {
                Console.WriteLine("Wallpaper change failed :< (https://picsum.photos/" + downloadTask.Result.PicsumId + ")");
                return 4;
            }

            try
            {
                File.Delete(photo.TempPath);
            }
            catch
            {
                //ignored
            }

            return 0;
        }



        private static async Task<PicsumPhoto> DownloadWallpaper(string wallPath)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
            var client = new WebClient();
            await client.DownloadFileTaskAsync(new Uri(wallPath), tempPath);

            return new PicsumPhoto
            {
                TempPath = tempPath,
                PicsumId = client.ResponseHeaders["picsum-id"]
            };
        }
    }
}
