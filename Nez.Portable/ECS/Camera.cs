﻿using System;
using Microsoft.Xna.Framework;
#if !FNA
using Microsoft.Xna.Framework.Input.Touch;
#endif


namespace Nez
{
	public class Camera : Component
	{
		struct CameraInset
		{
			internal float left;
			internal float right;
			internal float top;
			internal float bottom;
		}

		
		#region Fields and Properties

		#region 3D Camera Fields

		/// <summary>
		/// z-position of the 3D camera projections. Affects the fov greatly. Lower values make the objects appear very long in the z-direction.
		/// </summary>
		public float positionZ3D = 2000f;

		/// <summary>
		/// near clip plane of the 3D camera projection
		/// </summary>
		public float nearClipPlane3D = 0.0001f;

		/// <summary>
		/// far clip plane of the 3D camera projection
		/// </summary>
		public float farClipPlane3D = 5000f;

		#endregion


		/// <summary>
		/// shortcut to entity.transform.position
		/// </summary>
		/// <value>The position.</value>
		public vec2 position
		{
			get { return entity.transform.position; }
			set { entity.transform.position = value; }
		}

		/// <summary>
		/// shortcut to entity.transform.rotation
		/// </summary>
		/// <value>The rotation.</value>
		public float rotation
		{
			get { return entity.transform.rotation; }
			set { entity.transform.rotation = value; }
		}

		/// <summary>
		/// raw zoom value. This is the exact value used for the scale matrix. Default is 1.
		/// </summary>
		/// <value>The raw zoom.</value>
		public float rawZoom
		{
			get { return _zoom; }
			set
			{
				if( value != _zoom )
				{
					_zoom = value;
					_areBoundsDirty = true;
				}
			}
		}

		/// <summary>
		/// the zoom value should be between -1 and 1. This value is then translated to be from minimumZoom to maximumZoom. This lets you set
		/// appropriate minimum/maximum values then use a more intuitive -1 to 1 mapping to change the zoom.
		/// </summary>
		/// <value>The zoom.</value>
		public float zoom
		{
			get
			{
				if( _zoom == 0 )
					return 1f;

				if( _zoom < 1 )
					return Mathf.map( _zoom, _minimumZoom, 1, -1, 0 );
				return Mathf.map( _zoom, 1, _maximumZoom, 0, 1 );
			}
			set { setZoom( value ); }
		}

		/// <summary>
		/// minimum non-scaled value (0 - float.Max) that the camera zoom can be. Defaults to 0.3
		/// </summary>
		/// <value>The minimum zoom.</value>
		public float minimumZoom
		{
			get { return _minimumZoom; }
			set { setMinimumZoom( value ); }
		}

		/// <summary>
		/// maximum non-scaled value (0 - float.Max) that the camera zoom can be. Defaults to 3
		/// </summary>
		/// <value>The maximum zoom.</value>
		public float maximumZoom
		{
			get { return _maximumZoom; }
			set { setMaximumZoom( value ); }
		}

