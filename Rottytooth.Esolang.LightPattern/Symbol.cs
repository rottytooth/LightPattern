using System;

namespace Rottytooth.Esolang.LightPattern
{
    public class Symbol
    {
        private int _color;
        private int _fstop;
        private int _shutter;

        public Symbol()
        {
        }

        public Symbol(int shutter, int fstop, int color)
        {
            this.Shutter = shutter;
            this.FStop = fstop;
            this.Color = color;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Symbol symbol = (Symbol) obj;
            return (((this.Shutter == symbol.Shutter) && (this.FStop == symbol.FStop)) && (this.Color == symbol.Color));
        }

        public override int GetHashCode()
        {
            return (((this.Shutter * 100) + (this.FStop * 10)) + this.Color);
        }

        public static bool operator ==(Symbol x, Symbol y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Symbol x, Symbol y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return (this.Shutter.ToString() + this.FStop.ToString() + this.Color.ToString());
        }

        public int Color
        {
            get
            {
                return this._color;
            }
            set
            {
                if (value > 2)
                {
                    throw new ParserError("Color delta value can not be greater than 2");
                }
                this._color = value;
            }
        }

        public int FStop
        {
            get
            {
                return this._fstop;
            }
            set
            {
                if (value > 2)
                {
                    throw new ParserError("FStop delta value can not be greater than 2");
                }
                this._fstop = value;
            }
        }

        public int Shutter
        {
            get
            {
                return this._shutter;
            }
            set
            {
                if (value > 2)
                {
                    throw new ParserError("Shutter delta value can not be greater than 2");
                }
                this._shutter = value;
            }
        }
    }
}

