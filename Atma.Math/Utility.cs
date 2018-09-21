﻿#region LGPL License

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

#endregion

#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Utility.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;
//using Atma.Utilities;
//using Atma;
//using Plane = Atma.Plane;
//
//using Radian = System.Single;
//using Degree = System.Single;

#endregion Namespace Declarations

namespace Atma
{
    public static partial class glm
    {
        public static readonly double PIOverTwo = Math.PI / 2.0;
        public static readonly double PIOverFour = Math.PI / 4.0;
        public static readonly double PI = Math.PI;
        public static readonly float PIf = (float)Math.PI;
        public static readonly double TwoPI = Math.PI * 2.0;
        //public static readonly float TWO_PI = PI * 2.0f;
        //public static readonly float HALF_PI = PI * 0.5f;

        //public static readonly float RADIANS_PER_DEGREE = PI / 180.0f;
        //public static readonly float DEGREES_PER_RADIAN = 180.0f / PI;

        private static Random random = new Random();

        /// <summary>
        /// Empty static constructor
        /// DO NOT DELETE.  It needs to be here because:
        /// 
        ///     # The presence of a static constructor suppresses beforeFieldInit.
        ///     # Static field variables are initialized before the static constructor is called.
        ///     # Having a static constructor is the only way to ensure that all resources are 
        ///       initialized before other static functions are called.
        /// 
        /// (from "Static Constructors Demystified" by Satya Komatineni
        ///  http://www.ondotnet.com/pub/a/dotnet/2003/07/07/staticxtor.html)
        /// </summary>
        static glm() { }

        public static float InvLength(vec2 lhs, float fail_value)
        {
            float d = lhs.x * lhs.x + lhs.y * lhs.y;
            if (d > 0.0f)
                return 1.0f / (float)Math.Sqrt(d);
            return fail_value;
        }

        public static int UpperPowerOfTwo(int v)
        {
            v--; v |= v >> 1; v |= v >> 2; v |= v >> 4; v |= v >> 8; v |= v >> 16; v++; return v;
        }