		/// <summary>
		/// world-space bounds of the camera. useful for culling.
		/// </summary>
		/// <value>The bounds.</value>
		public RectangleF bounds
		{
			get
			{
				if( _areMatrixesDirty )
					updateMatrixes();

				if( _areBoundsDirty )
				{
					// top-left and bottom-right are needed by either rotated or non-rotated bounds
					var topLeft = screenToWorldPoint( new vec2( Core.graphicsDevice.Viewport.X + _inset.left, Core.graphicsDevice.Viewport.Y + _inset.top ) );
					var bottomRight = screenToWorldPoint( new vec2( Core.graphicsDevice.Viewport.X + Core.graphicsDevice.Viewport.Width - _inset.right, Core.graphicsDevice.Viewport.Y + Core.graphicsDevice.Viewport.Height - _inset.bottom ) );

					if ( entity.transform.rotation != 0 )
					{
						// special care for rotated bounds. we need to find our absolute min/max values and create the bounds from that
						var topRight = screenToWorldPoint( new vec2( Core.graphicsDevice.Viewport.X + Core.graphicsDevice.Viewport.Width - _inset.right, Core.graphicsDevice.Viewport.Y + _inset.top ) );
						var bottomLeft = screenToWorldPoint( new vec2( Core.graphicsDevice.Viewport.X + _inset.left, Core.graphicsDevice.Viewport.Y + Core.graphicsDevice.Viewport.Height - _inset.bottom ) );

						var minX = Mathf.minOf( topLeft.x, bottomRight.x, topRight.x, bottomLeft.x );
						var maxX = Mathf.maxOf( topLeft.x, bottomRight.x, topRight.x, bottomLeft.x );
						var minY = Mathf.minOf( topLeft.y, bottomRight.y, topRight.y, bottomLeft.y );
						var maxY = Mathf.maxOf( topLeft.y, bottomRight.y, topRight.y, bottomLeft.y );

						_bounds.location = new vec2( minX, minY );
						_bounds.width = maxX - minX;
						_bounds.height = maxY - minY;
					}
					else
					{
						_bounds.location = topLeft;
						_bounds.width = bottomRight.x - topLeft.x;
						_bounds.height = bottomRight.y - topLeft.y;
					}

					_areBoundsDirty = false;
				}

				return _bounds;
			}
		}

		/// <summary>
		/// used to convert from world coordinates to screen
		/// </summary>
		/// <value>The transform matrix.</value>
		public Matrix2D transformMatrix
		{
			get
			{
				if( _areMatrixesDirty )
					updateMatrixes();
				return _transformMatrix;
			}
		}

		/// <summary>
		/// used to convert from screen coordinates to world
		/// </summary>
		/// <value>The inverse transform matrix.</value>
		public Matrix2D inverseTransformMatrix
		{
			get
			{
				if( _areMatrixesDirty )
					updateMatrixes();
				return _inverseTransformMatrix;
			}
		}

		/// <summary>
		/// the 2D Cameras projection matrix
		/// </summary>
		/// <value>The projection matrix.</value>
		public mat4 projectionMatrix
		{
			get
			{
				if( _isProjectionMatrixDirty )
				{
					_projectionMatrix = mat4.CreateOrthographicOffCenter( 0, Core.graphicsDevice.Viewport.Width, Core.graphicsDevice.Viewport.Height, 0, 0, -1);
					_isProjectionMatrixDirty = false;
				}
				return _projectionMatrix;
			}
		}

		/// <summary>
		/// gets the view-projection matrix which is the transformMatrix * the projection matrix
		/// </summary>
		/// <value>The view projection matrix.</value>
		public mat4 viewProjectionMatrix { get { return mat4.Multiply(transformMatrix, projectionMatrix); } }

		//#region 3D Camera Matrixes

		///// <summary>
		///// returns a perspective projection for this camera for use when rendering 3D objects
		///// </summary>
		///// <value>The projection matrix3 d.</value>
		//public mat4 projectionMatrix3D
		//{
		//	get
		//	{
		//		var targetHeight = ( Core.graphicsDevice.Viewport.Height / _zoom );
		//		var fov = (float)Math.Atan( targetHeight / ( 2f * positionZ3D ) ) * 2f;
		//		return mat4.CreatePerspectiveFieldOfView( fov, Core.graphicsDevice.Viewport.AspectRatio, nearClipPlane3D, farClipPlane3D );
		//	}
		//}

		///// <summary>
		///// returns a view mat4 via CreateLookAt for this camera for use when rendering 3D objects
		///// </summary>
		///// <value>The view matrix3 d.</value>
		//public mat4 viewMatrix3D
		//{
		//	get
		//	{
		//		// we need to always invert the y-values to match the way Batcher/SpriteBatch does things
		//		var position3D = new vec3( position.X, -position.Y, positionZ3D );
		//		return mat4.CreateLookAt( position3D, position3D + vec3.Forward, vec3.Up );
		//	}
		//}

		//#endregion

		public vec2 origin
		{
			get { return _origin; }
			internal set
			{
				if( _origin != value )
				{
					_origin = value;
					forceMatrixUpdate();
				}
			}
		}


