﻿using System;
using Microsoft.Xna.Framework;


namespace Nez
{
    using Vector2 = Nez.vec2;
	public class CameraShake : Component, IUpdatable
	{
		Vector2 _shakeDirection;
		Vector2 _shakeOffset;
		float _shakeIntensity = 0f;
		float _shakeDegredation = 0.95f;


		/// <summary>
		/// if the shake is already running this will overwrite the current values only if shakeIntensity > the current shakeIntensity.
		/// if the shake is not currently active it will be started.
		/// </summary>
		/// <param name="shakeIntensity">how much should we shake it</param>
		/// <param name="shakeDegredation">higher values cause faster degradation</param>
		/// <param name="shakeDirection">Vector3.zero will result in a shake on just the x/y axis. any other values will result in the passed
		/// in shakeDirection * intensity being the offset the camera is moved</param>
		public void shake( float shakeIntensity = 15f, float shakeDegredation = 0.9f, Vector2 shakeDirection = default( Vector2 ) )
		{
			enabled = true;
			if( _shakeIntensity < shakeIntensity )
			{
				_shakeDirection = shakeDirection;
				_shakeIntensity = shakeIntensity;
				if( shakeDegredation < 0f || shakeDegredation >= 1f )
					shakeDegredation = 0.95f;
				
				_shakeDegredation = shakeDegredation;
			}
		}


		void IUpdatable.update()
		{
			if( Math.Abs( _shakeIntensity ) > 0f )
			{
				_shakeOffset = _shakeDirection;
				if( _shakeOffset.X != 0f || _shakeOffset.Y != 0f )
				{
					_shakeOffset.Normalize();
				}
				else
				{
					_shakeOffset.X = _shakeOffset.X + Random.nextFloat() - 0.5f;
					_shakeOffset.Y = _shakeOffset.Y + Random.nextFloat() - 0.5f;
				}

				// TODO: this needs to be multiplied by camera zoom so that less shake gets applied when zoomed in
				_shakeOffset *= _shakeIntensity;
				_shakeIntensity *= -_shakeDegredation;
				if( Math.Abs( _shakeIntensity ) <= 0.01f )
				{
					_shakeIntensity = 0f;
					enabled = false;
				}
			}

			entity.scene.camera.position += _shakeOffset;
		}
	}
}

