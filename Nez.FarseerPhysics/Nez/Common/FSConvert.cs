using Microsoft.Xna.Framework;


namespace Nez.Farseer
{
	/// <summary>
	/// Convert units between display and simulation units
	/// </summary>
	public static class FSConvert
	{
		/// <summary>
		/// converts simulation (meters) to display (pixels)
		/// </summary>
		public static float simToDisplay = 100f;

		/// <summary>
		/// converts display (pixels) to simulation (meters)
		/// </summary>
		public static float displayToSim = 1 / simToDisplay;


		public static void setDisplayUnitToSimUnitRatio( float displayUnitsPerSimUnit )
		{
			simToDisplay = displayUnitsPerSimUnit;
			displayToSim = 1 / displayUnitsPerSimUnit;
		}

		public static float toDisplayUnits( float simUnits )
		{
			return simUnits * simToDisplay;
		}

		public static float toDisplayUnits( int simUnits )
		{
			return simUnits * simToDisplay;
		}

		public static vec2 toDisplayUnits( vec2 simUnits )
		{
			return simUnits * simToDisplay;
		}

		public static void toDisplayUnits( ref vec2 simUnits, out vec2 displayUnits )
		{
			vec2.Multiply( ref simUnits, simToDisplay, out displayUnits );
		}

		public static Vector3 toDisplayUnits( Vector3 simUnits )
		{
			return simUnits * simToDisplay;
		}

		public static vec2 toDisplayUnits( float x, float y )
		{
			return new vec2( x, y ) * simToDisplay;
		}

		public static void toDisplayUnits( float x, float y, out vec2 displayUnits )
		{
			displayUnits = vec2.Zero;
			displayUnits.x = x * simToDisplay;
			displayUnits.y = y * simToDisplay;
		}

		public static float toSimUnits( float displayUnits )
		{
			return displayUnits * displayToSim;
		}

		public static float toSimUnits( double displayUnits )
		{
			return (float)displayUnits * displayToSim;
		}

		public static float toSimUnits( int displayUnits )
		{
			return displayUnits * displayToSim;
		}

		public static vec2 toSimUnits( vec2 displayUnits )
		{
			return displayUnits * displayToSim;
		}

		public static Vector3 toSimUnits( Vector3 displayUnits )
		{
			return displayUnits * displayToSim;
		}

		public static void toSimUnits( ref vec2 displayUnits, out vec2 simUnits )
		{
			vec2.Multiply( ref displayUnits, displayToSim, out simUnits );
		}

		public static vec2 toSimUnits( float x, float y )
		{
			return new vec2( x, y ) * displayToSim;
		}

		public static vec2 toSimUnits( double x, double y )
		{
			return new vec2( (float)x, (float)y ) * displayToSim;
		}

		public static void toSimUnits( float x, float y, out vec2 simUnits )
		{
			simUnits = vec2.Zero;
			simUnits.x = x * displayToSim;
			simUnits.y = y * displayToSim;
		}

	}
}