﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;


namespace Nez.Common.TextureTools
{
	// User contribution from Sickbattery aka David Reschke.

	/// <summary>
	/// The detection type affects the resulting polygon data.
	/// </summary>
	public enum VerticesDetectionType
	{
		/// <summary>
		/// Holes are integrated into the main polygon.
		/// </summary>
		Integrated = 0,

		/// <summary>
		/// The data of the main polygon and hole polygons is returned separately.
		/// </summary>
		Separated = 1
	}

	public sealed class TextureConverter
	{
		#region Properties/Fields

		/// <summary>
		/// Get or set the polygon detection type.
		/// </summary>
		public VerticesDetectionType polygonDetectionType;

		/// <summary>
		/// Will detect texture 'holes' if set to true. Slows down the detection. Default is false.
		/// </summary>
		public bool holeDetection;

		/// <summary>
		/// Will detect texture multiple 'solid' isles if set to true. Slows down the detection. Default is false.
		/// </summary>
		public bool multipartDetection;

		/// <summary>
		/// Will optimize the vertex positions along the interpolated normal between two edges about a half pixel (post processing). Default is false.
		/// </summary>
		public bool pixelOffsetOptimization;

		/// <summary>
		/// Can be used for scaling.
		/// </summary>
		public Matrix transform = Matrix.Identity;

		/// <summary>
		/// Alpha (coverage) tolerance. Default is 20: Every pixel with a coverage value equal or greater to 20 will be counts as solid.
		/// </summary>
		public byte alphaTolerance
		{
			get { return (byte)( _alphaTolerance >> 24 ); }
			set { _alphaTolerance = (uint)value << 24; }
		}

		/// <summary>
		/// Default is 1.5f.
		/// </summary>
		public float hullTolerance
		{
			get { return _hullTolerance; }
			set
			{
				if( value > 4f )
					_hullTolerance = 4f;
				else if( value < 0.9f )
					_hullTolerance = 0.9f;
				else
					_hullTolerance = value;
			}
		}

		const int closepixelsLength = 8;

