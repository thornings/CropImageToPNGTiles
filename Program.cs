using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace CropToPNGTiles
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
               Console.WriteLine("Please enter two parameters. First must be the file to be tiled and the secound a numeric tilesize.");
            } else if (args.Length==2)
            {
                int tileSize = 0;
                bool canCropIt = true;

                tileSize = CanTileIt(args, ref canCropIt);
                if (canCropIt)
                {
                    string imagePath = args[0];
                    CropAll(imagePath, tileSize).GetAwaiter().GetResult();
                }
            }
        }

        private static int CanTileIt(string[] args, ref bool canCropIt)
        {
            int tileSize;
            if (!int.TryParse(args[1], out tileSize))
            {
                Console.WriteLine("Second parameter must be a integer");
                canCropIt = false;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("First parameter must be an file");
                canCropIt = false;
            }
            return tileSize;
        }

        private static async Task CropAll(string imagePath, int tilesize)
        {
            using (var originalImage = new Bitmap(imagePath))
            {
                var height = originalImage.Height;
                var width = originalImage.Width;

                // minimal one size in both directions
                if (width >= tilesize && height >= tilesize)
                {
                    var i = 0;
                    for (int y = 0; y <= height - tilesize; y += tilesize)
                    {
                        for (int x = 0; x <= width-tilesize; x += tilesize)
                        {
                            Bitmap croppedImage;
                            Rectangle crop = new Rectangle(x, y, tilesize, tilesize);
                            croppedImage = originalImage.Clone(crop, originalImage.PixelFormat);
                            string newFilename = i+imagePath;
                            if (!File.Exists(newFilename))
                            {
                                croppedImage.Save(newFilename, ImageFormat.Png);
                            }
                            croppedImage.Dispose();
                            await Task.Delay(1);
                            i++;
                        }
                    }
                }
            }
        }
    }
}