		float _zoom;
		float _minimumZoom = 0.3f;
		float _maximumZoom = 3f;
		RectangleF _bounds;
		CameraInset _inset;
		Matrix2D _transformMatrix = Matrix2D.identity;
		Matrix2D _inverseTransformMatrix = Matrix2D.identity;
		mat4 _projectionMatrix;
		vec2 _origin;

		bool _areMatrixesDirty = true;
		bool _areBoundsDirty = true;
		bool _isProjectionMatrixDirty = true;

		#endregion


		public Camera()
		{
			setZoom( 0 );
		}


		/// <summary>
		/// when the scene render target size changes we update the cameras origin and adjust the position to keep it where it was
		/// </summary>
		/// <param name="newWidth">New width.</param>
		/// <param name="newHeight">New height.</param>
		internal void onSceneRenderTargetSizeChanged( int newWidth, int newHeight )
		{
			_isProjectionMatrixDirty = true;
			var oldOrigin = _origin;
			origin = new vec2( newWidth / 2f, newHeight / 2f );
            var change = _origin - oldOrigin;
            // offset our position to match the new center
            entity.transform.position = (vec2)entity.transform.position + change;
		}


		protected virtual void updateMatrixes()
		{
			if( !_areMatrixesDirty )
				return;

			Matrix2D tempMat;
			_transformMatrix = Matrix2D.createTranslation( -entity.transform.position.x, -entity.transform.position.y ); // position

			if( _zoom != 1f )
			{
				Matrix2D.createScale( _zoom, _zoom, out tempMat ); // scale ->
				Matrix2D.multiply( ref _transformMatrix, ref tempMat, out _transformMatrix );
			}

			if( entity.transform.rotation != 0f )
			{
				Matrix2D.createRotation( entity.transform.rotation, out tempMat ); // rotation
				Matrix2D.multiply( ref _transformMatrix, ref tempMat, out _transformMatrix );
			}

			Matrix2D.createTranslation( (int)_origin.x, (int)_origin.y, out tempMat ); // translate -origin
			Matrix2D.multiply( ref _transformMatrix, ref tempMat, out _transformMatrix );

			// calculate our inverse as well
			Matrix2D.invert( ref _transformMatrix, out _inverseTransformMatrix );

			// whenever the matrix changes the bounds are then invalid
			_areBoundsDirty = true;
			_areMatrixesDirty = false;
		}


		#region Fluent setters

		/// <summary>
		/// sets the amount used to inset the camera bounds from the viewport edge
		/// </summary>
		/// <param name="left">The amount to set the left bounds in from the viewport.</param>
		/// <param name="right">The amount to set the right bounds in from the viewport.</param>
		/// <param name="top">The amount to set the top bounds in from the viewport.</param>
		/// <param name="bottom">The amount to set the bottom bounds in from the viewport.</param>
		public Camera setInset( float left, float right, float top, float bottom )
		{
			_inset = new CameraInset { left = left, right = right , top = top, bottom = bottom };
			_areBoundsDirty = true;
			return this;
		}


		/// <summary>
		/// shortcut to entity.transform.setPosition
		/// </summary>
		/// <param name="position">Position.</param>
		public Camera setPosition( vec2 position )
		{
			entity.transform.setPosition( position );
			return this;
		}


		/// <summary>
		/// shortcut to entity.transform.setRotation
		/// </summary>
		/// <param name="radians">Radians.</param>
		public Camera setRotation( float radians )
		{
			entity.transform.setRotation( radians );
			return this;
		}


		/// <summary>
		/// shortcut to entity.transform.setRotationDegrees
		/// </summary>
		/// <param name="degrees">Degrees.</param>
		public Camera setRotationDegrees( float degrees )
		{
			entity.transform.setRotationDegrees( degrees );
			return this;
		}