		/// <summary>
		/// This array is ment to be readonly.
		/// It's not because it is accessed very frequently.
		/// </summary>
		static int[,] _closePixels = new[,] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 } };

		uint[] _data;
		int _dataLength;
		int _width;
		int _height;

		uint _alphaTolerance;
		float _hullTolerance;

		#endregion


		#region Constructors

		public TextureConverter()
		{
			Initialize( null, null, null, null, null, null, null, null );
		}

		public TextureConverter( byte? alphaTolerance, float? hullTolerance,
			bool? holeDetection, bool? multipartDetection, bool? pixelOffsetOptimization, Matrix? transform )
		{
			Initialize( null, null, alphaTolerance, hullTolerance, holeDetection,
				multipartDetection, pixelOffsetOptimization, transform );
		}

		public TextureConverter( uint[] data, int width )
		{
			Initialize( data, width, null, null, null, null, null, null );
		}

		public TextureConverter( uint[] data, int width, byte? alphaTolerance,
			float? hullTolerance, bool? holeDetection, bool? multipartDetection,
			bool? pixelOffsetOptimization, Matrix? transform )
		{
			Initialize( data, width, alphaTolerance, hullTolerance, holeDetection,
				multipartDetection, pixelOffsetOptimization, transform );
		}

		#endregion


		void Initialize( uint[] data, int? width, byte? alphaTolerance, float? hullTolerance, bool? holeDetection, bool? multipartDetection,
			bool? pixelOffsetOptimization, Matrix? transform )
		{
			if( data != null && !width.HasValue )
				throw new ArgumentNullException( nameof( width ), "'width' can't be null if 'data' is set." );

			if( data == null && width.HasValue )
				throw new ArgumentNullException( nameof( data ), "'data' can't be null if 'width' is set." );

			if( data != null && width.HasValue )
				setTextureData( data, width.Value );

			if( alphaTolerance.HasValue )
				this.alphaTolerance = alphaTolerance.Value;
			else
				this.alphaTolerance = 20;

			if( hullTolerance.HasValue )
				this.hullTolerance = hullTolerance.Value;
			else
				this.hullTolerance = 1.5f;

			if( holeDetection.HasValue )
				this.holeDetection = holeDetection.Value;
			else
				this.holeDetection = false;

			if( multipartDetection.HasValue )
				this.multipartDetection = multipartDetection.Value;
			else
				this.multipartDetection = false;

			if( pixelOffsetOptimization.HasValue )
				this.pixelOffsetOptimization = pixelOffsetOptimization.Value;
			else
				this.pixelOffsetOptimization = false;

			if( transform.HasValue )
				this.transform = transform.Value;
			else
				this.transform = Matrix.Identity;
		}


		void setTextureData( uint[] data, int width )
		{
			if( data == null )
				throw new ArgumentNullException( nameof( data ), "'data' can't be null." );

			if( data.Length < 4 )
				throw new ArgumentOutOfRangeException( nameof( data ), "'data' length can't be less then 4. Your texture must be at least 2 x 2 pixels in size." );

			if( width < 2 )
				throw new ArgumentOutOfRangeException( nameof( width ), "'width' can't be less then 2. Your texture must be at least 2 x 2 pixels in size." );

			if( data.Length % width != 0 )
				throw new ArgumentException( "'width' has an invalid value." );

			_data = data;
			_dataLength = _data.Length;
			_width = width;
			_height = _dataLength / width;
		}

		/// <summary>
		/// Detects the vertices of the supplied texture data. (PolygonDetectionType.Integrated)
		/// </summary>
		/// <param name="data">The texture data.</param>
		/// <param name="width">The texture width.</param>
		/// <returns></returns>
		public static Vertices detectVertices( uint[] data, int width )
		{
			var tc = new TextureConverter( data, width );
			var detectedVerticesList = tc.detectVertices();

			return detectedVerticesList[0];
		}

		/// <summary>
		/// Detects the vertices of the supplied texture data.
		/// </summary>
		/// <param name="data">The texture data.</param>
		/// <param name="width">The texture width.</param>
		/// <param name="holeDetection">if set to <c>true</c> it will perform hole detection.</param>
		/// <returns></returns>
		public static Vertices detectVertices( uint[] data, int width, bool holeDetection )
		{
			var tc = new TextureConverter( data, width )
			{
				holeDetection = holeDetection
			};

			var detectedVerticesList = tc.detectVertices();

			return detectedVerticesList[0];
		}

		/// <summary>
		/// Detects the vertices of the supplied texture data.
		/// </summary>
		/// <param name="data">The texture data.</param>
		/// <param name="width">The texture width.</param>
		/// <param name="holeDetection">if set to <c>true</c> it will perform hole detection.</param>
		/// <param name="hullTolerance">The hull tolerance.</param>
		/// <param name="alphaTolerance">The alpha tolerance.</param>
		/// <param name="multiPartDetection">if set to <c>true</c> it will perform multi part detection.</param>
		/// <returns></returns>
		public static List<Vertices> detectVertices( uint[] data, int width, float hullTolerance, byte alphaTolerance, bool multiPartDetection, bool holeDetection )
		{
			var tc = new TextureConverter( data, width )
			{
				hullTolerance = hullTolerance,
				alphaTolerance = alphaTolerance,
				multipartDetection = multiPartDetection,
				holeDetection = holeDetection
			};

			var detectedVerticesList = tc.detectVertices();
			var result = new List<Vertices>();

			for( int i = 0; i < detectedVerticesList.Count; i++ )
				result.Add( detectedVerticesList[i] );

			return result;
		}

		public List<Vertices> detectVertices()
		{
			#region Check TextureConverter setup.

			if( _data == null )
				throw new Exception( "'_data' can't be null. You have to use SetTextureData(uint[] data, int width) before calling this method." );

			if( _data.Length < 4 )
				throw new Exception( "'_data' length can't be less then 4. Your texture must be at least 2 x 2 pixels in size. " +
					"You have to use SetTextureData(uint[] data, int width) before calling this method." );

			if( _width < 2 )
				throw new Exception( "'_width' can't be less then 2. Your texture must be at least 2 x 2 pixels in size. " +
					"You have to use SetTextureData(uint[] data, int width) before calling this method." );

			if( _data.Length % _width != 0 )
				throw new Exception( "'_width' has an invalid value. You have to use SetTextureData(uint[] data, int width) before calling this method." );

			#endregion

			var detectedPolygons = new List<Vertices>();

			vec2? holeEntrance = null;
			vec2? polygonEntrance = null;

			var blackList = new List<vec2>();

			bool searchOn;
			do
			{
				Vertices polygon;
				if( detectedPolygons.Count == 0 )
				{
					// First pass / single polygon
					polygon = new Vertices( createSimplePolygon( vec2.Zero, vec2.Zero ) );

					if( polygon.Count > 2 )
						polygonEntrance = getTopMostVertex( polygon );
				}
				else if( polygonEntrance.HasValue )
				{
					// Multi pass / multiple polygons
					polygon = new Vertices( createSimplePolygon( polygonEntrance.Value, new vec2( polygonEntrance.Value.x - 1f, polygonEntrance.Value.y ) ) );
				}
				else
					break;

				searchOn = false;


				if( polygon.Count > 2 )
				{
					if( holeDetection )
					{
						do
						{
							holeEntrance = searchHoleEntrance( polygon, holeEntrance );

							if( holeEntrance.HasValue )
							{
								if( !blackList.Contains( holeEntrance.Value ) )
								{
									blackList.Add( holeEntrance.Value );
									var holePolygon = createSimplePolygon( holeEntrance.Value,
																			   new vec2( holeEntrance.Value.x + 1, holeEntrance.Value.y ) );

									if( holePolygon != null && holePolygon.Count > 2 )
									{
										switch( polygonDetectionType )
										{
											case VerticesDetectionType.Integrated:

												// Add first hole polygon vertex to close the hole polygon.
												holePolygon.Add( holePolygon[0] );

												int vertex1Index, vertex2Index;
												if( splitPolygonEdge( polygon, holeEntrance.Value, out vertex1Index, out vertex2Index ) )
													polygon.InsertRange( vertex2Index, holePolygon );

												break;

											case VerticesDetectionType.Separated:
												if( polygon.holes == null )
													polygon.holes = new List<Vertices>();

												polygon.holes.Add( holePolygon );
												break;
										}
									}
								}
								else
									break;
							}
							else
								break;
						}
						while( true );
					}

					detectedPolygons.Add( polygon );
				}

				if( multipartDetection || polygon.Count <= 2 )
				{
					if( searchNextHullEntrance( detectedPolygons, polygonEntrance.Value, out polygonEntrance ) )
						searchOn = true;
				}
			}
			while( searchOn );

			if( detectedPolygons == null || ( detectedPolygons != null && detectedPolygons.Count == 0 ) )
				throw new Exception( "Couldn't detect any vertices." );

			// Post processing.
			if( polygonDetectionType == VerticesDetectionType.Separated ) // Only when VerticesDetectionType.Separated? -> Recheck.
				applyTriangulationCompatibleWinding( ref detectedPolygons );

			if( transform != Matrix.Identity )
				applyTransform( ref detectedPolygons );

			return detectedPolygons;
		}

		void applyTriangulationCompatibleWinding( ref List<Vertices> detectedPolygons )
		{
			for( int i = 0; i < detectedPolygons.Count; i++ )
			{
				detectedPolygons[i].Reverse();

				if( detectedPolygons[i].holes != null && detectedPolygons[i].holes.Count > 0 )
				{
					for( int j = 0; j < detectedPolygons[i].holes.Count; j++ )
						detectedPolygons[i].holes[j].Reverse();
				}
			}
		}

		void applyTransform( ref List<Vertices> detectedPolygons )
		{
			for( int i = 0; i < detectedPolygons.Count; i++ )
				detectedPolygons[i].transform( ref transform );
		}


		#region Data[] functions

		int _tempIsSolidX;
		int _tempIsSolidY;

		public bool isSolid( ref vec2 v )
		{
			_tempIsSolidX = (int)v.x;
			_tempIsSolidY = (int)v.y;

			if( _tempIsSolidX >= 0 && _tempIsSolidX < _width && _tempIsSolidY >= 0 && _tempIsSolidY < _height )
				return ( _data[_tempIsSolidX + _tempIsSolidY * _width] >= _alphaTolerance );
			//return ((_data[_tempIsSolidX + _tempIsSolidY * _width] & 0xFF000000) >= _alphaTolerance);

			return false;
		}

		public bool isSolid( ref int x, ref int y )
		{
			if( x >= 0 && x < _width && y >= 0 && y < _height )
				return ( _data[x + y * _width] >= _alphaTolerance );
			//return ((_data[x + y * _width] & 0xFF000000) >= _alphaTolerance);

			return false;
		}

		public bool isSolid( ref int index )
		{
			if( index >= 0 && index < _dataLength )
				return ( _data[index] >= _alphaTolerance );
			//return ((_data[index] & 0xFF000000) >= _alphaTolerance);

			return false;
		}

		public bool inBounds( ref vec2 coord )
		{
			return ( coord.x >= 0f && coord.x < _width && coord.y >= 0f && coord.y < _height );
		}

		#endregion


		/// <summary>
		/// Function to search for an entrance point of a hole in a polygon. It searches the polygon from top to bottom between the polygon edges.
		/// </summary>
		/// <param name="polygon">The polygon to search in.</param>
		/// <param name="lastHoleEntrance">The last entrance point.</param>
		/// <returns>The next holes entrance point. Null if ther are no holes.</returns>
		vec2? searchHoleEntrance( Vertices polygon, vec2? lastHoleEntrance )
		{
			if( polygon == null )
				throw new ArgumentNullException( "'polygon' can't be null." );

			if( polygon.Count < 3 )
				throw new ArgumentException( "'polygon.MainPolygon.Count' can't be less then 3." );


			List<float> xCoords;
			vec2? entrance;

			int startY;
			int endY;

			int lastSolid = 0;
			bool foundSolid;
			bool foundTransparent;

			// Set start y coordinate.
			if( lastHoleEntrance.HasValue )
			{
				// We need the y coordinate only.
				startY = (int)lastHoleEntrance.Value.y;
			}
			else
			{
				// Start from the top of the polygon if last entrance == null.
				startY = (int)getTopMostCoord( polygon );
			}

			// Set the end y coordinate.
			endY = (int)getBottomMostCoord( polygon );

			if( startY > 0 && startY < _height && endY > 0 && endY < _height )
			{
				// go from top to bottom of the polygon
				for( int y = startY; y <= endY; y++ )
				{
					// get x-coord of every polygon edge which crosses y
					xCoords = searchCrossingEdges( polygon, y );

					// We need an even number of crossing edges. 
					// It's always a pair of start and end edge: nothing | polygon | hole | polygon | nothing ...
					// If it's not then don't bother, it's probably a peak ...
					// ...which should be filtered out by SearchCrossingEdges() anyway.
					if( xCoords.Count > 1 && xCoords.Count % 2 == 0 )
					{
						// Ok, this is short, but probably a little bit confusing.
						// This part searches from left to right between the edges inside the polygon.
						// The problem: We are using the polygon data to search in the texture data.
						// That's simply not accurate, but necessary because of performance.
						for( int i = 0; i < xCoords.Count; i += 2 )
						{
							foundSolid = false;
							foundTransparent = false;

							// We search between the edges inside the polygon.
							for( int x = (int)xCoords[i]; x <= (int)xCoords[i + 1]; x++ )
							{
								// First pass: IsSolid might return false.
								// In that case the polygon edge doesn't lie on the texture's solid pixel, because of the hull tolearance.
								// If the edge lies before the first solid pixel then we need to skip our transparent pixel finds.

								// The algorithm starts to search for a relevant transparent pixel (which indicates a possible hole) 
								// after it has found a solid pixel.

								// After we've found a solid and a transparent pixel (a hole's left edge) 
								// we search for a solid pixel again (a hole's right edge).
								// When found the distance of that coodrinate has to be greater then the hull tolerance.

								if( isSolid( ref x, ref y ) )
								{
									if( !foundTransparent )
									{
										foundSolid = true;
										lastSolid = x;
									}

									if( foundSolid && foundTransparent )
									{
										entrance = new vec2( lastSolid, y );

										if( distanceToHullAcceptable( polygon, entrance.Value, true ) )
											return entrance;

										entrance = null;
										break;
									}
								}
								else
								{
									if( foundSolid )
										foundTransparent = true;
								}
							}
						}
					}
					else
					{
						if( xCoords.Count % 2 == 0 )
							Debug.WriteLine( "SearchCrossingEdges() % 2 != 0" );
					}
				}
			}

			return null;
		}

		bool distanceToHullAcceptableHoles( Vertices polygon, vec2 point, bool higherDetail )
		{
			if( polygon == null )
				throw new ArgumentNullException( nameof( polygon ), "'polygon' can't be null." );

			if( polygon.Count < 3 )
				throw new ArgumentException( "'polygon.MainPolygon.Count' can't be less then 3." );

			// Check the distance to main polygon.
			if( distanceToHullAcceptable( polygon, point, higherDetail ) )
			{
				if( polygon.holes != null )
				{
					for( int i = 0; i < polygon.holes.Count; i++ )
					{
						// If there is one distance not acceptable then return false.
						if( !distanceToHullAcceptable( polygon.holes[i], point, higherDetail ) )
							return false;
					}
				}

				// All distances are larger then _hullTolerance.
				return true;
			}

			// Default to false.
			return false;
		}

		bool distanceToHullAcceptable( Vertices polygon, vec2 point, bool higherDetail )
		{
			if( polygon == null )
				throw new ArgumentNullException( nameof( polygon ), "'polygon' can't be null." );

			if( polygon.Count < 3 )
				throw new ArgumentException( "'polygon.Count' can't be less then 3." );


			vec2 edgeVertex2 = polygon[polygon.Count - 1];
			vec2 edgeVertex1;

			if(higherDetail)
			{
				for( int i = 0; i < polygon.Count; i++ )
				{
					edgeVertex1 = polygon[i];

					if( LineTools.distanceBetweenPointAndLineSegment( ref point, ref edgeVertex1, ref edgeVertex2 ) <= _hullTolerance || vec2.Distance( point, edgeVertex1 ) <= _hullTolerance )
						return false;

					edgeVertex2 = polygon[i];
				}

				return true;
			}
			else
			{
				for( int i = 0; i < polygon.Count; i++ )
				{
					edgeVertex1 = polygon[i];

					if( LineTools.distanceBetweenPointAndLineSegment( ref point, ref edgeVertex1, ref edgeVertex2 ) <= _hullTolerance )
						return false;

					edgeVertex2 = polygon[i];
				}

				return true;
			}
		}

		bool inPolygon( Vertices polygon, vec2 point )
		{
			bool inPolygon = !distanceToHullAcceptableHoles( polygon, point, true );

			if( !inPolygon )
			{
				List<float> xCoords = searchCrossingEdgesHoles( polygon, (int)point.y );

				if( xCoords.Count > 0 && xCoords.Count % 2 == 0 )
				{
					for( int i = 0; i < xCoords.Count; i += 2 )
					{
						if( xCoords[i] <= point.x && xCoords[i + 1] >= point.x )
							return true;
					}
				}

				return false;
			}

			return true;
		}

		vec2? getTopMostVertex( Vertices vertices )
		{
			float topMostValue = float.MaxValue;
			vec2? topMost = null;

			for( int i = 0; i < vertices.Count; i++ )
			{
				if( topMostValue > vertices[i].y )
				{
					topMostValue = vertices[i].y;
					topMost = vertices[i];
				}
			}

			return topMost;
		}

		float getTopMostCoord( Vertices vertices )
		{
			float returnValue = float.MaxValue;

			for( int i = 0; i < vertices.Count; i++ )
			{
				if( returnValue > vertices[i].y )
				{
					returnValue = vertices[i].y;
				}
			}

			return returnValue;
		}

		float getBottomMostCoord( Vertices vertices )
		{
			float returnValue = float.MinValue;

			for( int i = 0; i < vertices.Count; i++ )
			{
				if( returnValue < vertices[i].y )
				{
					returnValue = vertices[i].y;
				}
			}

			return returnValue;
		}

		List<float> searchCrossingEdgesHoles( Vertices polygon, int y )
		{
			if( polygon == null )
				throw new ArgumentNullException( nameof( polygon ), "'polygon' can't be null." );

			if( polygon.Count < 3 )
				throw new ArgumentException( "'polygon.MainPolygon.Count' can't be less then 3." );

			List<float> result = searchCrossingEdges( polygon, y );

			if( polygon.holes != null )
			{
				for( int i = 0; i < polygon.holes.Count; i++ )
				{
					result.AddRange( searchCrossingEdges( polygon.holes[i], y ) );
				}
			}

			result.Sort();
			return result;
		}

		/// <summary>
		/// Searches the polygon for the x coordinates of the edges that cross the specified y coordinate.
		/// </summary>
		/// <param name="polygon">Polygon to search in.</param>
		/// <param name="y">Y coordinate to check for edges.</param>
		/// <returns>Descending sorted list of x coordinates of edges that cross the specified y coordinate.</returns>
		List<float> searchCrossingEdges( Vertices polygon, int y )
		{
			// sick-o-note:
			// Used to search the x coordinates of edges in the polygon for a specific y coordinate.
			// (Usualy comming from the texture data, that's why it's an int and not a float.)

			var edges = new List<float>();

			// current edge
			vec2 slope;
			vec2 vertex1;    // i
			vec2 vertex2;    // i - 1

			// next edge
			vec2 nextSlope;
			vec2 nextVertex; // i + 1

			bool addFind;

			if( polygon.Count > 2 )
			{
				// There is a gap between the last and the first vertex in the vertex list.
				// We will bridge that by setting the last vertex (vertex2) to the last 
				// vertex in the list.
				vertex2 = polygon[polygon.Count - 1];

				// We are moving along the polygon edges.
				for( int i = 0; i < polygon.Count; i++ )
				{
					vertex1 = polygon[i];

					// Approx. check if the edge crosses our y coord.
					if( ( vertex1.y >= y && vertex2.y <= y ) ||
						( vertex1.y <= y && vertex2.y >= y ) )
					{
						// Ignore edges that are parallel to y.
						if( vertex1.y != vertex2.y )
						{
							addFind = true;
							slope = vertex2 - vertex1;

							// Special threatment for edges that end at the y coord.
							if( vertex1.y == y )
							{
								// Create preview of the next edge.
								nextVertex = polygon[( i + 1 ) % polygon.Count];
								nextSlope = vertex1 - nextVertex;

								// Ignore peaks. 
								// If thwo edges are aligned like this: /\ and the y coordinate lies on the top,
								// then we get the same x coord twice and we don't need that.
								if( slope.y > 0 )
									addFind = ( nextSlope.y <= 0 );
								else
									addFind = ( nextSlope.y >= 0 );
							}

							if( addFind )
								edges.Add( ( y - vertex1.y ) / slope.y * slope.x + vertex1.x ); // Calculate and add the x coord.
						}
					}

					// vertex1 becomes vertex2 :).
					vertex2 = vertex1;
				}
			}

			edges.Sort();
			return edges;
		}

		bool splitPolygonEdge( Vertices polygon, vec2 coordInsideThePolygon, out int vertex1Index, out int vertex2Index )
		{
			vec2 slope;
			int nearestEdgeVertex1Index = 0;
			int nearestEdgeVertex2Index = 0;
			bool edgeFound = false;

			float shortestDistance = float.MaxValue;

			bool edgeCoordFound = false;
			vec2 foundEdgeCoord = vec2.Zero;

			List<float> xCoords = searchCrossingEdges( polygon, (int)coordInsideThePolygon.y );

			vertex1Index = 0;
			vertex2Index = 0;

			foundEdgeCoord.y = coordInsideThePolygon.y;

			if( xCoords != null && xCoords.Count > 1 && xCoords.Count % 2 == 0 )
			{
				float distance;
				for( int i = 0; i < xCoords.Count; i++ )
				{
					if( xCoords[i] < coordInsideThePolygon.x )
					{
						distance = coordInsideThePolygon.x - xCoords[i];

						if( distance < shortestDistance )
						{
							shortestDistance = distance;
							foundEdgeCoord.x = xCoords[i];

							edgeCoordFound = true;
						}
					}
				}

				if( edgeCoordFound )
				{
					shortestDistance = float.MaxValue;

					int edgeVertex2Index = polygon.Count - 1;

					int edgeVertex1Index;
					for( edgeVertex1Index = 0; edgeVertex1Index < polygon.Count; edgeVertex1Index++ )
					{
						vec2 tempVector1 = polygon[edgeVertex1Index];
						vec2 tempVector2 = polygon[edgeVertex2Index];
						distance = LineTools.distanceBetweenPointAndLineSegment( ref foundEdgeCoord,
																				ref tempVector1, ref tempVector2 );
						if( distance < shortestDistance )
						{
							shortestDistance = distance;

							nearestEdgeVertex1Index = edgeVertex1Index;
							nearestEdgeVertex2Index = edgeVertex2Index;

							edgeFound = true;
						}

						edgeVertex2Index = edgeVertex1Index;
					}

					if( edgeFound )
					{
						slope = polygon[nearestEdgeVertex2Index] - polygon[nearestEdgeVertex1Index];
                        slope.Normalize();
						//Nez.Vector2Ext.normalize( ref slope );

						var tempVector = polygon[nearestEdgeVertex1Index];
						distance = vec2.Distance( tempVector, foundEdgeCoord );

						vertex1Index = nearestEdgeVertex1Index;
						vertex2Index = nearestEdgeVertex1Index + 1;

						polygon.Insert( nearestEdgeVertex1Index, distance * slope + polygon[vertex1Index] );
						polygon.Insert( nearestEdgeVertex1Index, distance * slope + polygon[vertex2Index] );

						return true;
					}
				}
			}

			return false;
		}

		Vertices createSimplePolygon( vec2 entrance, vec2 last )
		{
			var entranceFound = false;
			var endOfHull = false;

			var polygon = new Vertices( 32 );
			var hullArea = new Vertices( 32 );
			var endOfHullArea = new Vertices( 32 );

			var current = vec2.Zero;

			#region Entrance check

			// Get the entrance point. //todo: alle möglichkeiten testen
			if( entrance == vec2.Zero || !inBounds( ref entrance ) )
			{
				entranceFound = searchHullEntrance( out entrance );

				if( entranceFound )
				{
					current = new vec2( entrance.x - 1f, entrance.y );
				}
			}
			else
			{
				if( isSolid( ref entrance ) )
				{
					if( isNearPixel( ref entrance, ref last ) )
					{
						current = last;
						entranceFound = true;
					}
					else
					{
						vec2 temp;
						if( searchNearPixels( false, ref entrance, out temp ) )
						{
							current = temp;
							entranceFound = true;
						}
						else
						{
							entranceFound = false;
						}
					}
				}
			}

			#endregion

			if( entranceFound )
			{
				polygon.Add( entrance );
				hullArea.Add( entrance );

				vec2 next = entrance;

				do
				{
					// Search in the pre vision list for an outstanding point.
					vec2 outstanding;
					if( searchForOutstandingVertex( hullArea, out outstanding ) )
					{
						if( endOfHull )
						{
							// We have found the next pixel, but is it on the last bit of the hull?
							if( endOfHullArea.Contains( outstanding ) )
							{
								// Indeed.
								polygon.Add( outstanding );
							}

							// That's enough, quit.
							break;
						}

						// Add it and remove all vertices that don't matter anymore
						// (all the vertices before the outstanding).
						polygon.Add( outstanding );
						hullArea.RemoveRange( 0, hullArea.IndexOf( outstanding ) );
					}

					// Last point gets current and current gets next. Our little spider is moving forward on the hull ;).
					last = current;
					current = next;

					// Get the next point on hull.
					if( getNextHullPoint( ref last, ref current, out next ) )
					{
						// Add the vertex to a hull pre vision list.
						hullArea.Add( next );
					}
					else
					{
						// Quit
						break;
					}

					if( next == entrance && !endOfHull )
					{
						// It's the last bit of the hull, search on and exit at next found vertex.
						endOfHull = true;
						endOfHullArea.AddRange( hullArea );

						// We don't want the last vertex to be the same as the first one, because it causes the triangulation code to crash.
						if( endOfHullArea.Contains( entrance ) )
							endOfHullArea.Remove( entrance );
					}

				} while( true );
			}

			return polygon;
		}

		bool searchNearPixels( bool searchingForSolidPixel, ref vec2 current, out vec2 foundPixel )
		{
			for( int i = 0; i < closepixelsLength; i++ )
			{
				int x = (int)current.x + _closePixels[i, 0];
				int y = (int)current.y + _closePixels[i, 1];

				if( !searchingForSolidPixel ^ isSolid( ref x, ref y ) )
				{
					foundPixel = new vec2( x, y );
					return true;
				}
			}

			// Nothing found.
			foundPixel = vec2.Zero;
			return false;
		}

		bool isNearPixel( ref vec2 current, ref vec2 near )
		{
			for( int i = 0; i < closepixelsLength; i++ )
			{
				int x = (int)current.x + _closePixels[i, 0];
				int y = (int)current.y + _closePixels[i, 1];

				if( x >= 0 && x <= _width && y >= 0 && y <= _height )
				{
					if( x == (int)near.x && y == (int)near.y )
					{
						return true;
					}
				}
			}

			return false;
		}

		bool searchHullEntrance( out vec2 entrance )
		{
			// Search for first solid pixel.
			for( int y = 0; y <= _height; y++ )
			{
				for( int x = 0; x <= _width; x++ )
				{
					if( isSolid( ref x, ref y ) )
					{
						entrance = new vec2( x, y );
						return true;
					}
				}
			}

			// If there are no solid pixels.
			entrance = vec2.Zero;
			return false;
		}

		/// <summary>
		/// Searches for the next shape.
		/// </summary>
		/// <param name="detectedPolygons">Already detected polygons.</param>
		/// <param name="start">Search start coordinate.</param>
		/// <param name="entrance">Returns the found entrance coordinate. Null if no other shapes found.</param>
		/// <returns>True if a new shape was found.</returns>
		bool searchNextHullEntrance( List<Vertices> detectedPolygons, vec2 start, out vec2? entrance )
		{
			int x;

			bool foundTransparent = false;
			bool isInPolygon = false;

			for( int i = (int)start.x + (int)start.y * _width; i <= _dataLength; i++ )
			{
				if( isSolid( ref i ) )
				{
					if( foundTransparent )
					{
						x = i % _width;
						entrance = new vec2( x, ( i - x ) / (float)_width );

						isInPolygon = false;
						for( int polygonIdx = 0; polygonIdx < detectedPolygons.Count; polygonIdx++ )
						{
							if( inPolygon( detectedPolygons[polygonIdx], entrance.Value ) )
							{
								isInPolygon = true;
								break;
							}
						}

						if( isInPolygon )
							foundTransparent = false;
						else
							return true;
					}
				}
				else
					foundTransparent = true;
			}

			entrance = null;
			return false;
		}

		bool getNextHullPoint( ref vec2 last, ref vec2 current, out vec2 next )
		{
			int x;
			int y;

			int indexOfFirstPixelToCheck = getIndexOfFirstPixelToCheck( ref last, ref current );
			int indexOfPixelToCheck;

			for( int i = 0; i < closepixelsLength; i++ )
			{
				indexOfPixelToCheck = ( indexOfFirstPixelToCheck + i ) % closepixelsLength;

				x = (int)current.x + _closePixels[indexOfPixelToCheck, 0];
				y = (int)current.y + _closePixels[indexOfPixelToCheck, 1];

				if( x >= 0 && x < _width && y >= 0 && y <= _height )
				{
					if( isSolid( ref x, ref y ) )
					{
						next = new vec2( x, y );
						return true;
					}
				}
			}

			next = vec2.Zero;
			return false;
		}

		bool searchForOutstandingVertex( Vertices hullArea, out vec2 outstanding )
		{
			vec2 outstandingResult = vec2.Zero;
			bool found = false;

			if( hullArea.Count > 2 )
			{
				int hullAreaLastPoint = hullArea.Count - 1;

				vec2 tempVector1;
				vec2 tempVector2 = hullArea[0];
				vec2 tempVector3 = hullArea[hullAreaLastPoint];

				// Search between the first and last hull point.
				for( int i = 1; i < hullAreaLastPoint; i++ )
				{
					tempVector1 = hullArea[i];

					// Check if the distance is over the one that's tolerable.
					if( LineTools.distanceBetweenPointAndLineSegment( ref tempVector1, ref tempVector2, ref tempVector3 ) >= _hullTolerance )
					{
						outstandingResult = hullArea[i];
						found = true;
						break;
					}
				}
			}

			outstanding = outstandingResult;
			return found;
		}

		int getIndexOfFirstPixelToCheck( ref vec2 last, ref vec2 current )
		{
			// .: pixel
			// l: last position
			// c: current position
			// f: first pixel for next search

			// f . .
			// l c .
			// . . .

			//Calculate in which direction the last move went and decide over the next pixel to check.
			switch( (int)( current.x - last.x ) )
			{
				case 1:
					switch( (int)( current.y - last.y ) )
					{
						case 1:
							return 1;

						case 0:
							return 0;

						case -1:
							return 7;
					}
					break;

				case 0:
					switch( (int)( current.y - last.y ) )
					{
						case 1:
							return 2;

						case -1:
							return 6;
					}
					break;

				case -1:
					switch( (int)( current.y - last.y ) )
					{
						case 1:
							return 3;

						case 0:
							return 4;

						case -1:
							return 5;
					}
					break;
			}

			return 0;
		}
	
	}
}