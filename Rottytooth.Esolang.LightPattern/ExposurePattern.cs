using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Rottytooth.Esolang.LightPattern
{

    public class ExposurePattern
    {
        private List<CodeUnit> _codeList = new List<CodeUnit>();
        private List<ImageData> _imageList;
        private string _path;

        public static  event _Image_Loaded Image_Loaded;

        public static  event _Image_PreLoad Image_PreLoad;

        public ExposurePattern(string path)
        {
            this._path = path;
        }

        public static string DescribeImage(string path)
        {
            StringBuilder builder = new StringBuilder();
            Image theImage = new Bitmap(path);
            ExifSummary summary = new ExifSummary(theImage, path);
            builder.Append("Shutter Speed: ");
            builder.Append(summary.ShutterSpeedNumerator.ToString() + "/");
            builder.AppendLine(summary.ShutterSpeedDenominator.ToString());
            builder.Append("F Stop: ");
            builder.AppendLine(summary.FStop.ToString());
            builder.Append("Exposure time: ");
            builder.AppendLine(summary.ExposureTime.ToString());
            DominantColor averageColor = ImageAnalyzer.GetAverageColor(theImage);
            builder.AppendLine("Dominant Color: " + averageColor.Color.ToString());
            Image_Loaded(builder.ToString());
            return builder.ToString();
        }

        public void Load()
        {
            DirectoryInfo info = new DirectoryInfo(this._path);
            this._imageList = new List<ImageData>();
            foreach (FileInfo info2 in info.GetFiles())
            {
                if ((info2.Extension.ToUpper() == ".JPEG") || (info2.Extension.ToUpper() == ".JPG"))
                {
                    ImageData imageData = null;
                    try
                    {
                        imageData = ReadImage(info2.FullName);
                    }
                    catch (IndeterminateColorException)
                    {
                        Console.WriteLine("Could not determine color for image " + info2.Name);
                        continue;
                    }
                    this._imageList.Add(imageData);
                    DescribeImage(info2.FullName);
                }
            }
        }

        public static ImageData ReadImage(string path)
        {
            ImageData data = new ImageData();
            Image_PreLoad(path);
            Image theImage = new Bitmap(path);
            ExifSummary summary = new ExifSummary(theImage, path);
            data.ExposureTime = summary.ExposureTime;
            data.FStop = summary.FStop;
            ColorType color = ImageAnalyzer.GetAverageColor(theImage).Color;
            data.Color = color;
            return data;
        }

        public List<CodeUnit> CodeList
        {
            get
            {
                return this._codeList;
            }
        }

        public List<ImageData> ImageList
        {
            get
            {
                return this._imageList;
            }
        }

        public delegate void _Image_Loaded(string results);

        public delegate void _Image_PreLoad(string path);
    }
}

