using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;

namespace Rottytooth.Esolang.LightPattern
{
    public class ExifSummary
    {
        public const int EXPOSURE_TIME = 0x829a;
        public const int FNUMBER = 0x829d;
        public const int SHUTTER_SPEED = 0x9201;

        public ExifSummary(Image theImage, string name)
        {
            byte[] buffer = new byte[1];
            byte[] buffer2 = new byte[1];
            byte[] buffer3 = new byte[1];
            try
            {
                PropertyItem[] propertyItems = theImage.PropertyItems;
                foreach (PropertyItem item in propertyItems)
                {
                    switch (item.Id)
                    {
                        case 0x829a:
                            buffer = item.Value;
                            break;

                        case 0x829d:
                            buffer2 = item.Value;
                            break;

                        case 0x9201:
                            buffer3 = item.Value;
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            if (buffer.Length < 5)
            {
                FileInfo fi = new FileInfo(name);
                throw new ParserError("Could not determine Shutter Speed for image " + fi.Name);
            }
            this.FStop = ((float) buffer2[0]) / ((float) buffer2[4]);
            this.ShutterSpeedNumerator = buffer[0];
            this.ShutterSpeedDenominator = buffer[4];
            this.ExposureTime = ((float) buffer[0]) / ((float) buffer[4]);
        }

        public float ExposureTime { get; private set; }

        public float FStop { get; private set; }

        public int ShutterSpeedDenominator { get; private set; }

        public int ShutterSpeedNumerator { get; private set; }
    }
}