		/// <summary>
		/// sets the the zoom value which should be between -1 and 1. This value is then translated to be from minimumZoom to maximumZoom.
		/// This lets you set appropriate minimum/maximum values then use a more intuitive -1 to 1 mapping to change the zoom.
		/// </summary>
		/// <param name="zoom">Zoom.</param>
		public Camera setZoom( float zoom )
		{
			var newZoom = Mathf.clamp( zoom, -1, 1 );
			if( newZoom == 0 )
				_zoom = 1f;
			else if( newZoom < 0 )
				_zoom = Mathf.map( newZoom, -1, 0, _minimumZoom, 1 );
			else
				_zoom = Mathf.map( newZoom, 0, 1, 1, _maximumZoom );

			_areMatrixesDirty = true;

			return this;
		}


		/// <summary>
		/// minimum non-scaled value (0 - float.Max) that the camera zoom can be. Defaults to 0.3
		/// </summary>
		/// <param name="value">Value.</param>
		public Camera setMinimumZoom( float minZoom )
		{
			Assert.isTrue( minZoom > 0, "minimumZoom must be greater than zero" );

			if( _zoom < minZoom )
				_zoom = minimumZoom;

			_minimumZoom = minZoom;
			return this;
		}


		/// <summary>
		/// maximum non-scaled value (0 - float.Max) that the camera zoom can be. Defaults to 3
		/// </summary>
		/// <param name="maxZoom">Max zoom.</param>
		public Camera setMaximumZoom( float maxZoom )
		{
			Assert.isTrue( maxZoom > 0, "MaximumZoom must be greater than zero" );

			if( _zoom > maxZoom )
				_zoom = maxZoom;

			_maximumZoom = maxZoom;
			return this;
		}

		#endregion


		/// <summary>
		/// this forces the matrix and bounds dirty
		/// </summary>
		public void forceMatrixUpdate()
		{
			_areMatrixesDirty = _areBoundsDirty = true;
		}


		#region component overrides

		public override void onEntityTransformChanged( Transform.Component comp )
		{
			forceMatrixUpdate();
		}

		#endregion


		#region zoom helpers

		public void zoomIn( float deltaZoom )
		{
			zoom += deltaZoom;
		}


		public void zoomOut( float deltaZoom )
		{
			zoom -= deltaZoom;
		}

		#endregion


		#region transformations

		/// <summary>
		/// converts a point from world coordinates to screen
		/// </summary>
		/// <returns>The to screen point.</returns>
		/// <param name="worldPosition">World position.</param>
		public vec2 worldToScreenPoint( vec2 worldPosition )
		{
			updateMatrixes();
            worldPosition = vec2.Transform(worldPosition, _transformMatrix);
			//Vector2Ext.transform( ref worldPosition, ref _transformMatrix, out worldPosition );
			return worldPosition;
		}


		/// <summary>
		/// converts a point from screen coordinates to world
		/// </summary>
		/// <returns>The to world point.</returns>
		/// <param name="screenPosition">Screen position.</param>
		public vec2 screenToWorldPoint( vec2 screenPosition )
		{
			updateMatrixes();
            screenPosition = vec2.Transform(screenPosition, inverseTransformMatrix);
			//Vector2Ext.transform( ref screenPosition, ref _inverseTransformMatrix, out screenPosition );
			return screenPosition;
		}


		/// <summary>
		/// converts a point from screen coordinates to world
		/// </summary>
		/// <returns>The to world point.</returns>
		/// <param name="screenPosition">Screen position.</param>
		public vec2 screenToWorldPoint( Point screenPosition )
		{
			return screenToWorldPoint( screenPosition );
		}


		/// <summary>
		/// returns the mouse position in world space
		/// </summary>
		/// <returns>The to world point.</returns>
		public vec2 mouseToWorldPoint()
		{
			return screenToWorldPoint( Input.mousePosition );
		}


#if !FNA
		/// <summary>
		/// returns the touch position in world space
		/// </summary>
		/// <returns>The to world point.</returns>
		public vec2 touchToWorldPoint( TouchLocation touch )
		{
			return screenToWorldPoint( touch.scaledPosition() );
		}
#endif

		#endregion

	}
}

