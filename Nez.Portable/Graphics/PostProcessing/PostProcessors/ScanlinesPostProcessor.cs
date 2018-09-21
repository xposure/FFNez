#if FEATURE_GRAPHICS


namespace Atma
{
	public class ScanlinesPostProcessor : PostProcessor<ScanlinesEffect>
	{
		public ScanlinesPostProcessor( int executionOrder ) : base( executionOrder )
		{
			effect = new ScanlinesEffect();
		}
	}
}

#endif
