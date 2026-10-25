using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace Rottytooth.Esolang.LightPattern
{
    /// <summary>
    /// Retrieves metadata and color from photo
    /// </summary>
    public class ImageAnalyzer
    {
        public static void ExtractMetaData(Image theImage)
        {
            try
            {
                PropertyItem[] propertyItems = theImage.PropertyItems;
                int num = 0;
                foreach (PropertyItem item in propertyItems)
                {
                    Console.Write("Property Item " + num.ToString());
                    Console.Write("   ID: 0x" + item.Id.ToString("x"));
                    Console.Write("   type: " + item.Type.ToString());
                    Console.Write("   length: " + item.Len.ToString());
                    Console.WriteLine();
                    num++;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public static DominantColor GetAverageColor(Image theImage)
        {
            Bitmap bitmap = new Bitmap(theImage);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapdata = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int length = bitmapdata.Stride * bitmap.Height;
            byte[] destination = new byte[length];
            Marshal.Copy(bitmapdata.Scan0, destination, 0, length);
            bitmap.UnlockBits(bitmapdata);
            long red = 0L;
            long green = 0L;
            long blue = 0L;
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    red += pixel.R;
                    green += pixel.G;
                    blue += pixel.B;
                }
            }
            return new DominantColor(red, green, blue);
        }
    }
}

