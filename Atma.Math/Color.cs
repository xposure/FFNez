namespace Atma
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///		This class is necessary so we can store the color components as byte
    ///		values in the range [0,255].  
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Color : IComparable
    {
#if DEBUG
        public static explicit operator vec4(Color v) => new vec4(v.R / 255f, v.G / 255f, v.B / 255f, v.A / 255f);
        public static implicit operator Microsoft.Xna.Framework.Vector4(Color v) => new Microsoft.Xna.Framework.Vector4(v.R / 255f, v.G / 255f, v.B / 255f, v.A / 255f);
        public static implicit operator Microsoft.Xna.Framework.Color(Color v) => new Microsoft.Xna.Framework.Color(v.PackedValue);
        public static implicit operator Color(Microsoft.Xna.Framework.Color v) => new Color(v.PackedValue);
        public vec3 ToVector3() => ((vec4)this).xyz;
        public Color(vec3 v)
        {
            _packedValue = 0;
            var scalar = 255;
            A = 255;
            R = (byte)glm.Clamp(v.x * scalar, 0, 255);
            G = (byte)glm.Clamp(v.y * scalar, 0, 255);
            B = (byte)glm.Clamp(v.z * scalar, 0, 255);
        }
        public Color(vec4 v)
        {
            _packedValue = 0;
            var scalar = 255;
            A = (byte)glm.Clamp(v.w * scalar, 0, 255);
            R = (byte)glm.Clamp(v.x * scalar, 0, 255);
            G = (byte)glm.Clamp(v.y * scalar, 0, 255);
            B = (byte)glm.Clamp(v.z * scalar, 0, 255);
        }
#endif

        #region Member variables
        internal uint _packedValue;
        public uint PackedValue => _packedValue;
        #endregion Member variables

        #region Constructors

        ///// <summary>
        /////	Constructor taking RGB values
        ///// </summary>
        ///// <param name="r">Red color component.</param>
        ///// <param name="g">Green color component.</param>
        ///// <param name="b">Blue color component.</param>
        //public Color(float r, float g, float b)
        //    : this(1.0f, r, g, b) { }

        ///// <summary>
        /////		Constructor taking all component values.
        ///// </summary>
        ///// <param name="a">Alpha value.</param>
        ///// <param name="r">Red color component.</param>
        ///// <param name="g">Green color component.</param>
        ///// <param name="b">Blue color component.</param>
        //public Color(float a, float r, float g, float b)
        //{
        //    Debug.Assert(a >= 0f && a <= 1f);
        //    Debug.Assert(r >= 0f && r <= 1f);
        //    Debug.Assert(g >= 0f && g <= 1f);
        //    Debug.Assert(b >= 0f && b <= 1f);

        //    this.a = a;
        //    this.r = r;
        //    this.g = g;
        //    this.b = b;
        //}

        public Color(uint abgr)
        {
            _packedValue = abgr;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The ColorEx instance to copy</param>
        public Color(Color other)
            : this()
        {
            this._packedValue = other._packedValue;
        }

        public Color(Color color, byte alpha)
        {
            _packedValue = 0;

            R = color.R;
            G = color.G;
            B = color.B;
            A = alpha;
        }

        public Color(Color color, float alpha)
        {
            _packedValue = 0;

            R = color.R;
            G = color.G;
            B = color.B;
            A = Convert.ToByte(alpha * 255);
        }

        public Color(float r, float g, float b)
        {
            _packedValue = 0;

            R = Convert.ToByte(r * 255);
            G = Convert.ToByte(g * 255);
            B = Convert.ToByte(b * 255);
            A = 255;
        }

        public Color(byte r, byte g, byte b)
        {
            _packedValue = 0;
            R = r;
            G = g;
            B = b;
            A = (byte)255;
        }


        public Color(byte r, byte g, byte b, byte alpha)
        {
            _packedValue = 0;
            R = r;
            G = g;
            B = b;
            A = alpha;
        }

        public Color(float r, float g, float b, float alpha)
        {
            _packedValue = 0;

            R = (byte)glm.Clamp(r * 255, Byte.MinValue, Byte.MaxValue);
            G = (byte)glm.Clamp(g * 255, Byte.MinValue, Byte.MaxValue);
            B = (byte)glm.Clamp(b * 255, Byte.MinValue, Byte.MaxValue);
            A = (byte)glm.Clamp(alpha * 255, Byte.MinValue, Byte.MaxValue);
        }

        public byte R
        {
            get
            {
                return (byte)this._packedValue;
            }
            set
            {
                this._packedValue = (this._packedValue & 0xffffff00) | value;
            }
        }

        public byte G
        {
            get
            {
                return (byte)(this._packedValue >> 8);
            }
            set
            {
                this._packedValue = (this._packedValue & 0xffff00ff) | ((uint)(value << 8));
            }
        }
        public byte B
        {
            get
            {
                return (byte)(this._packedValue >> 16);
            }
            set
            {
                this._packedValue = (this._packedValue & 0xff00ffff) | ((uint)(value << 16));
            }
        }
        public byte A
        {
            get
            {
                return (byte)(this._packedValue >> 24);
            }
            set
            {
                this._packedValue = (this._packedValue & 0x00ffffff) | ((uint)(value << 24));
            }
        }

        #endregion Constructors

        #region Methods

        //public int ToRGBA()
        //{
        //    int result = 0;

        //    result += ((int)(r * 255.0f)) << 24;
        //    result += ((int)(g * 255.0f)) << 16;
        //    result += ((int)(b * 255.0f)) << 8;
        //    result += ((int)(a * 255.0f));

        //    return result;
        //}

        ///// <summary>
        /////		Converts this color value to packed ABGR format.
        ///// </summary>
        ///// <returns></returns>
        //public int ToABGR()
        //{
        //    int result = 0;

        //    result += ((int)(a * 255.0f)) << 24;
        //    result += ((int)(b * 255.0f)) << 16;
        //    result += ((int)(g * 255.0f)) << 8;
        //    result += ((int)(r * 255.0f));

        //    return result;
        //}

        ///// <summary>
        /////		Converts this color value to packed ARBG format.
        ///// </summary>
        ///// <returns></returns>
        //public int ToARGB()
        //{
        //    int result = 0;

        //    result += ((int)(a * 255.0f)) << 24;
        //    result += ((int)(r * 255.0f)) << 16;
        //    result += ((int)(g * 255.0f)) << 8;
        //    result += ((int)(b * 255.0f));

        //    return result;
        //}

        ///// <summary>
        /////		Populates the color components in a 4 elements array in RGBA order.
        ///// </summary>
        ///// <remarks>
        /////		Primarily used to help in OpenGL.
        ///// </remarks>
        ///// <returns></returns>
        //public void ToArrayRGBA(float[] vals)
        //{
        //    vals[0] = r;
        //    vals[1] = g;
        //    vals[2] = b;
        //    vals[3] = a;
        //}

        ///// <summary>
        ///// Clamps color value to the range [0, 1]
        ///// </summary>
        //public void Saturate()
        //{
        //    r = Utility.Clamp(r, 1.0f, 0.0f);
        //    g = Utility.Clamp(g, 1.0f, 0.0f);
        //    b = Utility.Clamp(b, 1.0f, 0.0f);
        //    a = Utility.Clamp(a, 1.0f, 0.0f);
        //}

        ///// <summary>
        ///// Clamps color value to the range [0, 1] in a copy
        ///// </summary>
        //public Color SaturateCopy()
        //{
        //    Color saturated;
        //    saturated.r = Utility.Clamp(r, 1.0f, 0.0f);
        //    saturated.g = Utility.Clamp(g, 1.0f, 0.0f);
        //    saturated.b = Utility.Clamp(b, 1.0f, 0.0f);
        //    saturated.a = Utility.Clamp(a, 1.0f, 0.0f);

        //    return saturated;
        //}

        #endregion Methods

        #region Operators

        public static bool operator ==(Color left, Color right)
        {
            return left._packedValue == right._packedValue;
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        //public static Color operator *(Color left, Color right)
        //{
        //    Color retVal = left;
        //    retVal.A *= right.A;
        //    retVal.R *= right.R;
        //    retVal.G *= right.G;
        //    retVal.B *= right.B;
        //    return retVal;
        //}

        public static Color operator *(Color left, float scalar)
        {
            Color retVal = left;
            retVal.A = (byte)glm.Clamp(retVal.A * scalar, 0, 255);
            retVal.R = (byte)glm.Clamp(retVal.R * scalar, 0, 255);
            retVal.G = (byte)glm.Clamp(retVal.G * scalar, 0, 255);
            retVal.B = (byte)glm.Clamp(retVal.B * scalar, 0, 255);
            return retVal;
        }

        //public static Color operator /(Color left, Color right)
        //{
        //    Color retVal = left;
        //    retVal.A /= right.A;
        //    retVal.R /= right.R;
        //    retVal.G /= right.G;
        //    retVal.B /= right.B;
        //    return retVal;
        //}

        //public static Color operator /(Color left, float scalar)
        //{
        //    Color retVal = left;
        //    retVal.A /= scalar;
        //    retVal.R /= scalar;
        //    retVal.G /= scalar;
        //    retVal.B /= scalar;
        //    return retVal;
        //}

        public static Color operator -(Color left, Color right)
        {
            Color retVal = left;
            retVal.A -= right.A;
            retVal.R -= right.R;
            retVal.G -= right.G;
            retVal.B -= right.B;
            return retVal;
        }

        public static Color operator +(Color left, Color right)
        {
            Color retVal = left;
            retVal.A += right.A;
            retVal.R += right.R;
            retVal.G += right.G;
            retVal.B += right.B;
            return retVal;
        }

        #endregion Operators

        #region Object overloads

        /// <summary>
        ///    Override GetHashCode.
        /// </summary>
        /// <remarks>
        ///    Done mainly to quash warnings, no float need for it.
        /// </remarks>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)_packedValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is Color)
            {
                return this == (Color)obj;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})", R, G, B, A);
        }

        #endregion Object overloads

        #region IComparable Members

        /// <summary>
        ///    Used to compare 2 ColorEx objects for equality.
        /// </summary>
        /// <param name="obj">An instance of a ColorEx object to compare to this instance.</param>
        /// <returns>0 if they are equal, 1 if they are not.</returns>
        public int CompareTo(object obj)
        {
            Color other = (Color)obj;

            if (this._packedValue == other._packedValue)
            {
                return 0;
            }

            return 1;
        }

        #endregion IComparable Members

        #region ICloneable Implementation

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public Color Clone()
        {
            Color clone;
            clone._packedValue = this._packedValue;
            return clone;
        }

        #endregion ICloneable Implementation

        public static Color FromRGB(int color)
        {
            uint c = (uint)(0x00ffffff & color);
            return new Color(((c >> 16) | ((c & 0xff) << 16) | (0xff00 & c)) | 0xff000000);
        }

        //public static Color FromRGBA(int color)
        //{
        //    uint c = (uint)(0x00ffffff & color);
        //    return new Color(((c >> 16) | ((c & 0xff) << 16) | (0xff00 & c)) | 0xff000000);
        //}


        public static readonly Color TransparentBlack = new Color(0);
        public static readonly Color Transparent = new Color(0);
        public static readonly Color AliceBlue = new Color(0xfffff8f0);
        public static readonly Color AntiqueWhite = new Color(0xffd7ebfa);
        public static readonly Color Aqua = new Color(0xffffff00);
        public static readonly Color Aquamarine = new Color(0xffd4ff7f);
        public static readonly Color Azure = new Color(0xfffffff0);
        public static readonly Color Beige = new Color(0xffdcf5f5);
        public static readonly Color Bisque = new Color(0xffc4e4ff);
        public static readonly Color Black = new Color(0xff000000);
        public static readonly Color BlanchedAlmond = new Color(0xffcdebff);
        public static readonly Color Blue = new Color(0xffff0000);
        public static readonly Color BlueViolet = new Color(0xffe22b8a);
        public static readonly Color Brown = new Color(0xff2a2aa5);
        public static readonly Color BurlyWood = new Color(0xff87b8de);
        public static readonly Color CadetBlue = new Color(0xffa09e5f);
        public static readonly Color Chartreuse = new Color(0xff00ff7f);
        public static readonly Color Chocolate = new Color(0xff1e69d2);
        public static readonly Color Coral = new Color(0xff507fff);
        public static readonly Color CornflowerBlue = new Color(0xffed9564);
        public static readonly Color Cornsilk = new Color(0xffdcf8ff);
        public static readonly Color Crimson = new Color(0xff3c14dc);
        public static readonly Color Cyan = new Color(0xffffff00);
        public static readonly Color DarkBlue = new Color(0xff8b0000);
        public static readonly Color DarkCyan = new Color(0xff8b8b00);
        public static readonly Color DarkGoldenrod = new Color(0xff0b86b8);
        public static readonly Color DarkGray = new Color(0xffa9a9a9);
        public static readonly Color DarkGreen = new Color(0xff006400);
        public static readonly Color DarkKhaki = new Color(0xff6bb7bd);
        public static readonly Color DarkMagenta = new Color(0xff8b008b);
        public static readonly Color DarkOliveGreen = new Color(0xff2f6b55);
        public static readonly Color DarkOrange = new Color(0xff008cff);
        public static readonly Color DarkOrchid = new Color(0xffcc3299);
        public static readonly Color DarkRed = new Color(0xff00008b);
        public static readonly Color DarkSalmon = new Color(0xff7a96e9);
        public static readonly Color DarkSeaGreen = new Color(0xff8bbc8f);
        public static readonly Color DarkSlateBlue = new Color(0xff8b3d48);
        public static readonly Color DarkSlateGray = new Color(0xff4f4f2f);
        public static readonly Color DarkTurquoise = new Color(0xffd1ce00);
        public static readonly Color DarkViolet = new Color(0xffd30094);
        public static readonly Color DeepPink = new Color(0xff9314ff);
        public static readonly Color DeepSkyBlue = new Color(0xffffbf00);
        public static readonly Color DimGray = new Color(0xff696969);
        public static readonly Color DodgerBlue = new Color(0xffff901e);
        public static readonly Color Firebrick = new Color(0xff2222b2);
        public static readonly Color FloralWhite = new Color(0xfff0faff);
        public static readonly Color ForestGreen = new Color(0xff228b22);
        public static readonly Color Fuchsia = new Color(0xffff00ff);
        public static readonly Color Gainsboro = new Color(0xffdcdcdc);
        public static readonly Color GhostWhite = new Color(0xfffff8f8);
        public static readonly Color Gold = new Color(0xff00d7ff);
        public static readonly Color Goldenrod = new Color(0xff20a5da);
        public static readonly Color Gray = new Color(0xff808080);
        public static readonly Color Green = new Color(0xff008000);
        public static readonly Color GreenYellow = new Color(0xff2fffad);
        public static readonly Color Honeydew = new Color(0xfff0fff0);
        public static readonly Color HotPink = new Color(0xffb469ff);
        public static readonly Color IndianRed = new Color(0xff5c5ccd);
        public static readonly Color Indigo = new Color(0xff82004b);
        public static readonly Color Ivory = new Color(0xfff0ffff);
        public static readonly Color Khaki = new Color(0xff8ce6f0);
        public static readonly Color Lavender = new Color(0xfffae6e6);
        public static readonly Color LavenderBlush = new Color(0xfff5f0ff);
        public static readonly Color LawnGreen = new Color(0xff00fc7c);
        public static readonly Color LemonChiffon = new Color(0xffcdfaff);
        public static readonly Color LightBlue = new Color(0xffe6d8ad);
        public static readonly Color LightCoral = new Color(0xff8080f0);
        public static readonly Color LightCyan = new Color(0xffffffe0);
        public static readonly Color LightGoldenrodYellow = new Color(0xffd2fafa);
        public static readonly Color LightGray = new Color(0xffd3d3d3);
        public static readonly Color LightGreen = new Color(0xff90ee90);
        public static readonly Color LightPink = new Color(0xffc1b6ff);
        public static readonly Color LightSalmon = new Color(0xff7aa0ff);
        public static readonly Color LightSeaGreen = new Color(0xffaab220);
        public static readonly Color LightSkyBlue = new Color(0xffface87);
        public static readonly Color LightSlateGray = new Color(0xff998877);
        public static readonly Color LightSteelBlue = new Color(0xffdec4b0);
        public static readonly Color LightYellow = new Color(0xffe0ffff);
        public static readonly Color Lime = new Color(0xff00ff00);
        public static readonly Color LimeGreen = new Color(0xff32cd32);
        public static readonly Color Linen = new Color(0xffe6f0fa);
        public static readonly Color Magenta = new Color(0xffff00ff);
        public static readonly Color Maroon = new Color(0xff000080);
        public static readonly Color MediumAquamarine = new Color(0xffaacd66);
        public static readonly Color MediumBlue = new Color(0xffcd0000);
        public static readonly Color MediumOrchid = new Color(0xffd355ba);
        public static readonly Color MediumPurple = new Color(0xffdb7093);
        public static readonly Color MediumSeaGreen = new Color(0xff71b33c);
        public static readonly Color MediumSlateBlue = new Color(0xffee687b);
        public static readonly Color MediumSpringGreen = new Color(0xff9afa00);
        public static readonly Color MediumTurquoise = new Color(0xffccd148);
        public static readonly Color MediumVioletRed = new Color(0xff8515c7);
        public static readonly Color MidnightBlue = new Color(0xff701919);
        public static readonly Color MintCream = new Color(0xfffafff5);
        public static readonly Color MistyRose = new Color(0xffe1e4ff);
        public static readonly Color Moccasin = new Color(0xffb5e4ff);
        public static readonly Color MonoGameOrange = new Color(0xff003ce7);
        public static readonly Color NavajoWhite = new Color(0xffaddeff);
        public static readonly Color Navy = new Color(0xff800000);
        public static readonly Color OldLace = new Color(0xffe6f5fd);
        public static readonly Color Olive = new Color(0xff008080);
        public static readonly Color OliveDrab = new Color(0xff238e6b);
        public static readonly Color Orange = new Color(0xff00a5ff);
        public static readonly Color OrangeRed = new Color(0xff0045ff);
        public static readonly Color Orchid = new Color(0xffd670da);
        public static readonly Color PaleGoldenrod = new Color(0xffaae8ee);
        public static readonly Color PaleGreen = new Color(0xff98fb98);
        public static readonly Color PaleTurquoise = new Color(0xffeeeeaf);
        public static readonly Color PaleVioletRed = new Color(0xff9370db);
        public static readonly Color PapayaWhip = new Color(0xffd5efff);
        public static readonly Color PeachPuff = new Color(0xffb9daff);
        public static readonly Color Peru = new Color(0xff3f85cd);
        public static readonly Color Pink = new Color(0xffcbc0ff);
        public static readonly Color Plum = new Color(0xffdda0dd);
        public static readonly Color PowderBlue = new Color(0xffe6e0b0);
        public static readonly Color Purple = new Color(0xff800080);
        public static readonly Color Red = new Color(0xff0000ff);
        public static readonly Color RosyBrown = new Color(0xff8f8fbc);
        public static readonly Color RoyalBlue = new Color(0xffe16941);
        public static readonly Color SaddleBrown = new Color(0xff13458b);
        public static readonly Color Salmon = new Color(0xff7280fa);
        public static readonly Color SandyBrown = new Color(0xff60a4f4);
        public static readonly Color SeaGreen = new Color(0xff578b2e);
        public static readonly Color SeaShell = new Color(0xffeef5ff);
        public static readonly Color Sienna = new Color(0xff2d52a0);
        public static readonly Color Silver  = new Color(0xffc0c0c0);
        public static readonly Color SkyBlue  = new Color(0xffebce87);
        public static readonly Color SlateBlue = new Color(0xffcd5a6a);
        public static readonly Color SlateGray = new Color(0xff908070);
        public static readonly Color Snow = new Color(0xfffafaff);
        public static readonly Color SpringGreen = new Color(0xff7fff00);
        public static readonly Color SteelBlue = new Color(0xffb48246);
        public static readonly Color Tan = new Color(0xff8cb4d2);
        public static readonly Color Teal = new Color(0xff808000);
        public static readonly Color Thistle = new Color(0xffd8bfd8);
        public static readonly Color Tomato = new Color(0xff4763ff);
        public static readonly Color Turquoise = new Color(0xffd0e040);
        public static readonly Color Violet = new Color(0xffee82ee);
        public static readonly Color Wheat = new Color(0xffb3def5);
        public static readonly Color White = new Color(uint.MaxValue);
        public static readonly Color WhiteSmoke = new Color(0xfff5f5f5);
        public static readonly Color Yellow = new Color(0xff00ffff);
        public static readonly Color YellowGreen = new Color(0xff32cd9a);
    }
}
