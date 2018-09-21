#if FEATURE_INPUT
using System;


namespace Atma
{
	public interface IClipboard
	{
		string getContents();
		void setContents( string text );
	}
}

#endif