        public static vec2 RotateAround(vec2 src, vec2 pivot, float amount)
        {
            if (amount == 0)
                return src;

            var rot = pivot.Angle;
            rot += amount;

            var dir = new vec2((float)Math.Cos(rot), (float)Math.Sin(rot));
            return dir * (src - pivot).Length + pivot;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="aabb"></param>
        ///// <returns></returns>
        //public static float BoundingRadiusFromAABB(AxisAlignedBox aabb)
        //{
        //    Vector2 max = aabb.Maximum;
        //    Vector2 min = aabb.Minimum;

        //    Vector2 magnitude = max;
        //    magnitude.Ceil(-max);
        //    magnitude.Ceil(min);
        //    magnitude.Ceil(-min);

        //    return magnitude.Length;
        //}

        ///// <summary>
        /////		Converts radians to degrees.
        ///// </summary>
        ///// <param name="radians"></param>
        ///// <returns></returns>
        //public static Degree RadiansToDegrees(Radian radians)
        //{
        //    return radians;
        //}

        ///// <summary>
        /////		Converts degrees to radians.
        ///// </summary>
        ///// <param name="degrees"></param>
        ///// <returns></returns>
        //public static float DegreesToRadians(float degrees)
        //{
        //    return degrees * RADIANS_PER_DEGREE;
        //}

        ///// <summary>
        /////		Converts radians to degrees.
        ///// </summary>
        ///// <param name="radians"></param>
        ///// <returns></returns>
        //public static float RadiansToDegrees(float radians)
        //{
        //    return radians * DEGREES_PER_RADIAN;
        //}

        //public static float Lerp(float a, float b, float n)
        //{
        //    return (a * (1 - n) + b * n);
        //}

        //public static int Lerp(int a, int b, int n)
        //{
        //    return (a * (1 - n) + b * n);
        //}

        //public static float WrapAngle(float angle)
        //{
        //    angle = (float)System.Math.IEEERemainder(angle, 6.2831854820251465);
        //    if (angle <= -3.14159274f)
        //    {
        //        angle += 6.28318548f;
        //    }
        //    else
        //    {
        //        if (angle > 3.14159274f)
        //        {
        //            angle -= 6.28318548f;
        //        }
        //    }
        //    return angle;
        //}

        ///// <summary>
        /////     Compares float values for equality, taking into consideration
        /////     that floating point values should never be directly compared using
        /////     the '==' operator.  2 floats could be conceptually equal, but vary by a 
        /////     float.Epsilon which would fail in a direct comparison.  To circumvent that,
        /////     a tolerance value is used to see if the difference between the 2 floats
        /////     is less than the desired amount of accuracy.
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <param name="tolerance"></param>
        ///// <returns></returns>
        //public static bool floatEqual(float a, float b, float tolerance)
        //{
        //    return (System.Math.Abs(b - a) <= tolerance);
        //}

        ///// <summary>
        /////     Compares float values for equality, taking into consideration
        /////     that floating point values should never be directly compared using
        /////     the '==' operator.  2 floats could be conceptually equal, but vary by a 
        /////     float.Epsilon which would fail in a direct comparison.
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <returns></returns>
        //public static bool floatEqual(float a, float b)
        //{
        //    return (System.Math.Abs(b - a) <= float.Epsilon);
        //}

        //public static float Parsefloat(string value)
        //{
        //    return float.Parse(value, new System.Globalization.CultureInfo("en-US"));
        //}

        ///// <summary>
        /////     Returns the sign of a float number.
        ///// The result will be -1 for a negative number, 0 for zero and 1 for positive number.
        ///// </summary>
        ///// <param name="number"></param>
        ///// <returns></returns>
        //public static int Sign(float number)
        //{
        //    return System.Math.Sign(number);
        //}

        ///// <summary>
        /////	Returns the sine of the specified angle.
        ///// </summary>
        //public static float Sin(Radian angle)
        //{
        //    return (float)System.Math.Sin((double)angle);
        //}

        ///// <summary>
        /////	Returns the angle whose cosine is the specified number.
        ///// </summary>
        //public static Radian ASin(float angle)
        //{
        //    return (Radian)System.Math.Asin(angle);
        //}

        ///// <summary>
        /////	Returns the cosine of the specified angle.
        ///// </summary>
        //public static float Cos(Radian angle)
        //{
        //    return (float)System.Math.Cos((double)angle);
        //}

        ///// <summary>
        /////	Returns the angle whose cosine is the specified number.
        ///// </summary>
        //public static Radian ACos(float angle)
        //{
        //    // Ok, this needs to be looked at.  The decimal precision of float values can sometimes be 
        //    // *slightly* off from what is loaded from .skeleton files.  In some scenarios when we end up having 
        //    // a cos value calculated above that is just over 1 (i.e. 1.000000012), which the ACos of is Nan, thus 
        //    // completly throwing off node transformations and rotations associated with an animation.
        //    if (angle > 1)
        //    {
        //        angle = 1.0f;
        //    }

        //    return (Radian)System.Math.Acos(angle);
        //}

        ///// <summary>
        ///// Returns the tangent of the specified angle.
        ///// </summary>
        //public static float Tan(Radian value)
        //{
        //    return (float)System.Math.Tan((double)value);
        //}

        ///// <summary>
        ///// Return the angle whos tangent is the specified number.
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static Radian ATan(float value)
        //{
        //    return (Radian)System.Math.Atan(value);
        //}

        ///// <summary>
        ///// Returns the angle whose tangent is the quotient of the two specified numbers.
        ///// </summary>
        //public static Radian ATan(float y, float x)
        //{
        //    return (Radian)System.Math.Atan2(y, x);
        //}

        //public static float ATan2(float y, float x)
        //{
        //    return (float)System.Math.Atan2(y, x);
        //}

        ///// <summary>
        /////		Returns the square root of a number.
        ///// </summary>
        ///// <remarks>This is one of the more expensive math operations.  Avoid when possible.</remarks>
        ///// <param name="number"></param>
        ///// <returns></returns>
        //public static float Sqrt(float number)
        //{
        //    return (float)System.Math.Sqrt(number);
        //}

        ///// <summary>
        /////    Inverse square root.
        ///// </summary>
        ///// <param name="number"></param>
        ///// <returns></returns>
        //public static float InvSqrt(float number)
        //{
        //    return 1 / Sqrt(number);
        //}

        ///// <summary>
        /////     Raise a number to a power.
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <returns>The number x raised to power y</returns>
        //public static float Pow(float x, float y)
        //{
        //    return (float)System.Math.Pow((double)x, (double)y);
        //}

        ///// <summary>
        /////		Returns the absolute value of the supplied number.
        ///// </summary>
        ///// <param name="number"></param>
        ///// <returns></returns>
        //public static float Abs(float number)
        //{
        //    return (float)System.Math.Abs(number);
        //}

        ///// <summary>
        ///// Returns the maximum of the two supplied values.
        ///// </summary>
        ///// <param name="lhs"></param>
        ///// <param name="rhs"></param>
        ///// <returns></returns>
        //public static float Max(float lhs, float rhs)
        //{
        //    return lhs > rhs ? lhs : rhs;
        //}

        ///// <summary>
        ///// Returns the maximum of the two supplied values.
        ///// </summary>
        ///// <param name="lhs"></param>
        ///// <param name="rhs"></param>
        ///// <returns></returns>
        //public static long Max(long lhs, long rhs)
        //{
        //    return lhs > rhs ? lhs : rhs;
        //}


        ///// <summary>
        ///// Returns the maximum of the two supplied values.
        ///// </summary>
        ///// <param name="lhs"></param>
        ///// <param name="rhs"></param>
        ///// <returns></returns>
        //public static int Max(int lhs, int rhs)
        //{
        //    return lhs > rhs ? lhs : rhs;
        //}

        //public static vec2 Max(vec2 lhs, vec2 rhs)
        //{
        //    return new vec2(Max(lhs.x, rhs.x), Max(lhs.y, rhs.y));
        //}

        ///// <summary>
        /////     Finds the first maximum value in the array and returns the index of it.
        ///// </summary>
        ///// <param name="values">Array of values containing one value at least.</param>
        ///// <returns></returns>
        //public static int Max(float[] values)
        //{
        //    Debug.Assert(values != null && values.Length > 0);

        //    int maxIndex = 0;
        //    float max = values[0];
        //    for (int i = 1; i < values.Length; i++)
        //    {
        //        if (values[i] > max)
        //        {
        //            max = values[i];
        //            maxIndex = i;
        //        }
        //    }

        //    return maxIndex;
        //}

        ///// <summary>
        ///// Returns the minumum of the two supplied values.
        ///// </summary>
        ///// <param name="lhs"></param>
        ///// <param name="rhs"></param>
        ///// <returns></returns>
        //public static float Min(float lhs, float rhs)
        //{
        //    return lhs < rhs ? lhs : rhs;
        //}

        ///// <summary>
        ///// Returns the minumum of the two supplied values.
        ///// </summary>
        ///// <param name="lhs"></param>
        ///// <param name="rhs"></param>
        ///// <returns></returns>
        //public static long Min(long lhs, long rhs)
        //{
        //    return lhs < rhs ? lhs : rhs;
        //}

        ///// <summary>
        ///// Returns the minumum of the two supplied values.
        ///// </summary>
        ///// <param name="lhs"></param>
        ///// <param name="rhs"></param>
        ///// <returns></returns>
        //public static int Min(int lhs, int rhs)
        //{
        //    return lhs < rhs ? lhs : rhs;
        //}

        //public static vec2 Min(vec2 lhs, vec2 rhs)
        //{
        //    return new vec2(Min(lhs.x, rhs.x), Min(lhs.y, rhs.y));
        //}

        ///// <summary>
        /////     Finds the first minimum value in the array and returns the index of it.
        ///// </summary>
        ///// <param name="values">Array of values containing one value at least.</param>
        ///// <returns></returns>
        //public static int Min(float[] values)
        //{
        //    Debug.Assert(values != null && values.Length > 0);

        //    int minIndex = 0;
        //    float min = values[0];
        //    for (int i = 1; i < values.Length; i++)
        //    {
        //        if (values[i] < min)
        //        {
        //            min = values[i];
        //            minIndex = i;
        //        }
        //    }

        //    return minIndex;
        //}

        ///// <summary>
        ///// Returns the smallest integer greater than or equal to the specified value.
        ///// </summary>
        ///// <param name="number"></param>
        ///// <returns></returns>
        //public static float Ceiling(float number)
        //{
        //    return (float)System.Math.Ceiling(number);
        //}

        ///// <summary>
        /////    Returns a random value between the specified min and max values.
        ///// </summary>
        ///// <param name="min">Minimum value.</param>
        ///// <param name="max">Maximum value.</param>
        ///// <returns>A random value in the range [min,max].</returns>
        //public static float RangeRandom(float min, float max)
        //{
        //    return (max - min) * UnitRandom() + min;
        //}

        ///// <summary>
        /////    
        ///// </summary>
        ///// <returns></returns>
        //public static float UnitRandom()
        //{
        //    return (float)random.Next(Int32.MaxValue) / (float)Int32.MaxValue;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public static float SymmetricRandom()
        //{
        //    return (float)(2.0f * UnitRandom() - 1.0f);
        //}

        /////// <summary>
        ///////     Builds a reflection matrix for the specified plane.
        /////// </summary>
        /////// <param name="plane"></param>
        /////// <returns></returns>
        ////public static Matrix4 BuildReflectionMatrix(Plane plane)
        ////{
        ////    Vector2 normal = plane.Normal;

        ////    return new Matrix4(
        ////        -2.0f * normal.x * normal.x + 1.0f, -2.0f * normal.x * normal.y, -2.0f * normal.x * normal.z, -2.0f * normal.x * plane.D,
        ////        -2.0f * normal.y * normal.x, -2.0f * normal.y * normal.y + 1.0f, -2.0f * normal.y * normal.z, -2.0f * normal.y * plane.D,
        ////        -2.0f * normal.z * normal.x, -2.0f * normal.z * normal.y, -2.0f * normal.z * normal.z + 1.0f, -2.0f * normal.z * plane.D,
        ////        0.0f, 0.0f, 0.0f, 1.0f);
        ////}

        /////// <summary>
        ///////		Calculate a face normal, including the w script which is the offset from the origin.
        /////// </summary>
        /////// <param name="v1"></param>
        /////// <param name="v2"></param>
        /////// <param name="v3"></param>
        /////// <returns></returns>
        ////public static Vector4 CalculateFaceNormal(Vector2 v1, Vector2 v2, Vector2 v3)
        ////{
        ////    Vector2 normal = CalculateBasicFaceNormal(v1, v2, v3);

        ////    // Now set up the w (distance of tri from origin
        ////    return new Vector4(normal.x, normal.y, normal.z, -(normal.Dot(v1)));
        ////}

        /////// <summary>
        ///////		Calculate a face normal, no w-information.
        /////// </summary>
        /////// <param name="v1"></param>
        /////// <param name="v2"></param>
        /////// <param name="v3"></param>
        /////// <returns></returns>
        ////public static Vector2 CalculateBasicFaceNormal(Vector2 v1, Vector2 v2, Vector2 v3)
        ////{
        ////    Vector2 normal = (v2 - v1).Cross(v3 - v1);
        ////    normal.Normalize();

        ////    return normal;
        ////}

        /////// <summary>
        ///////		Calculate a face normal, no w-information.
        /////// </summary>
        /////// <param name="v1"></param>
        /////// <param name="v2"></param>
        /////// <param name="v3"></param>
        /////// <returns></returns>
        ////public static Vector2 CalculateBasicFaceNormalWithoutNormalize(Vector2 v1, Vector2 v2, Vector2 v3)
        ////{
        ////    Vector2 normal = (v2 - v1).Cross(v3 - v1);
        ////    return normal;
        ////}

        /////// <summary>
        ///////    Calculates the tangent space vector for a given set of positions / texture coords.
        /////// </summary>
        /////// <remarks>
        ///////    Adapted from bump mapping tutorials at:
        ///////    http://www.paulsprojects.net/tutorials/simplebump/simplebump.html
        ///////    author : paul.baker@univ.ox.ac.uk
        /////// </remarks>
        /////// <param name="position1"></param>
        /////// <param name="position2"></param>
        /////// <param name="position3"></param>
        /////// <param name="u1"></param>
        /////// <param name="v1"></param>
        /////// <param name="u2"></param>
        /////// <param name="v2"></param>
        /////// <param name="u3"></param>
        /////// <param name="v3"></param>
        /////// <returns></returns>
        ////public static Vector2 CalculateTangentSpaceVector(
        ////    Vector2 position1, Vector2 position2, Vector2 position3, float u1, float v1, float u2, float v2, float u3, float v3)
        ////{
        ////    // side0 is the vector along one side of the triangle of vertices passed in, 
        ////    // and side1 is the vector along another side. Taking the cross product of these returns the normal.
        ////    Vector2 side0 = position1 - position2;
        ////    Vector2 side1 = position3 - position1;
        ////    // Calculate face normal
        ////    Vector2 normal = side1.Cross(side0);
        ////    normal.Normalize();

        ////    // Now we use a formula to calculate the tangent. 
        ////    float deltaV0 = v1 - v2;
        ////    float deltaV1 = v3 - v1;
        ////    Vector2 tangent = deltaV1 * side0 - deltaV0 * side1;
        ////    tangent.Normalize();

        ////    // Calculate binormal
        ////    float deltaU0 = u1 - u2;
        ////    float deltaU1 = u3 - u1;
        ////    Vector2 binormal = deltaU1 * side0 - deltaU0 * side1;
        ////    binormal.Normalize();

        ////    // Now, we take the cross product of the tangents to get a vector which 
        ////    // should point in the same direction as our normal calculated above. 
        ////    // If it points in the opposite direction (the dot product between the normals is less than zero), 
        ////    // then we need to reverse the s and t tangents. 
        ////    // This is because the triangle has been mirrored when going from tangent space to object space.
        ////    // reverse tangents if necessary.
        ////    Vector2 tangentCross = tangent.Cross(binormal);
        ////    if (tangentCross.Dot(normal) < 0.0f)
        ////    {
        ////        tangent = -tangent;
        ////        binormal = -binormal;
        ////    }

        ////    return tangent;
        ////}

        ///// <summary>
        /////		Checks wether a given point is inside a triangle, in a
        /////		2-dimensional (Cartesian) space.
        ///// </summary>
        ///// <remarks>
        /////		The vertices of the triangle must be given in either
        /////		trigonometrical (anticlockwise) or inverse trigonometrical
        /////		(clockwise) order.
        ///// </remarks>
        ///// <param name="px">
        /////    The X-coordinate of the point.
        ///// </param>
        ///// <param name="py">
        /////    The Y-coordinate of the point.
        ///// </param>
        ///// <param name="ax">
        /////    The X-coordinate of the triangle's first vertex.
        ///// </param>
        ///// <param name="ay">
        /////    The Y-coordinate of the triangle's first vertex.
        ///// </param>
        ///// <param name="bx">
        /////    The X-coordinate of the triangle's second vertex.
        ///// </param>
        ///// <param name="by">
        /////    The Y-coordinate of the triangle's second vertex.
        ///// </param>
        ///// <param name="cx">
        /////    The X-coordinate of the triangle's third vertex.
        ///// </param>
        ///// <param name="cy">
        /////    The Y-coordinate of the triangle's third vertex.
        ///// </param>
        ///// <returns>
        /////    <list type="bullet">
        /////        <item>
        /////            <description><b>true</b> - the point resides in the triangle.</description>
        /////        </item>
        /////        <item>
        /////            <description><b>false</b> - the point is outside the triangle</description>
        /////         </item>
        /////     </list>
        ///// </returns>
        //public static bool PointInTri2D(float px, float py, float ax, float ay, float bx, float by, float cx, float cy)
        //{
        //    float v1x, v2x, v1y, v2y;
        //    bool bClockwise;

        //    v1x = bx - ax;
        //    v1y = by - ay;

        //    v2x = px - bx;
        //    v2y = py - by;

        //    bClockwise = (v1x * v2y - v1y * v2x >= 0.0);

        //    v1x = cx - bx;
        //    v1y = cy - by;

        //    v2x = px - cx;
        //    v2y = py - cy;

        //    if ((v1x * v2y - v1y * v2x >= 0.0) != bClockwise)
        //    {
        //        return false;
        //    }

        //    v1x = ax - cx;
        //    v1y = ay - cy;

        //    v2x = px - ax;
        //    v2y = py - ay;

        //    if ((v1x * v2y - v1y * v2x >= 0.0) != bClockwise)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        ///// <summary>
        /////    Method delegate with a simple signature. 
        /////    Used to measure execution time of a method for instance.
        ///// </summary>
        //public delegate void SimpleMethodDelegate();

        ///// <summary>
        /////     Measure the execution time of a method.
        ///// </summary>
        ///// <param name="method"></param>
        ///// <returns>The elapsed time in seconds.</returns>
        //public static float Measure(SimpleMethodDelegate method)
        //{
        //    long start = System.Diagnostics.Stopwatch.GetTimestamp();

        //    method();

        //    double elapsed = (double)(System.Diagnostics.Stopwatch.GetTimestamp() - start);
        //    double freq = (double)System.Diagnostics.Stopwatch.Frequency;

        //    return (float)(elapsed / freq);
        //}

        //#region Intersection Methods

        /// <summary>
        ///    Tests an intersection between a ray and a box.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="box"></param>
        /// <returns>A Pair object containing whether the intersection occurred, and the distance between the 2 objects.</returns>
        public static IntersectResult Intersects(Ray ray, AxisAlignedBox box)
        {
            //Contract.RequiresNotNull(ray, "ray");
            //Contract.RequiresNotNull(box, "box");

            if (box.IsNull)
            {
                return new IntersectResult(false, 0);
            }

            if (box.IsInfinite)
            {
                return new IntersectResult(true, 0);
            }

            float lowt = 0.0f;
            float t;
            bool hit = false;
            vec2 hitPoint;
            vec2 min = box.Minimum;
            vec2 max = box.Maximum;

            // check origin inside first
            if (ray.origin.x > min.x && ray.origin.y > min.y && ray.origin.x < max.x && ray.origin.y < max.y)
            {
                return new IntersectResult(true, 0.0f);
            }

            // check each face in turn, only check closest 3

            // Min X
            if (ray.origin.x <= min.x && ray.direction.x > 0)
            {
                t = (min.x - ray.origin.x) / ray.direction.x;

                if (t >= 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.y >= min.y && hitPoint.y <= max.y &&
                        //hitPoint.z >= min.z && hitPoint.z <= max.z &&
                        (!hit || t < lowt))
                    {
                        hit = true;
                        lowt = t;
                    }
                }
            }

            // Max X
            if (ray.origin.x >= max.x && ray.direction.x < 0)
            {
                t = (max.x - ray.origin.x) / ray.direction.x;

                if (t >= 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.y >= min.y && hitPoint.y <= max.y &&
                        //hitPoint.z >= min.z && hitPoint.z <= max.z &&
                        (!hit || t < lowt))
                    {
                        hit = true;
                        lowt = t;
                    }
                }
            }

