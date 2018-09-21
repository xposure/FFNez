#region LGPL License

/*
Axiom Graphics Engine Library
Copyright © 2003-2011 Axiom Project Team

The overall design, and a majority of the core engine and rendering code
contained within this library is a derivative of the open source Object Oriented
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.
Many thanks to the OGRE team for maintaining such a high quality project.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/

#endregion LGPL License

#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ColorEx.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#endregion Namespace Declarations

namespace Atma
{
    /// <summary>
    ///		This class is necessary so we can store the color components as floating
    ///		point values in the range [0,1].  It serves as an intermediary to System.Drawing.Color, which
    ///		stores them as byte values.  This doesn't allow for slow color component
    ///		interpolation, because with the values always being cast back to a byte would lose
    ///		any small interpolated values (i.e. 223 - .25 as a byte is 223).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Colorf : IComparable
    {
        #region Member variables

        /// <summary>
        ///		Alpha value [0,1].
        /// </summary>
        public float a;

        /// <summary>
        ///		Red color component [0,1].
        /// </summary>
        public float r;

        /// <summary>
        ///		Green color component [0,1].
        /// </summary>
        public float g;

        /// <summary>
        ///		Blue color component [0,1].
        /// </summary>
        public float b;

        #endregion Member variables

        #region Constructors

        /// <summary>
        ///	Constructor taking RGB values
        /// </summary>
        /// <param name="r">Red color component.</param>
        /// <param name="g">Green color component.</param>
        /// <param name="b">Blue color component.</param>
        public Colorf(float r, float g, float b)
            : this(1.0f, r, g, b) { }

        /// <summary>
        ///		Constructor taking all component values.
        /// </summary>
        /// <param name="a">Alpha value.</param>
        /// <param name="r">Red color component.</param>
        /// <param name="g">Green color component.</param>
        /// <param name="b">Blue color component.</param>
        public Colorf(float a, float r, float g, float b)
        {
            Debug.Assert(a >= 0f && a <= 1f);
            Debug.Assert(r >= 0f && r <= 1f);
            Debug.Assert(g >= 0f && g <= 1f);
            Debug.Assert(b >= 0f && b <= 1f);

            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The ColorEx instance to copy</param>
        public Colorf(Colorf other)
            : this()
        {
            this.a = other.a;
            this.r = other.r;
            this.g = other.g;
            this.b = other.b;
        }

        public Colorf(vec4 color)
        {
            this.a = color.w;
            this.r = color.x;
            this.g = color.y;
            this.b = color.z;
        }

        #endregion Constructors

        public float R { get { return r; } set { r = value; } }
        public float G { get { return g; } set { g = value; } }
        public float B { get { return b; } set { b = value; } }
        public float A { get { return a; } set { a = value; } }

        #region Methods

        public int ToRGBA()
        {
            int result = 0;

            result += ((int)(r * 255.0f)) << 24;
            result += ((int)(g * 255.0f)) << 16;
            result += ((int)(b * 255.0f)) << 8;
            result += ((int)(a * 255.0f));

            return result;
        }

        /// <summary>
        ///		Converts this color value to packed ABGR format.
        /// </summary>
        /// <returns></returns>
        public int ToABGR()
        {
            int result = 0;

            result += ((int)(a * 255.0f)) << 24;
            result += ((int)(b * 255.0f)) << 16;
            result += ((int)(g * 255.0f)) << 8;
            result += ((int)(r * 255.0f));

            return result;
        }

        /// <summary>
        ///		Converts this color value to packed ARBG format.
        /// </summary>
        /// <returns></returns>
        public int ToARGB()
        {
            int result = 0;

            result += ((int)(a * 255.0f)) << 24;
            result += ((int)(r * 255.0f)) << 16;
            result += ((int)(g * 255.0f)) << 8;
            result += ((int)(b * 255.0f));

            return result;
        }

        /// <summary>
        ///		Populates the color components in a 4 elements array in RGBA order.
        /// </summary>
        /// <remarks>
        ///		Primarily used to help in OpenGL.
        /// </remarks>
        /// <returns></returns>
        public void ToArrayRGBA(float[] vals)
        {
            vals[0] = r;
            vals[1] = g;
            vals[2] = b;
            vals[3] = a;
        }

        /// <summary>
        /// Clamps color value to the range [0, 1]
        /// </summary>
        public void Saturate()
        {
            r = glm.Clamp(r, 0, 1);
            g = glm.Clamp(g, 0, 1);
            b = glm.Clamp(b, 0, 1);
            a = glm.Clamp(a, 0, 1);
        }

        /// <summary>
        /// Clamps color value to the range [0, 1] in a copy
        /// </summary>
        public Colorf SaturateCopy()
        {
            Colorf saturated;
            saturated.r = glm.Clamp(r, 0, 1);
            saturated.g = glm.Clamp(g, 0, 1);
            saturated.b = glm.Clamp(b, 0, 1);
            saturated.a = glm.Clamp(a, 0, 1);

            return saturated;
        }

        public vec4 ToVector4()
        {
            return new vec4(r, g, b, a);
        }

        #endregion Methods

        #region Operators

        public static bool operator ==(Colorf left, Colorf right)
        {
            return left.a == right.a &&
                   left.b == right.b &&
                   left.g == right.g &&
                   left.r == right.r;
        }

        public static bool operator !=(Colorf left, Colorf right)
        {
            return !(left == right);
        }

        public static Colorf operator *(Colorf left, Colorf right)
        {
            Colorf retVal = left;
            retVal.a *= right.a;
            retVal.r *= right.r;
            retVal.g *= right.g;
            retVal.b *= right.b;
            return retVal;
        }

        public static Colorf operator *(Colorf left, float scalar)
        {
            Colorf retVal = left;
            retVal.a *= scalar;
            retVal.r *= scalar;
            retVal.g *= scalar;
            retVal.b *= scalar;
            return retVal;
        }

        public static Colorf operator /(Colorf left, Colorf right)
        {
            Colorf retVal = left;
            retVal.a /= right.a;
            retVal.r /= right.r;
            retVal.g /= right.g;
            retVal.b /= right.b;
            return retVal;
        }

        public static Colorf operator /(Colorf left, float scalar)
        {
            Colorf retVal = left;
            retVal.a /= scalar;
            retVal.r /= scalar;
            retVal.g /= scalar;
            retVal.b /= scalar;
            return retVal;
        }

        public static Colorf operator -(Colorf left, Colorf right)
        {
            Colorf retVal = left;
            retVal.a -= right.a;
            retVal.r -= right.r;
            retVal.g -= right.g;
            retVal.b -= right.b;
            return retVal;
        }

        public static Colorf operator +(Colorf left, Colorf right)
        {
            Colorf retVal = left;
            retVal.a += right.a;
            retVal.r += right.r;
            retVal.g += right.g;
            retVal.b += right.b;
            return retVal;
        }

        public static implicit operator vec3(Colorf c)
        {
            return new vec3(c.r, c.g, c.b);
        }

        public static implicit operator Colorf(vec3 v)
        {
            return new Colorf(v.x, v.y, v.z, 1f);
        }

        public static implicit operator vec4(Colorf c)
        {
            return new vec4(c.r, c.g, c.b, c.a);
        }

        public static implicit operator Colorf(vec4 v)
        {
            return new Colorf(v.x, v.y, v.z, v.w);
        }


        #endregion Operators

        #region Static color properties

        /// <summary>
        ///		The color Transparent.
        /// </summary>
        public static Colorf Transparent
        {
            get
            {
                Colorf retVal;
                retVal.a = 0f;
                retVal.r = 1f;
                retVal.g = 1f;
                retVal.b = 1f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color AliceBlue.
        /// </summary>
        public static Colorf AliceBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9411765f;
                retVal.g = 0.972549f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color AntiqueWhite.
        /// </summary>
        public static Colorf AntiqueWhite
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9803922f;
                retVal.g = 0.9215686f;
                retVal.b = 0.8431373f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Aqua.
        /// </summary>
        public static Colorf Aqua
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 1.0f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Aquamarine.
        /// </summary>
        public static Colorf Aquamarine
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4980392f;
                retVal.g = 1.0f;
                retVal.b = 0.8313726f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Azure.
        /// </summary>
        public static Colorf Azure
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9411765f;
                retVal.g = 1.0f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Beige.
        /// </summary>
        public static Colorf Beige
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9607843f;
                retVal.g = 0.9607843f;
                retVal.b = 0.8627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Bisque.
        /// </summary>
        public static Colorf Bisque
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.8941177f;
                retVal.b = 0.7686275f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Black.
        /// </summary>
        public static Colorf Black
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.0f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color BlanchedAlmond.
        /// </summary>
        public static Colorf BlanchedAlmond
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.9215686f;
                retVal.b = 0.8039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Blue.
        /// </summary>
        public static Colorf Blue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.0f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color BlueViolet.
        /// </summary>
        public static Colorf BlueViolet
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5411765f;
                retVal.g = 0.1686275f;
                retVal.b = 0.8862745f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Brown.
        /// </summary>
        public static Colorf Brown
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6470588f;
                retVal.g = 0.1647059f;
                retVal.b = 0.1647059f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color BurlyWood.
        /// </summary>
        public static Colorf BurlyWood
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8705882f;
                retVal.g = 0.7215686f;
                retVal.b = 0.5294118f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color CadetBlue.
        /// </summary>
        public static Colorf CadetBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.372549f;
                retVal.g = 0.6196079f;
                retVal.b = 0.627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Chartreuse.
        /// </summary>
        public static Colorf Chartreuse
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4980392f;
                retVal.g = 1.0f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Chocolate.
        /// </summary>
        public static Colorf Chocolate
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8235294f;
                retVal.g = 0.4117647f;
                retVal.b = 0.1176471f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Coral.
        /// </summary>
        public static Colorf Coral
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.4980392f;
                retVal.b = 0.3137255f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color CornflowerBlue.
        /// </summary>
        public static Colorf CornflowerBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.3921569f;
                retVal.g = 0.5843138f;
                retVal.b = 0.9294118f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Cornsilk.
        /// </summary>
        public static Colorf Cornsilk
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.972549f;
                retVal.b = 0.8627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Crimson.
        /// </summary>
        public static Colorf Crimson
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8627451f;
                retVal.g = 0.07843138f;
                retVal.b = 0.2352941f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Cyan.
        /// </summary>
        public static Colorf Cyan
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 1.0f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkBlue.
        /// </summary>
        public static Colorf DarkBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.0f;
                retVal.b = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkCyan.
        /// </summary>
        public static Colorf DarkCyan
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.5450981f;
                retVal.b = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkGoldenrod.
        /// </summary>
        public static Colorf DarkGoldenrod
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.7215686f;
                retVal.g = 0.5254902f;
                retVal.b = 0.04313726f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkGray.
        /// </summary>
        public static Colorf DarkGray
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6627451f;
                retVal.g = 0.6627451f;
                retVal.b = 0.6627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkGreen.
        /// </summary>
        public static Colorf DarkGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.3921569f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkKhaki.
        /// </summary>
        public static Colorf DarkKhaki
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.7411765f;
                retVal.g = 0.7176471f;
                retVal.b = 0.4196078f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkMagenta.
        /// </summary>
        public static Colorf DarkMagenta
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5450981f;
                retVal.g = 0.0f;
                retVal.b = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkOliveGreen.
        /// </summary>
        public static Colorf DarkOliveGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.3333333f;
                retVal.g = 0.4196078f;
                retVal.b = 0.1843137f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkOrange.
        /// </summary>
        public static Colorf DarkOrange
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.5490196f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkOrchid.
        /// </summary>
        public static Colorf DarkOrchid
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6f;
                retVal.g = 0.1960784f;
                retVal.b = 0.8f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkRed.
        /// </summary>
        public static Colorf DarkRed
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5450981f;
                retVal.g = 0.0f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkSalmon.
        /// </summary>
        public static Colorf DarkSalmon
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9137255f;
                retVal.g = 0.5882353f;
                retVal.b = 0.4784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkSeaGreen.
        /// </summary>
        public static Colorf DarkSeaGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5607843f;
                retVal.g = 0.7372549f;
                retVal.b = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkSlateBlue.
        /// </summary>
        public static Colorf DarkSlateBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.282353f;
                retVal.g = 0.2392157f;
                retVal.b = 0.5450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkSlateGray.
        /// </summary>
        public static Colorf DarkSlateGray
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.1843137f;
                retVal.g = 0.3098039f;
                retVal.b = 0.3098039f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkTurquoise.
        /// </summary>
        public static Colorf DarkTurquoise
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.8078431f;
                retVal.b = 0.8196079f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DarkViolet.
        /// </summary>
        public static Colorf DarkViolet
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5803922f;
                retVal.g = 0.0f;
                retVal.b = 0.827451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DeepPink.
        /// </summary>
        public static Colorf DeepPink
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.07843138f;
                retVal.b = 0.5764706f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DeepSkyBlue.
        /// </summary>
        public static Colorf DeepSkyBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.7490196f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DimGray.
        /// </summary>
        public static Colorf DimGray
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4117647f;
                retVal.g = 0.4117647f;
                retVal.b = 0.4117647f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color DodgerBlue.
        /// </summary>
        public static Colorf DodgerBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.1176471f;
                retVal.g = 0.5647059f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Firebrick.
        /// </summary>
        public static Colorf Firebrick
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6980392f;
                retVal.g = 0.1333333f;
                retVal.b = 0.1333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color FloralWhite.
        /// </summary>
        public static Colorf FloralWhite
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.9803922f;
                retVal.b = 0.9411765f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color ForestGreen.
        /// </summary>
        public static Colorf ForestGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.1333333f;
                retVal.g = 0.5450981f;
                retVal.b = 0.1333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Fuchsia.
        /// </summary>
        public static Colorf Fuchsia
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.0f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Gainsboro.
        /// </summary>
        public static Colorf Gainsboro
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8627451f;
                retVal.g = 0.8627451f;
                retVal.b = 0.8627451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color GhostWhite.
        /// </summary>
        public static Colorf GhostWhite
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.972549f;
                retVal.g = 0.972549f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Gold.
        /// </summary>
        public static Colorf Gold
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.8431373f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Goldenrod.
        /// </summary>
        public static Colorf Goldenrod
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.854902f;
                retVal.g = 0.6470588f;
                retVal.b = 0.1254902f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Gray.
        /// </summary>
        public static Colorf Gray
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5019608f;
                retVal.g = 0.5019608f;
                retVal.b = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Green.
        /// </summary>
        public static Colorf Green
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.5019608f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color GreenYellow.
        /// </summary>
        public static Colorf GreenYellow
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6784314f;
                retVal.g = 1.0f;
                retVal.b = 0.1843137f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Honeydew.
        /// </summary>
        public static Colorf Honeydew
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9411765f;
                retVal.g = 1.0f;
                retVal.b = 0.9411765f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color HotPink.
        /// </summary>
        public static Colorf HotPink
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.4117647f;
                retVal.b = 0.7058824f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color IndianRed.
        /// </summary>
        public static Colorf IndianRed
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8039216f;
                retVal.g = 0.3607843f;
                retVal.b = 0.3607843f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Indigo.
        /// </summary>
        public static Colorf Indigo
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.2941177f;
                retVal.g = 0.0f;
                retVal.b = 0.509804f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Ivory.
        /// </summary>
        public static Colorf Ivory
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 1.0f;
                retVal.b = 0.9411765f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Khaki.
        /// </summary>
        public static Colorf Khaki
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9411765f;
                retVal.g = 0.9019608f;
                retVal.b = 0.5490196f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Lavender.
        /// </summary>
        public static Colorf Lavender
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9019608f;
                retVal.g = 0.9019608f;
                retVal.b = 0.9803922f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LavenderBlush.
        /// </summary>
        public static Colorf LavenderBlush
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.9411765f;
                retVal.b = 0.9607843f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LawnGreen.
        /// </summary>
        public static Colorf LawnGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4862745f;
                retVal.g = 0.9882353f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LemonChiffon.
        /// </summary>
        public static Colorf LemonChiffon
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.9803922f;
                retVal.b = 0.8039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightBlue.
        /// </summary>
        public static Colorf LightBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6784314f;
                retVal.g = 0.8470588f;
                retVal.b = 0.9019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightCoral.
        /// </summary>
        public static Colorf LightCoral
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9411765f;
                retVal.g = 0.5019608f;
                retVal.b = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightCyan.
        /// </summary>
        public static Colorf LightCyan
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8784314f;
                retVal.g = 1.0f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightGoldenrodYellow.
        /// </summary>
        public static Colorf LightGoldenrodYellow
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9803922f;
                retVal.g = 0.9803922f;
                retVal.b = 0.8235294f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightGreen.
        /// </summary>
        public static Colorf LightGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5647059f;
                retVal.g = 0.9333333f;
                retVal.b = 0.5647059f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightGray.
        /// </summary>
        public static Colorf LightGray
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.827451f;
                retVal.g = 0.827451f;
                retVal.b = 0.827451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightPink.
        /// </summary>
        public static Colorf LightPink
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.7137255f;
                retVal.b = 0.7568628f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSalmon.
        /// </summary>
        public static Colorf LightSalmon
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.627451f;
                retVal.b = 0.4784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSeaGreen.
        /// </summary>
        public static Colorf LightSeaGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.1254902f;
                retVal.g = 0.6980392f;
                retVal.b = 0.6666667f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSkyBlue.
        /// </summary>
        public static Colorf LightSkyBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5294118f;
                retVal.g = 0.8078431f;
                retVal.b = 0.9803922f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSlateGray.
        /// </summary>
        public static Colorf LightSlateGray
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4666667f;
                retVal.g = 0.5333334f;
                retVal.b = 0.6f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightSteelBlue.
        /// </summary>
        public static Colorf LightSteelBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6901961f;
                retVal.g = 0.7686275f;
                retVal.b = 0.8705882f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LightYellow.
        /// </summary>
        public static Colorf LightYellow
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 1.0f;
                retVal.b = 0.8784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Lime.
        /// </summary>
        public static Colorf Lime
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 1.0f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color LimeGreen.
        /// </summary>
        public static Colorf LimeGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.1960784f;
                retVal.g = 0.8039216f;
                retVal.b = 0.1960784f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Linen.
        /// </summary>
        public static Colorf Linen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9803922f;
                retVal.g = 0.9411765f;
                retVal.b = 0.9019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Magenta.
        /// </summary>
        public static Colorf Magenta
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.0f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Maroon.
        /// </summary>
        public static Colorf Maroon
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5019608f;
                retVal.g = 0.0f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumAquamarine.
        /// </summary>
        public static Colorf MediumAquamarine
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4f;
                retVal.g = 0.8039216f;
                retVal.b = 0.6666667f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumBlue.
        /// </summary>
        public static Colorf MediumBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.0f;
                retVal.b = 0.8039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumOrchid.
        /// </summary>
        public static Colorf MediumOrchid
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.7294118f;
                retVal.g = 0.3333333f;
                retVal.b = 0.827451f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumPurple.
        /// </summary>
        public static Colorf MediumPurple
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5764706f;
                retVal.g = 0.4392157f;
                retVal.b = 0.8588235f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumSeaGreen.
        /// </summary>
        public static Colorf MediumSeaGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.2352941f;
                retVal.g = 0.7019608f;
                retVal.b = 0.4431373f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumSlateBlue.
        /// </summary>
        public static Colorf MediumSlateBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4823529f;
                retVal.g = 0.4078431f;
                retVal.b = 0.9333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumSpringGreen.
        /// </summary>
        public static Colorf MediumSpringGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.9803922f;
                retVal.b = 0.6039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumTurquoise.
        /// </summary>
        public static Colorf MediumTurquoise
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.282353f;
                retVal.g = 0.8196079f;
                retVal.b = 0.8f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MediumVioletRed.
        /// </summary>
        public static Colorf MediumVioletRed
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.7803922f;
                retVal.g = 0.08235294f;
                retVal.b = 0.5215687f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MidnightBlue.
        /// </summary>
        public static Colorf MidnightBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.09803922f;
                retVal.g = 0.09803922f;
                retVal.b = 0.4392157f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MintCream.
        /// </summary>
        public static Colorf MintCream
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9607843f;
                retVal.g = 1.0f;
                retVal.b = 0.9803922f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color MistyRose.
        /// </summary>
        public static Colorf MistyRose
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.8941177f;
                retVal.b = 0.8823529f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Moccasin.
        /// </summary>
        public static Colorf Moccasin
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.8941177f;
                retVal.b = 0.7098039f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color NavajoWhite.
        /// </summary>
        public static Colorf NavajoWhite
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.8705882f;
                retVal.b = 0.6784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Navy.
        /// </summary>
        public static Colorf Navy
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.0f;
                retVal.b = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color OldLace.
        /// </summary>
        public static Colorf OldLace
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9921569f;
                retVal.g = 0.9607843f;
                retVal.b = 0.9019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Olive.
        /// </summary>
        public static Colorf Olive
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5019608f;
                retVal.g = 0.5019608f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color OliveDrab.
        /// </summary>
        public static Colorf OliveDrab
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4196078f;
                retVal.g = 0.5568628f;
                retVal.b = 0.1372549f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Orange.
        /// </summary>
        public static Colorf Orange
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.6470588f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color OrangeRed.
        /// </summary>
        public static Colorf OrangeRed
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.2705882f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Orchid.
        /// </summary>
        public static Colorf Orchid
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.854902f;
                retVal.g = 0.4392157f;
                retVal.b = 0.8392157f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PaleGoldenrod.
        /// </summary>
        public static Colorf PaleGoldenrod
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9333333f;
                retVal.g = 0.9098039f;
                retVal.b = 0.6666667f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PaleGreen.
        /// </summary>
        public static Colorf PaleGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5960785f;
                retVal.g = 0.9843137f;
                retVal.b = 0.5960785f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PaleTurquoise.
        /// </summary>
        public static Colorf PaleTurquoise
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6862745f;
                retVal.g = 0.9333333f;
                retVal.b = 0.9333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PaleVioletRed.
        /// </summary>
        public static Colorf PaleVioletRed
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8588235f;
                retVal.g = 0.4392157f;
                retVal.b = 0.5764706f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PapayaWhip.
        /// </summary>
        public static Colorf PapayaWhip
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.9372549f;
                retVal.b = 0.8352941f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PeachPuff.
        /// </summary>
        public static Colorf PeachPuff
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.854902f;
                retVal.b = 0.7254902f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Peru.
        /// </summary>
        public static Colorf Peru
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8039216f;
                retVal.g = 0.5215687f;
                retVal.b = 0.2470588f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Pink.
        /// </summary>
        public static Colorf Pink
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.7529412f;
                retVal.b = 0.7960784f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Plum.
        /// </summary>
        public static Colorf Plum
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8666667f;
                retVal.g = 0.627451f;
                retVal.b = 0.8666667f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color PowderBlue.
        /// </summary>
        public static Colorf PowderBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6901961f;
                retVal.g = 0.8784314f;
                retVal.b = 0.9019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Purple.
        /// </summary>
        public static Colorf Purple
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5019608f;
                retVal.g = 0.0f;
                retVal.b = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Red.
        /// </summary>
        public static Colorf Red
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.0f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color RosyBrown.
        /// </summary>
        public static Colorf RosyBrown
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.7372549f;
                retVal.g = 0.5607843f;
                retVal.b = 0.5607843f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color RoyalBlue.
        /// </summary>
        public static Colorf RoyalBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.254902f;
                retVal.g = 0.4117647f;
                retVal.b = 0.8823529f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SaddleBrown.
        /// </summary>
        public static Colorf SaddleBrown
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5450981f;
                retVal.g = 0.2705882f;
                retVal.b = 0.07450981f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Salmon.
        /// </summary>
        public static Colorf Salmon
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9803922f;
                retVal.g = 0.5019608f;
                retVal.b = 0.4470588f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SandyBrown.
        /// </summary>
        public static Colorf SandyBrown
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9568627f;
                retVal.g = 0.6431373f;
                retVal.b = 0.3764706f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SeaGreen.
        /// </summary>
        public static Colorf SeaGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.1803922f;
                retVal.g = 0.5450981f;
                retVal.b = 0.3411765f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SeaShell.
        /// </summary>
        public static Colorf SeaShell
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.9607843f;
                retVal.b = 0.9333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Sienna.
        /// </summary>
        public static Colorf Sienna
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.627451f;
                retVal.g = 0.3215686f;
                retVal.b = 0.1764706f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Silver.
        /// </summary>
        public static Colorf Silver
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.7529412f;
                retVal.g = 0.7529412f;
                retVal.b = 0.7529412f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SkyBlue.
        /// </summary>
        public static Colorf SkyBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.5294118f;
                retVal.g = 0.8078431f;
                retVal.b = 0.9215686f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SlateBlue.
        /// </summary>
        public static Colorf SlateBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4156863f;
                retVal.g = 0.3529412f;
                retVal.b = 0.8039216f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SlateGray.
        /// </summary>
        public static Colorf SlateGray
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.4392157f;
                retVal.g = 0.5019608f;
                retVal.b = 0.5647059f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Snow.
        /// </summary>
        public static Colorf Snow
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.9803922f;
                retVal.b = 0.9803922f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SpringGreen.
        /// </summary>
        public static Colorf SpringGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 1.0f;
                retVal.b = 0.4980392f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color SteelBlue.
        /// </summary>
        public static Colorf SteelBlue
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.2745098f;
                retVal.g = 0.509804f;
                retVal.b = 0.7058824f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Tan.
        /// </summary>
        public static Colorf Tan
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8235294f;
                retVal.g = 0.7058824f;
                retVal.b = 0.5490196f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Teal.
        /// </summary>
        public static Colorf Teal
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.0f;
                retVal.g = 0.5019608f;
                retVal.b = 0.5019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Thistle.
        /// </summary>
        public static Colorf Thistle
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.8470588f;
                retVal.g = 0.7490196f;
                retVal.b = 0.8470588f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Tomato.
        /// </summary>
        public static Colorf Tomato
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 0.3882353f;
                retVal.b = 0.2784314f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Turquoise.
        /// </summary>
        public static Colorf Turquoise
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.2509804f;
                retVal.g = 0.8784314f;
                retVal.b = 0.8156863f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Violet.
        /// </summary>
        public static Colorf Violet
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9333333f;
                retVal.g = 0.509804f;
                retVal.b = 0.9333333f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Wheat.
        /// </summary>
        public static Colorf Wheat
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9607843f;
                retVal.g = 0.8705882f;
                retVal.b = 0.7019608f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color White.
        /// </summary>
        public static Colorf White
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 1.0f;
                retVal.b = 1.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color WhiteSmoke.
        /// </summary>
        public static Colorf WhiteSmoke
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.9607843f;
                retVal.g = 0.9607843f;
                retVal.b = 0.9607843f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color Yellow.
        /// </summary>
        public static Colorf Yellow
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 1.0f;
                retVal.g = 1.0f;
                retVal.b = 0.0f;
                return retVal;
            }
        }

        /// <summary>
        ///		The color YellowGreen.
        /// </summary>
        public static Colorf YellowGreen
        {
            get
            {
                Colorf retVal;
                retVal.a = 1.0f;
                retVal.r = 0.6039216f;
                retVal.g = 0.8039216f;
                retVal.b = 0.1960784f;
                return retVal;
            }
        }

        //TODO : Move this to StringConverter
        public static Colorf Parse_0_255_String(string parsableText)
        {
            Colorf retVal;
            if (parsableText == null)
            {
                throw new ArgumentException("The parsableText parameter cannot be null.");
            }
            string[] vals = parsableText.TrimStart('(', '[', '<').TrimEnd(')', ']', '>').Split(',');
            if (vals.Length < 3)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it must of the form (r,g,b) or (r,g,b,a)",
                                                          parsableText));
            }
            //float r, g, b, a;
            try
            {
                retVal.r = int.Parse(vals[0].Trim()) / 255f;
                retVal.g = int.Parse(vals[1].Trim()) / 255f;
                retVal.b = int.Parse(vals[2].Trim()) / 255f;
                if (vals.Length == 4)
                {
                    retVal.a = int.Parse(vals[3].Trim()) / 255f;
                }
                else
                {
                    retVal.a = 1.0f;
                }
            }
            catch //(Exception e)
            {
                throw new FormatException("The parts of the ColorEx in Parse_0_255 must be integers");
            }
            return retVal;
        }

        //TODO : Move this to StringConverter
        public string To_0_255_String()
        {
            return string.Format("({0},{1},{2},{3})",
                                  (int)(r * 255f),
                                  (int)(g * 255f),
                                  (int)(b * 255f),
                                  (int)(a * 255f));
        }

        #endregion Static color properties

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
            return this.ToARGB();
        }

        public override bool Equals(object obj)
        {
            if (obj is Colorf)
            {
                return this == (Colorf)obj;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return this.To_0_255_String();
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
            Colorf other = (Colorf)obj;

            if (this.a == other.a &&
                this.r == other.r &&
                this.g == other.g &&
                this.b == other.b)
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
        public Colorf Clone()
        {
            Colorf clone;
            clone.a = this.a;
            clone.r = this.r;
            clone.g = this.g;
            clone.b = this.b;
            return clone;
        }

        #endregion ICloneable Implementation
    }
}
