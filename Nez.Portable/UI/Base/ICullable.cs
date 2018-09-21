#if FEATURE_UI
using Microsoft.Xna.Framework;

namespace Nez.UI
{
    public interface ICullable
    {
        void setCullingArea(Rectangle cullingArea);
    }
}
#endif