            // Min Y
            if (ray.origin.y <= min.y && ray.direction.y > 0)
            {
                t = (min.y - ray.origin.y) / ray.direction.y;

                if (t >= 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.x >= min.x && hitPoint.x <= max.x &&
                        //hitPoint.z >= min.z && hitPoint.z <= max.z &&
                        (!hit || t < lowt))
                    {
                        hit = true;
                        lowt = t;
                    }
                }
            }

            // Max Y
            if (ray.origin.y >= max.y && ray.direction.y < 0)
            {
                t = (max.y - ray.origin.y) / ray.direction.y;

                if (t >= 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.x >= min.x && hitPoint.x <= max.x &&
                        //hitPoint.z >= min.z && hitPoint.z <= max.z &&
                        (!hit || t < lowt))
                    {
                        hit = true;
                        lowt = t;
                    }
                }
            }

            return new IntersectResult(hit, lowt);
        }

        //public static IntersectResult Intersects(Ray ray, Vector2 a,
        //                                          Vector2 b, Vector2 c, Vector2 normal, bool positiveSide, bool negativeSide)
        //{
        //    // Calculate intersection with plane.
        //    float t;
        //    {
        //        float denom = normal.Dot(ray.Direction);
        //        // Check intersect side
        //        if (denom > +float.Epsilon)
        //        {
        //            if (!negativeSide)
        //            {
        //                return new IntersectResult(false, 0);
        //            }
        //        }
        //        else if (denom < -float.Epsilon)
        //        {
        //            if (!positiveSide)
        //            {
        //                return new IntersectResult(false, 0);
        //            }
        //        }
        //        else
        //        {
        //            // Parallel or triangle area is close to zero when
        //            // the plane normal not normalised.
        //            return new IntersectResult(false, 0);
        //        }

        //        t = normal.Dot(a - ray.Origin) / denom;
        //        if (t < 0)
        //        {
        //            return new IntersectResult(false, 0);
        //        }
        //    }

        //    // Calculate the largest area projection plane in X, Y or Z.
        //    int i0, i1;
        //    {
        //        float n0 = Math.Utility.Abs(normal[0]);
        //        float n1 = Math.Utility.Abs(normal[1]);
        //        float n2 = Math.Utility.Abs(normal[2]);

        //        i0 = 1;
        //        i1 = 2;
        //        if (n1 > n2)
        //        {
        //            if (n1 > n0)
        //            {
        //                i0 = 0;
        //            }
        //        }
        //        else
        //        {
        //            if (n2 > n0)
        //            {
        //                i1 = 0;
        //            }
        //        }
        //    }

        //    // Check the intersection point is inside the triangle.
        //    {
        //        float u1 = b[i0] - a[i0];
        //        float v1 = b[i1] - a[i1];
        //        float u2 = c[i0] - a[i0];
        //        float v2 = c[i1] - a[i1];
        //        float u0 = t * ray.Direction[i0] + ray.Origin[i0] - a[i0];
        //        float v0 = t * ray.Direction[i1] + ray.Origin[i1] - a[i1];

        //        float alpha = u0 * v2 - u2 * v0;
        //        float beta = u1 * v0 - u0 * v1;
        //        float area = u1 * v2 - u2 * v1;

        //        // epsilon to avoid float precision error
        //        float EPSILON = 1e-3f;

        //        float tolerance = -EPSILON * area;

        //        if (area > 0)
        //        {
        //            if (alpha < tolerance || beta < tolerance || alpha + beta > area - tolerance)
        //            {
        //                return new IntersectResult(false, 0);
        //            }
        //        }
        //        else
        //        {
        //            if (alpha > tolerance || beta > tolerance || alpha + beta < area - tolerance)
        //            {
        //                return new IntersectResult(false, 0);
        //            }
        //        }
        //    }

        //    return new IntersectResult(true, t);
        //}

        //public static IntersectResult Intersects(Ray ray, Vector2 a,
        //                                          Vector2 b, Vector2 c, bool positiveSide, bool negativeSide)
        //{
        //    Vector2 normal = CalculateBasicFaceNormalWithoutNormalize(a, b, c);
        //    return Intersects(ray, a, b, c, normal, positiveSide, negativeSide);
        //}

        ///// <summary>
        /////    Tests an intersection between two boxes.
        ///// </summary>
        ///// <param name="boxA">
        /////    The primary box.
        ///// </param>
        ///// <param name="boxB">
        /////    The box to test intersection with boxA.
        ///// </param>
        ///// <returns>
        /////    <list type="bullet">
        /////        <item>
        /////            <description>None - There was no intersection between the 2 boxes.</description>
        /////        </item>
        /////        <item>
        /////            <description>Contained - boxA is fully within boxB.</description>
        /////         </item>
        /////        <item>
        /////            <description>Contains - boxB is fully within boxA.</description>
        /////         </item>
        /////        <item>
        /////            <description>Partial - boxA is partially intersecting with boxB.</description>
        /////         </item>
        /////     </list>
        ///// </returns>
        ///// Submitted by: romout
        //public static Intersection Intersects(AxisAlignedBox boxA, AxisAlignedBox boxB)
        //{
        //    Contract.RequiresNotNull(boxA, "boxA");
        //    Contract.RequiresNotNull(boxB, "boxB");

        //    // grab the max and mix vectors for both boxes for comparison
        //    Vector2 minA = boxA.Minimum;
        //    Vector2 maxA = boxA.Maximum;
        //    Vector2 minB = boxB.Minimum;
        //    Vector2 maxB = boxB.Maximum;

        //    if ((minB.x < minA.x) &&
        //        (maxB.x > maxA.x) &&
        //        (minB.y < minA.y) &&
        //        (maxB.y > maxA.y) &&
        //        (minB.z < minA.z) &&
        //        (maxB.z > maxA.z))
        //    {
        //        // boxA is within boxB
        //        return Intersection.Contained;
        //    }

        //    if ((minB.x > minA.x) &&
        //        (maxB.x < maxA.x) &&
        //        (minB.y > minA.y) &&
        //        (maxB.y < maxA.y) &&
        //        (minB.z > minA.z) &&
        //        (maxB.z < maxA.z))
        //    {
        //        // boxB is within boxA
        //        return Intersection.Contains;
        //    }

        //    if ((minB.x > maxA.x) ||
        //        (minB.y > maxA.y) ||
        //        (minB.z > maxA.z) ||
        //        (maxB.x < minA.x) ||
        //        (maxB.y < minA.y) ||
        //        (maxB.z < minA.z))
        //    {
        //        // not interesting at all
        //        return Intersection.None;
        //    }

        //    // if we got this far, they are partially intersecting
        //    return Intersection.Partial;
        //}

        public static IntersectResult Intersects(Ray ray, Circle circle)
        {
            return Intersects(ray, circle, false);
        }

        /// <summary>
        ///		Ray/Sphere intersection test.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="sphere"></param>
        /// <param name="discardInside"></param>
        /// <returns>Struct that contains a bool (hit?) and distance.</returns>
        public static IntersectResult Intersects(Ray ray, Circle circle, bool discardInside)
        {
            //Contract.RequiresNotNull(ray, "ray");
            //Contract.RequiresNotNull(sphere, "sphere");

            vec2 rayDir = ray.Direction;
            //Adjust ray origin relative to sphere center
            vec2 rayOrig = ray.Origin - circle.Center;
            float radius = circle.Radius;

            // check origin inside first
            if ((rayOrig.LengthSqr <= radius * radius) && discardInside)
            {
                return new IntersectResult(true, 0);
            }

            // mmm...sweet quadratics
            // Build coeffs which can be used with std quadratic solver
            // ie t = (-b +/- sqrt(b*b* + 4ac)) / 2a
            float a = vec2.Dot(rayDir, rayDir);
            float b = 2 * vec2.Dot(rayOrig, rayDir);
            float c = vec2.Dot(rayOrig, rayOrig) - (radius * radius);

            // calc determinant
            float d = (b * b) - (4 * a * c);

            if (d < 0)
            {
                // no intersection
                return new IntersectResult(false, 0);
            }
            else
            {
                // BTW, if d=0 there is one intersection, if d > 0 there are 2
                // But we only want the closest one, so that's ok, just use the 
                // '-' version of the solver
                float t = (-b - glm.Sqrt(d)) / (2 * a);

                if (t < 0)
                {
                    t = (-b + glm.Sqrt(d)) / (2 * a);
                }

                return new IntersectResult(true, t);
            }
        }

        ///// <summary>
        /////		Ray/Plane intersection test.
        ///// </summary>
        ///// <param name="ray"></param>
        ///// <param name="plane"></param>
        ///// <returns>Struct that contains a bool (hit?) and distance.</returns>
        //public static IntersectResult Intersects(Ray ray, Plane plane)
        //{
        //    //Contract.RequiresNotNull(ray, "ray");

        //    float denom = vec3.Dot(plane.Normal, ray.Direction);

        //    if (glm.Abs(denom) < float.Epsilon)
        //    {
        //        // Parellel
        //        return new IntersectResult(false, 0);
        //    }
        //    else
        //    {
        //        float nom = plane.Normal.Dot(ray.Origin) + plane.D;
        //        float t = -(nom / denom);
        //        return new IntersectResult(t >= 0, t);
        //    }
        //}

        public static bool Intersects(Rectangle rect, vec2 start, vec2 end)
        {
            //top line
            var a1 = new vec2(rect.X, rect.Y);
            var a2 = new vec2(rect.Right, rect.Y);

            //right line
            var b1 = new vec2(rect.Right, rect.Y);
            var b2 = new vec2(rect.Right, rect.Bottom);

            //bottom line
            var c1 = new vec2(rect.X, rect.Bottom);
            var c2 = new vec2(rect.Right, rect.Bottom);

            //left line
            var d1 = new vec2(rect.X, rect.Y);
            var d2 = new vec2(rect.X, rect.Bottom);

            return glm.Intersects(start, end, a1, a2) || glm.Intersects(start, end, b1, b2) ||
                    glm.Intersects(start, end, c1, c2) || glm.Intersects(start, end, d1, d2);
        }

        /// <summary>
        ///		Sphere/Box intersection test.
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="box"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Circle circle, AxisAlignedBox box)
        {
            //Contract.RequiresNotNull(sphere, "sphere");
            //Contract.RequiresNotNull(box, "box");

            if (box.IsNull)
            {
                return false;
            }

            // Use splitting planes
            vec2 center = circle.Center;
            float radius = circle.Radius;
            vec2 min = box.Minimum;
            vec2 max = box.Maximum;

            // just test facing planes, early fail if sphere is totally outside
            if (center.x < min.x &&
                min.x - center.x > radius)
            {
                return false;
            }
            if (center.x > max.x &&
                center.x - max.x > radius)
            {
                return false;
            }

            if (center.y < min.y &&
                min.y - center.y > radius)
            {
                return false;
            }
            if (center.y > max.y &&
                center.y - max.y > radius)
            {
                return false;
            }

            // Must intersect
            return true;
        }

        /// <summary>
        ///		Sphere/Box intersection test.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="box"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Sphere sphere, AxisAlignedBox3 box)
        {
            //Contract.RequiresNotNull(sphere, "sphere");
            //Contract.RequiresNotNull(box, "box");

            if (box.IsNull)
            {
                return false;
            }

            // Use splitting planes
            vec3 center = sphere.Center;
            float radius = sphere.Radius;
            vec3 min = box.Minimum;
            vec3 max = box.Maximum;

            // just test facing planes, early fail if sphere is totally outside
            if (center.x < min.x &&
                min.x - center.x > radius)
            {
                return false;
            }
            if (center.x > max.x &&
                center.x - max.x > radius)
            {
                return false;
            }

            if (center.y < min.y &&
                min.y - center.y > radius)
            {
                return false;
            }
            if (center.y > max.y &&
                center.y - max.y > radius)
            {
                return false;
            }

            if (center.z < min.z &&
                min.z - center.z > radius)
            {
                return false;
            }
            if (center.z > max.z &&
                center.z - max.z > radius)
            {
                return false;
            }

            // Must intersect
            return true;
        }

        /// <summary>
        ///		Plane/Box intersection test.
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="box"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Plane plane, AxisAlignedBox3 box)
        {
            //Contract.RequiresNotNull(box, "box");

            if (box.IsNull)
            {
                return false;
            }

            // Get corners of the box
            vec3[] corners = box.Corners;

            // Test which side of the plane the corners are
            // Intersection occurs when at least one corner is on the 
            // opposite side to another
            PlaneSide lastSide = plane.GetSide(corners[0]);

            for (int corner = 1; corner < 8; corner++)
            {
                if (plane.GetSide(corners[corner]) != lastSide)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///		Sphere/Plane intersection test.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="plane"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Sphere sphere, Plane plane)
        {
            //Contract.RequiresNotNull(sphere, "sphere");

            return glm.Abs(vec3.Dot(plane.Normal, sphere.Center)) <= sphere.Radius;
        }

        ///// <summary>
        /////     Builds a reflection matrix for the specified plane.
        ///// </summary>
        ///// <param name="plane"></param>
        ///// <returns></returns>
        //public static mat4 BuildReflectionMatrix(Plane plane)
        //{
        //    vec3 normal = plane.Normal;

        //    return new mat4(
        //        -2.0f * normal.x * normal.x + 1.0f, -2.0f * normal.x * normal.y, -2.0f * normal.x * normal.z, -2.0f * normal.x * plane.D,
        //        -2.0f * normal.y * normal.x, -2.0f * normal.y * normal.y + 1.0f, -2.0f * normal.y * normal.z, -2.0f * normal.y * plane.D,
        //        -2.0f * normal.z * normal.x, -2.0f * normal.z * normal.y, -2.0f * normal.z * normal.z + 1.0f, -2.0f * normal.z * plane.D,
        //        0.0f, 0.0f, 0.0f, 1.0f);
        //}

        ///// <summary>
        /////		Plane/Box intersection test.
        ///// </summary>
        ///// <param name="plane"></param>
        ///// <param name="box"></param>
        ///// <returns>True if there was an intersection, false otherwise.</returns>
        //public static bool Intersects(Plane plane, AxisAlignedBox box)
        //{
        //    Contract.RequiresNotNull(box, "box");

        //    if (box.IsNull)
        //    {
        //        return false;
        //    }

        //    // Get corners of the box
        //    Vector2[] corners = box.Corners;

        //    // Test which side of the plane the corners are
        //    // Intersection occurs when at least one corner is on the 
        //    // opposite side to another
        //    PlaneSide lastSide = plane.GetSide(corners[0]);

        //    for (int corner = 1; corner < 8; corner++)
        //    {
        //        if (plane.GetSide(corners[corner]) != lastSide)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        ///// <summary>
        /////		Sphere/Plane intersection test.
        ///// </summary>
        ///// <param name="sphere"></param>
        ///// <param name="plane"></param>
        ///// <returns>True if there was an intersection, false otherwise.</returns>
        //public static bool Intersects(Sphere sphere, Plane plane)
        //{
        //    Contract.RequiresNotNull(sphere, "sphere");

        //    return Utility.Abs(plane.Normal.Dot(sphere.Center)) <= sphere.Radius;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="ray"></param>
        ///// <param name="box"></param>
        ///// <param name="d1"></param>
        ///// <param name="d2"></param>
        ///// <returns></returns>
        //public static Tuple<bool, float, float> Intersect(Ray ray, AxisAlignedBox box)
        //{
        //    if (box.IsNull)
        //    {
        //        return new Tuple<bool, float, float>(false, float.NaN, float.NaN);
        //    }

        //    if (box.IsInfinite)
        //    {
        //        return new Tuple<bool, float, float>(true, float.NaN, float.PositiveInfinity);
        //    }

        //    Vector2 min = box.Minimum;
        //    Vector2 max = box.Maximum;
        //    Vector2 rayorig = ray.origin;
        //    Vector2 rayDir = ray.Direction;

        //    Vector2 absDir = Vector2.Zero;
        //    absDir[0] = Abs(rayDir[0]);
        //    absDir[1] = Abs(rayDir[1]);
        //    absDir[2] = Abs(rayDir[2]);

        //    // Sort the axis, ensure check minimise floating error axis first
        //    int imax = 0, imid = 1, imin = 2;
        //    if (absDir[0] < absDir[2])
        //    {
        //        imax = 2;
        //        imin = 0;
        //    }
        //    if (absDir[1] < absDir[imin])
        //    {
        //        imid = imin;
        //        imin = 1;
        //    }
        //    else if (absDir[1] > absDir[imax])
        //    {
        //        imid = imax;
        //        imax = 1;
        //    }

        //    float start = 0, end = float.PositiveInfinity;
        //    // Check each axis in turn

        //    if (!CalcAxis(imax, rayDir, rayorig, min, max, ref end, ref start))
        //    {
        //        return new Tuple<bool, float, float>(false, float.NaN, float.NaN);
        //    }

        //    if (absDir[imid] < float.Epsilon)
        //    {
        //        // Parallel with middle and minimise axis, check bounds only
        //        if (rayorig[imid] < min[imid] || rayorig[imid] > max[imid] ||
        //            rayorig[imin] < min[imin] || rayorig[imin] > max[imin])
        //        {
        //            return new Tuple<bool, float, float>(false, float.NaN, float.NaN);
        //        }
        //    }
        //    else
        //    {
        //        if (!CalcAxis(imid, rayDir, rayorig, min, max, ref end, ref start))
        //        {
        //            return new Tuple<bool, float, float>(false, float.NaN, float.NaN);
        //        }

        //        if (absDir[imin] < float.Epsilon)
        //        {
        //            // Parallel with minimise axis, check bounds only
        //            if (rayorig[imin] < min[imin] || rayorig[imin] > max[imin])
        //            {
        //                return new Tuple<bool, float, float>(false, float.NaN, float.NaN);
        //            }
        //        }
        //        else
        //        {
        //            if (!CalcAxis(imin, rayDir, rayorig, min, max, ref end, ref start))
        //            {
        //                return new Tuple<bool, float, float>(false, float.NaN, float.NaN);
        //            }
        //        }
        //    }
        //    return new Tuple<bool, float, float>(true, start, end);
        //}

        //private static bool CalcAxis(int i, Vector2 raydir, Vector2 rayorig, Vector2 min, Vector2 max, ref float end, ref float start)
        //{
        //    float denom = 1 / raydir[i];
        //    float newstart = (min[i] - rayorig[i]) * denom;
        //    float newend = (max[i] - rayorig[i]) * denom;
        //    if (newstart > newend)
        //    {
        //        Swap<float>(ref newstart, ref newend);
        //    }
        //    if (newstart > end || newend < start)
        //    {
        //        return false;
        //    }
        //    if (newstart > start)
        //    {
        //        start = newstart;
        //    }
        //    if (newend < end)
        //    {
        //        end = newend;
        //    }

        //    return true;
        //}

        ///// <summary>
        /////    Ray/PlaneBoundedVolume intersection test.
        ///// </summary>
        ///// <param name="ray"></param>
        ///// <param name="volume"></param>
        ///// <returns>Struct that contains a bool (hit?) and distance.</returns>
        //public static IntersectResult Intersects(Ray ray, PlaneBoundedVolume volume)
        //{
        //    Contract.RequiresNotNull(ray, "ray");
        //    Contract.RequiresNotNull(volume, "volume");

        //    PlaneList planes = volume.planes;

        //    float maxExtDist = 0.0f;
        //    float minIntDist = float.PositiveInfinity;

        //    float dist, denom, nom;

        //    for (int i = 0; i < planes.Count; i++)
        //    {
        //        Plane plane = (Plane)planes[i];

        //        denom = plane.Normal.Dot(ray.Direction);
        //        if (Utility.Abs(denom) < float.Epsilon)
        //        {
        //            // Parallel
        //            if (plane.GetSide(ray.Origin) == volume.outside)
        //            {
        //                return new IntersectResult(false, 0);
        //            }

        //            continue;
        //        }

        //        nom = plane.Normal.Dot(ray.Origin) + plane.D;
        //        dist = -(nom / denom);

        //        if (volume.outside == PlaneSide.Negative)
        //        {
        //            nom = -nom;
        //        }

        //        if (dist > 0.0f)
        //        {
        //            if (nom > 0.0f)
        //            {
        //                if (maxExtDist < dist)
        //                {
        //                    maxExtDist = dist;
        //                }
        //            }
        //            else
        //            {
        //                if (minIntDist > dist)
        //                {
        //                    minIntDist = dist;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //Ray points away from plane
        //            if (volume.outside == PlaneSide.Negative)
        //            {
        //                denom = -denom;
        //            }

        //            if (denom > 0.0f)
        //            {
        //                return new IntersectResult(false, 0);
        //            }
        //        }
        //    }

        //    if (maxExtDist > minIntDist)
        //    {
        //        return new IntersectResult(false, 0);
        //    }

        //    return new IntersectResult(true, maxExtDist);
        //}

        //#endregion Intersection Methods

        ///// <summary>
        ///// Swaps two values
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="v1"></param>
        ///// <param name="v2"></param>
        //public static void Swap<T>(ref T v1, ref T v2)
        //{
        //    T temp = v1;
        //    v1 = v2;
        //    v2 = temp;
        //}

        //public static bool Between(int v, int v0, int v1)
        //{
        //    if (v0 < v1)
        //        return (v > v0) && (v < v1);

        //    return (v > v1) && (v < v0);
        //}

        //public static bool Between(float v, float v0, float v1)
        //{
        //    if (v0 < v1)
        //        return (v > v0) && (v < v1);

        //    return (v > v1) && (v < v0);
        //}

        //public static bool BetweenOrEq(int v, int v0, int v1)
        //{
        //    if (v0 < v1)
        //        return (v >= v0) && (v <= v1);

        //    return (v >= v1) && (v <= v0);
        //}

        //public static bool BetweenOrEq(float v, float v0, float v1)
        //{
        //    if (v0 < v1)
        //        return (v >= v0) && (v <= v1);

        //    return (v >= v1) && (v <= v0);
        //}

        //public static vec2 ScaleToSize(vec2 size, vec2 targetSize)
        //{
        //    if (size.x <= targetSize.x && size.y <= targetSize.y)
        //        return size;

        //    var ratio = targetSize / size;
        //    if (ratio.x > ratio.y)
        //        size *= ratio.y;
        //    else
        //        size *= ratio.x;

        //    return size;
        //}



        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="value"></param>
        ///// <param name="max"></param>
        ///// <param name="min"></param>
        ///// <returns></returns>
        //public static T Clamp<T>(T value, T max, T min)
        //    where T : System.IComparable<T>
        //{
        //    T result = value;
        //    if (value.CompareTo(max) > 0)
        //    {
        //        result = max;
        //    }
        //    if (value.CompareTo(min) < 0)
        //    {
        //        result = min;
        //    }
        //    return result;
        //}

        //public static T Max<T>(T value, T max)
        //    where T : System.IComparable<T>
        //{
        //    T result = value;
        //    if (value.CompareTo(max) < 0)
        //    {
        //        result = max;
        //    }
        //    return result;
        //}

        //public static T Min<T>(T value, T min)
        //    where T : System.IComparable<T>
        //{
        //    T result = value;
        //    if (value.CompareTo(min) > 0)
        //    {
        //        result = min;
        //    }
        //    return result;
        //}

        //public static float Sqr(float number)
        //{
        //    return (float)Math.Pow(number, 2);
        //}

        public static int Log2(int x)
        {
            if (x <= 65536)
            {
                if (x <= 256)
                {
                    if (x <= 16)
                    {
                        if (x <= 4)
                        {
                            if (x <= 2)
                            {
                                if (x <= 1)
                                {
                                    return 0;
                                }
                                return 1;
                            }
                            return 2;
                        }
                        if (x <= 8)
                        {
                            return 3;
                        }
                        return 4;
                    }
                    if (x <= 64)
                    {
                        if (x <= 32)
                        {
                            return 5;
                        }
                        return 6;
                    }
                    if (x <= 128)
                    {
                        return 7;
                    }
                    return 8;
                }
                if (x <= 4096)
                {
                    if (x <= 1024)
                    {
                        if (x <= 512)
                        {
                            return 9;
                        }
                        return 10;
                    }
                    if (x <= 2048)
                    {
                        return 11;
                    }
                    return 12;
                }
                if (x <= 16384)
                {
                    if (x <= 8192)
                    {
                        return 13;
                    }
                    return 14;
                }
                if (x <= 32768)
                {
                    return 15;
                }
                return 16;
            }
            if (x <= 16777216)
            {
                if (x <= 1048576)
                {
                    if (x <= 262144)
                    {
                        if (x <= 131072)
                        {
                            return 17;
                        }
                        return 18;
                    }
                    if (x <= 524288)
                    {
                        return 19;
                    }
                    return 20;
                }
                if (x <= 4194304)
                {
                    if (x <= 2097152)
                    {
                        return 21;
                    }
                    return 22;
                }
                if (x <= 8388608)
                {
                    return 23;
                }
                return 24;
            }
            if (x <= 268435456)
            {
                if (x <= 67108864)
                {
                    if (x <= 33554432)
                    {
                        return 25;
                    }
                    return 26;
                }
                if (x <= 134217728)
                {
                    return 27;
                }
                return 28;
            }
            if (x <= 1073741824)
            {
                if (x <= 536870912)
                {
                    return 29;
                }
                return 30;
            }
            //	since int is unsigned it can never be higher than 2,147,483,647
            //	if( x <= 2147483648 )
            //		return	31;	
            //	return	32;	
            return 31;
        }

        ///// <summary>
        ///// Generates a value based on the Gaussian (normal) distribution function
        ///// with the given offset and scale parameters.
        ///// </summary>
        ///// <returns></returns>
        //public static float GaussianDistribution(float x, float offset, float scale)
        //{
        //    float nom = (float)System.Math.Exp(-Utility.Sqr(x - offset) / (2 * Utility.Sqr(scale)));
        //    float denom = scale * Utility.Sqrt(2 * Utility.PI);

        //    return nom / denom;
        //}

        //public static void RadixSort<T>(T[] a)
        //    where T : IRadixKey
        //{
        //    RadixSort(a, a.Length);
        //}

        //public static void RadixSort<T>(T[] a, int len)
        //    where T : IRadixKey
        //{
        //    // our helper array 
        //    T[] t = new T[len];

        //    // number of bits our group will be long 
        //    int r = 4; // try to set this also to 2, 8 or 16 to see if it is quicker or not 

        //    // number of bits of a C# int 
        //    int b = 32;

        //    // counting and prefix arrays
        //    // (note dimensions 2^r which is the number of all possible values of a r-bit number) 
        //    int[] count = new int[1 << r];
        //    int[] pref = new int[1 << r];

        //    // number of groups 
        //    int groups = (int)Math.Ceiling((double)b / (double)r);

        //    // the mask to identify groups 
        //    int mask = (1 << r) - 1;

        //    // the algorithm: 
        //    for (int c = 0, shift = 0; c < groups; c++, shift += r)
        //    {
        //        // reset count array 
        //        for (int j = 0; j < count.Length; j++)
        //            count[j] = 0;

        //        // counting elements of the c-th group 
        //        for (int i = 0; i < len; i++)
        //            count[(a[i].Key >> shift) & mask]++;

        //        // calculating prefixes 
        //        pref[0] = 0;
        //        for (int i = 1; i < count.Length; i++)
        //            pref[i] = pref[i - 1] + count[i - 1];

        //        // from a[] to t[] elements ordered by c-th group 
        //        for (int i = 0; i < len; i++)
        //            t[pref[(a[i].Key >> shift) & mask]++] = a[i];

        //        // a[]=t[] and start again until the last group 
        //        //t.CopyTo(a, 0);
        //        Array.Copy(t, a, len);
        //    }
        //    // a is sorted 
        //}

        //public static void RadixSort<T>(T[] a, int len, Func<T, int> key)
        //{
        //    // our helper array 
        //    T[] t = new T[len];

        //    // number of bits our group will be long 
        //    int r = 4; // try to set this also to 2, 8 or 16 to see if it is quicker or not 

        //    // number of bits of a C# int 
        //    int b = 32;

        //    // counting and prefix arrays
        //    // (note dimensions 2^r which is the number of all possible values of a r-bit number) 
        //    int[] count = new int[1 << r];
        //    int[] pref = new int[1 << r];

        //    // number of groups 
        //    int groups = (int)Math.Ceiling((double)b / (double)r);

        //    // the mask to identify groups 
        //    int mask = (1 << r) - 1;

        //    // the algorithm: 
        //    for (int c = 0, shift = 0; c < groups; c++, shift += r)
        //    {
        //        // reset count array 
        //        for (int j = 0; j < count.Length; j++)
        //            count[j] = 0;

        //        // counting elements of the c-th group 
        //        for (int i = 0; i < len; i++)
        //            count[(key(a[i]) >> shift) & mask]++;

        //        // calculating prefixes 
        //        pref[0] = 0;
        //        for (int i = 1; i < count.Length; i++)
        //            pref[i] = pref[i - 1] + count[i - 1];

        //        // from a[] to t[] elements ordered by c-th group 
        //        for (int i = 0; i < len; i++)
        //            t[pref[(key(a[i]) >> shift) & mask]++] = a[i];

        //        // a[]=t[] and start again until the last group 
        //        //t.CopyTo(a, 0);
        //        Array.Copy(t, a, len);
        //    }
        //    // a is sorted 
        //}


        //public static Colorf HSVToColor(float h, float s, float v)
        //{
        //    if (h == 0 && s == 0)
        //        return new Colorf(v, v, v);

        //    float c = s * v;
        //    float x = c * (1 - Math.Abs(h % 2 - 1));
        //    float m = v - c;

        //    if (h < 1) return new Colorf(c + m, x + m, m);
        //    else if (h < 2) return new Colorf(x + m, c + m, m);
        //    else if (h < 3) return new Colorf(m, c + m, x + m);
        //    else if (h < 4) return new Colorf(m, x + m, c + m);
        //    else if (h < 5) return new Colorf(x + m, m, c + m);
        //    else return new Colorf(c + m, m, x + m);
        //}

        //public static int NextPow(int x)
        //{
        //    int power = 1;
        //    while (power < x)
        //        power <<= 1;

        //    return power;
        //}

        public static bool Intersects(vec2 a1, vec2 a2, vec2 b1, vec2 b2)
        {
            var p = vec2.Zero;
            return Intersects(a1, a2, b1, b2, out p);
        }

        // a1 is line1 start, a2 is line1 end, b1 is line2 start, b2 is line2 end
        public static bool Intersects(vec2 a1, vec2 a2, vec2 b1, vec2 b2, out vec2 intersection)
        {
            intersection = vec2.Zero;

            vec2 b = a2 - a1;
            vec2 d = b2 - b1;
            float bDotDPerp = b.x * d.y - b.y * d.x;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            vec2 c = b1 - a1;
            float t = (c.x * d.y - c.y * d.x) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.x * b.y - c.y * b.x) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            intersection = a1 + t * b;

            return true;

        }
    }
}