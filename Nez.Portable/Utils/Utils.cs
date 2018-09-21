#if FEATURE_UTILS
using System;
using System.Text;
using Microsoft.Xna.Framework;

namespace Atma
{
	/// <summary>
	/// utility methods that don't yet have a proper home that makes sense
	/// </summary>
	public static class Utils
	{
		public static string randomString( int size = 38 )
		{
			var builder = new StringBuilder();

			char ch;
			for( int i = 0; i < size; i++ )
			{
				ch = Convert.ToChar( Convert.ToInt32( Math.Floor( 26 * Random.nextFloat() + 65 ) ) );
				builder.Append( ch );
			}

			return builder.ToString();
		}


		/// <summary>
		/// swaps the two object types
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void swap<T>( ref T first, ref T second )
		{
			T temp = first;
			first = second;
			second = temp;
		}

		/// <summary>
		/// converts a direction to a screen-space vector
		/// </summary>
		public static vec2 directionToVector( Direction direction )
		{
			switch (direction) {
                case Direction.Up:
                    return new vec2(0, -1);
                case Direction.Right:
                    return new vec2(1, 0);
                case Direction.Down:
                    return new vec2(0, 1);
                case Direction.Left:
                    return new vec2(-1, 0);
                default:
                    return vec2.Zero;
            }
		}

	}
}

#endif
