using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;


namespace Nez
{
	/// <summary>
	/// A virtual input that is represented as a vec2, with both X and Y as values between -1 and 1
	/// </summary>
	public class VirtualJoystick : VirtualInput
	{
		public List<Node> nodes = new List<Node>();
		public bool normalized;

		public vec2 value
		{
			get
			{
				for( int i = 0; i < nodes.Count; i++ )
				{
					var val = nodes[i].value;
					if( val != vec2.Zero )
					{
						if( normalized )
							val.Normalize();
						return val;
					}
				}

				return vec2.Zero;
			}
		}


		public VirtualJoystick( bool normalized ) : base()
		{
			this.normalized = normalized;
		}


		public VirtualJoystick( bool normalized, params Node[] nodes ) : base()
		{
			this.normalized = normalized;
			this.nodes.AddRange( nodes );
		}


		public override void update()
		{
			for( int i = 0; i < nodes.Count; i++ )
				nodes[i].update();
		}


		#region Node management

		/// <summary>
		/// adds GamePad left stick input to this VirtualJoystick
		/// </summary>
		/// <returns>The game pad left stick.</returns>
		/// <param name="gamepadIndex">Gamepad index.</param>
		/// <param name="deadzone">Deadzone.</param>
		public VirtualJoystick addGamePadLeftStick( int gamepadIndex = 0, float deadzone = Input.DEFAULT_DEADZONE )
		{
			nodes.Add( new GamePadLeftStick( gamepadIndex, deadzone ) );
			return this;
		}


		/// <summary>
		/// adds GamePad right stick input to this VirtualJoystick
		/// </summary>
		/// <returns>The game pad right stick.</returns>
		/// <param name="gamepadIndex">Gamepad index.</param>
		/// <param name="deadzone">Deadzone.</param>
		public VirtualJoystick addGamePadRightStick( int gamepadIndex = 0, float deadzone = Input.DEFAULT_DEADZONE )
		{
			nodes.Add( new GamePadRightStick( gamepadIndex, deadzone ) );
			return this;
		}


		/// <summary>
		/// adds GamePad DPad input to this VirtualJoystick
		/// </summary>
		/// <returns>The game pad DP ad.</returns>
		/// <param name="gamepadIndex">Gamepad index.</param>
		public VirtualJoystick addGamePadDPad( int gamepadIndex = 0 )
		{
			nodes.Add( new GamePadDpad( gamepadIndex ) );
			return this;
		}


		/// <summary>
		/// adds keyboard keys input to this VirtualJoystick. Four keyboard keys will emulate left/right/up/down. For example WASD or the arrow
		/// keys.
		/// </summary>
		/// <returns>The keyboard keys.</returns>
		/// <param name="overlapBehavior">Overlap behavior.</param>
		/// <param name="left">Left.</param>
		/// <param name="right">Right.</param>
		/// <param name="up">Up.</param>
		/// <param name="down">Down.</param>
		public VirtualJoystick addKeyboardKeys( OverlapBehavior overlapBehavior, Keys left, Keys right, Keys up, Keys down )
		{
			nodes.Add( new KeyboardKeys( overlapBehavior, left, right, up, down ) );
			return this;
		}

		#endregion


		static public implicit operator vec2( VirtualJoystick joystick )
		{
			return joystick.value;
		}


		#region Node types

		public abstract class Node : VirtualInputNode
		{
			public abstract vec2 value { get; }
		}


		public class GamePadLeftStick : Node
		{
			public int gamepadIndex;
			public float deadzone;


			public GamePadLeftStick( int gamepadIndex = 0, float deadzone = Input.DEFAULT_DEADZONE )
			{
				this.gamepadIndex = gamepadIndex;
				this.deadzone = deadzone;
			}


			public override vec2 value
			{
				get
				{
					return Input.gamePads[gamepadIndex].getLeftStick( deadzone );
				}
			}
		}


		public class GamePadRightStick : Node
		{
			public int gamepadIndex;
			public float deadzone;


			public GamePadRightStick( int gamepadIndex = 0, float deadzone = Input.DEFAULT_DEADZONE )
			{
				this.gamepadIndex = gamepadIndex;
				this.deadzone = deadzone;
			}

			public override vec2 value
			{
				get
				{
					return Input.gamePads[gamepadIndex].getRightStick( deadzone );
				}
			}
		}


		public class GamePadDpad : Node
		{
			public int gamepadIndex;


			public GamePadDpad( int gamepadIndex = 0 )
			{
				this.gamepadIndex = gamepadIndex;
			}


			public override vec2 value
			{
				get
				{
					var _value = vec2.Zero;

					if( Input.gamePads[gamepadIndex].DpadRightDown )
						_value.x = 1f;
					else if( Input.gamePads[gamepadIndex].DpadLeftDown )
						_value.x = -1f;

					if( Input.gamePads[gamepadIndex].DpadDownDown )
						_value.y = 1f;
					else if( Input.gamePads[gamepadIndex].DpadUpDown )
						_value.y = -1f;

					return _value;
				}
			}
		}


		public class KeyboardKeys : Node
		{
			public OverlapBehavior overlapBehavior;
			public Keys left;
			public Keys right;
			public Keys up;
			public Keys down;

			private bool _turnedX;
			private bool _turnedY;
			private vec2 _value;


			public KeyboardKeys( OverlapBehavior overlapBehavior, Keys left, Keys right, Keys up, Keys down )
			{
				this.overlapBehavior = overlapBehavior;
				this.left = left;
				this.right = right;
				this.up = up;
				this.down = down;
			}


			public override void update()
			{
				//X Axis
				if( Input.isKeyDown( left ) )
				{
					if( Input.isKeyDown( right ) )
					{
						switch( overlapBehavior )
						{
							default:
							case OverlapBehavior.CancelOut:
								_value.x = 0;
								break;
							case OverlapBehavior.TakeNewer:
								if( !_turnedX )
								{
									_value.x *= -1;
									_turnedX = true;
								}
								break;
							case OverlapBehavior.TakeOlder:
								//X stays the same
								break;
						}
					}
					else
					{
						_turnedX = false;
						_value.x = -1;
					}
				}
				else if( Input.isKeyDown( right ) )
				{
					_turnedX = false;
					_value.x = 1;
				}
				else
				{
					_turnedX = false;
					_value.x = 0;
				}

				//Y Axis
				if( Input.isKeyDown( up ) )
				{
					if( Input.isKeyDown( down ) )
					{
						switch( overlapBehavior )
						{
							default:
							case OverlapBehavior.CancelOut:
								_value.y = 0;
								break;
							case OverlapBehavior.TakeNewer:
								if( !_turnedY )
								{
									_value.y *= -1;
									_turnedY = true;
								}
								break;
							case OverlapBehavior.TakeOlder:
								//Y stays the same
								break;
						}
					}
					else
					{
						_turnedY = false;
						_value.y = -1;
					}
				}
				else if( Input.isKeyDown( down ) )
				{
					_turnedY = false;
					_value.y = 1;
				}
				else
				{
					_turnedY = false;
					_value.y = 0;
				}
			}


			public override vec2 value
			{
				get { return _value; }
			}
		}

		#endregion

	}
}

