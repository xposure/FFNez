﻿
using Microsoft.Xna.Framework.Graphics;


namespace Nez
{
	public class DissolveEffect : Effect
	{
		/// <summary>
		/// value from 0 - 1 that determines how much the dissolve effect will encompass
		/// </summary>
		/// <value>The progress.</value>
		public float progress
		{
			get { return _progress; }
			set
			{
				_progressParam.SetValue( value );
				_progress = value;
			}
		}

		/// <summary>
		/// determines how much area around the current dissolve threshold will be colored with dissolveThresholdColor
		/// </summary>
		/// <value>The dissolve threshold.</value>
		public float dissolveThreshold
		{
			get { return _dissolveThreshold; }
			set
			{
				_dissolveThresholdParam.SetValue( value );
				_dissolveThreshold = value;
			}
		}

		/// <summary>
		/// the Color that will appear on the threshold of the dissolve effect
		/// </summary>
		/// <value>The color of the dissolve threshold.</value>
		public Color dissolveThresholdColor
		{
			get { return _dissolveThresholdColor; }
			set
			{
				_dissolveThresholdColorParam.SetValue( value );
				_dissolveThresholdColor = value;
			}
		}

		/// <summary>
		/// the grayscale texture used to determine what is disolved
		/// </summary>
		/// <value>The dissolve texture.</value>
		public Texture2D dissolveTexture
		{
			set { _dissolveTexParam.SetValue( value ); }
		}

		float _progress = 0f;
		float _dissolveThreshold = 0.1f;
		Color _dissolveThresholdColor = Color.Red;


		EffectParameter _progressParam;
		EffectParameter _dissolveThresholdParam;
		EffectParameter _dissolveThresholdColorParam;
		EffectParameter _dissolveTexParam;


		public DissolveEffect( Texture2D dissolveTexture ) : base( Core.graphicsDevice, EffectResource.dissolveBytes )
		{
			_progressParam = Parameters["_progress"];
			_dissolveThresholdParam = Parameters["_dissolveThreshold"];
			_dissolveThresholdColorParam = Parameters["_dissolveThresholdColor"];
			_dissolveTexParam = Parameters["_dissolveTex"];

			_progressParam.SetValue( _progress );
			_dissolveThresholdParam.SetValue( _dissolveThreshold );
			_dissolveThresholdColorParam.SetValue( _dissolveThresholdColor );
			_dissolveTexParam.SetValue( dissolveTexture );
		}
	}
}

