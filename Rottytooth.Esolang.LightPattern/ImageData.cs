using System;
using System.Runtime.CompilerServices;


namespace Rottytooth.Esolang.LightPattern
{
    public class ImageData
    {
        public ImageData()
        {
        }

        public ImageData(double exposureTime, double fStop, ColorType color)
        {
            this.Color = color;
            this.ExposureTime = exposureTime;
            this.FStop = fStop;
        }

        public ColorType Color { get; set; }

        public double ExposureTime { get; set; }

        public double FStop { get; set; }
    }
}

