using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez.ECS.Components.Renderables.Particles
{
    public class FufParticleEmitter : RenderableComponent, IUpdatable
    {
        private Vector2 rootPosition => entity.transform.position + _localOffset;
        private List<FufParticle> _particles;
        public FufParticleCreatorConfig emitterConfig;

        public FufParticleEmitter(FufParticleCreatorConfig emitterConfig)
        {
            this.emitterConfig = emitterConfig;
        }

        public void update()
        {
        }

        public override void render(Graphics graphics, Camera camera)
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];
                graphics.batcher.draw(emitterConfig.Subtexture, rootPosition + particle.position, particle.color, particle.rotation, emitterConfig.Subtexture.center, particle.scale, SpriteEffects.None, layerDepth);
            }
        }
    }
}