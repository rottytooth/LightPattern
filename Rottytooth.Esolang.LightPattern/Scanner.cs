using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rottytooth.Esolang.LightPattern
{
    public class Scanner
    {
        private List<ImageData> _program = null;

        public List<Symbol> DeltaList { get; private set; }

        public Scanner(List<ImageData> program)
        {
            this._program = program;
            this.DeltaList = new List<Symbol>();
            this.InitialScan();
        }

        public void InitialScan()
        {
            if (this._program == null)
            {
                throw new ParserError("program not loaded yet -- can't scan");
            }
            for (int i = 0; i < this._program.Count; i++)
            {
                if (i != 0)
                {
                    Symbol item = new Symbol();

                    ImageData currentImage = this._program[i];
                    ImageData prevImage = this._program[i - 1];

                    
                    // Shutter: smaller number = shorter exposure = lower number (2 for longer exposure, 1 for same, 0 for shorter)

                    if (currentImage.ExposureTime > prevImage.ExposureTime)
                        item.Shutter = 2;
                    else if (currentImage.ExposureTime == prevImage.ExposureTime)
                        item.Shutter = 1;
                    else
                        item.Shutter = 0;


                    // Aperture: smaller number = more light = higher number (2 for bigger, 1 for same, 0 for smaller)
                    
                    if (currentImage.FStop > prevImage.FStop)
                        item.FStop = 0;
                    else if (currentImage.FStop == prevImage.FStop)
                        item.FStop = 1;
                    else
                        item.FStop = 2;


                    // Color: 2 is for positive direction: r -> g -> b, 0 for opposite, 1 for no change

                    if (currentImage.Color == prevImage.Color)
                        item.Color = 1;
                    else if (currentImage.Color == ColorType.Red)
                        item.Color = (prevImage.Color == ColorType.Green) ? 0 : 2;
                    else if (currentImage.Color == ColorType.Green)
                        item.Color = (prevImage.Color == ColorType.Blue) ? 0 : 2;
                    else
                        item.Color = (prevImage.Color == ColorType.Red) ? 0 : 2;

                    this.DeltaList.Add(item);

                    if (item.Shutter == 2 && item.FStop == 2 && item.Color == 2) // if it's a reset, skip the next image
                        i++;
                }
            }
        }

    }
}

