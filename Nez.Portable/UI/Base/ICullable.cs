#if FEATURE_UI
using Microsoft.Xna.Framework;

namespace Atma.UI
{
    public interface ICullable
    {
        void setCullingArea(Rectangle cullingArea);
    }
}
#endif
