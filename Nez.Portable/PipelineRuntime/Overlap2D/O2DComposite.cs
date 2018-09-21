#if FEATURE_PIPELINE
using System;
using System.Collections.Generic;


namespace Atma.Overlap2D
{
	public class O2DComposite
	{
		public List<O2DImage> images = new List<O2DImage>();
		public List<O2DCompositeItem> compositeItems = new List<O2DCompositeItem>();
		public List<O2DColorPrimitive> colorPrimitives = new List<O2DColorPrimitive>();


		public O2DComposite()
		{}
	}
}

#endif
