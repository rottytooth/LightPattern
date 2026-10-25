using System;
using System.Runtime.CompilerServices;

namespace Rottytooth.Esolang.LightPattern
{
    public class DominantColor
    {
        public ColorType Color { get; private set; }

        public long Red { get; private set; }
        public long Green { get; private set; }
        public long Blue { get; private set; }

        public double Ratio
        {
            get
            {
                long activeColor = 0L;
                long colorSum = this.Red + this.Green + this.Blue;
                
                switch(this.Color)
                {
                    case ColorType.Red:
                        activeColor = this.Red;
                        break;
                    case ColorType.Green:
                        activeColor = this.Green;
                        break;
                    case ColorType.Blue:
                        activeColor = this.Blue;
                        break;
                }

                return (((double)activeColor) / ((double)colorSum));
            }
        }


        public DominantColor(long red, long green, long blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;

            if ((red > green) && (red > blue))
            {
                this.Color = ColorType.Red;
            }
            else if ((green > red) && (green > blue))
            {
                this.Color = ColorType.Green;
            }
            else
            {
                if ((blue <= red) || (blue <= green))
                {
                    throw new IndeterminateColorException("Could not determine color");
                }
                this.Color = ColorType.Blue;
            }
        }
    }
}

