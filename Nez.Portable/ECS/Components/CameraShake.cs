using System;



namespace Nez
{
	public class CameraShake : Component, IUpdatable
	{
		vec2 _shakeDirection;
		vec2 _shakeOffset;
		float _shakeIntensity = 0f;
		float _shakeDegredation = 0.95f;


		/// <summary>
		/// if the shake is already running this will overwrite the current values only if shakeIntensity > the current shakeIntensity.
		/// if the shake is not currently active it will be started.
		/// </summary>
		/// <param name="shakeIntensity">how much should we shake it</param>
		/// <param name="shakeDegredation">higher values cause faster degradation</param>
		/// <param name="shakeDirection">vec3.zero will result in a shake on just the x/y axis. any other values will result in the passed
		/// in shakeDirection * intensity being the offset the camera is moved</param>
		public void shake( float shakeIntensity = 15f, float shakeDegredation = 0.9f, vec2 shakeDirection = default( vec2 ) )
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
				if( _shakeOffset.x != 0f || _shakeOffset.y != 0f )
				{
					_shakeOffset.Normalize();
				}
				else
				{
					_shakeOffset.x = _shakeOffset.x + Random.nextFloat() - 0.5f;
					_shakeOffset.y = _shakeOffset.y + Random.nextFloat() - 0.5f;
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

