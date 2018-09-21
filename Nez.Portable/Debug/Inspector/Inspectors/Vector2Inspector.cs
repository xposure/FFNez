#if FEATURE_UI
using Microsoft.Xna.Framework;
using Atma.UI;
using System.Globalization;


#if DEBUG
namespace Atma
{
	public class Vector2Inspector : Inspector
	{
		TextField _textFieldX, _textFieldY;


		public override void initialize( Table table, Skin skin, float leftCellWidth )
		{
			var value = getValue<vec2>();
			var label = createNameLabel( table, skin, leftCellWidth );

			var labelX = new Label( "x", skin );
			_textFieldX = new TextField( value.X.ToString( CultureInfo.InvariantCulture ), skin );
			_textFieldX.setTextFieldFilter( new FloatFilter() ).setPreferredWidth( 50 );
			_textFieldX.onTextChanged += ( field, str ) =>
			{
				float newX;
				if( float.TryParse( str, NumberStyles.Float, CultureInfo.InvariantCulture, out newX ) )
				{
					var newValue = getValue<vec2>();
					newValue.X = newX;
					setValue( newValue );
				}
			};

			var labelY = new Label( "y", skin );
			_textFieldY = new TextField( value.Y.ToString( CultureInfo.InvariantCulture ), skin );
			_textFieldY.setTextFieldFilter( new FloatFilter() ).setPreferredWidth( 50 );
			_textFieldY.onTextChanged += ( field, str ) =>
			{
				float newY;
				if( float.TryParse( str, NumberStyles.Float, CultureInfo.InvariantCulture, out newY ) )
				{
					var newValue = getValue<vec2>();
					newValue.Y = newY;
					setValue( newValue );
				}
			};

			var hBox = new HorizontalGroup( 5 );
			hBox.addElement( labelX );
			hBox.addElement( _textFieldX );
			hBox.addElement( labelY );
			hBox.addElement( _textFieldY );

			table.add( label );
			table.add( hBox );
		}


		public override void update()
		{
			var value = getValue<vec2>();
			_textFieldX.setText( value.X.ToString( CultureInfo.InvariantCulture ) );
			_textFieldY.setText( value.Y.ToString( CultureInfo.InvariantCulture ) );
		}

	}
}
#endif
#endif
