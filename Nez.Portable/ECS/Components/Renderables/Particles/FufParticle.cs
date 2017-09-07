using Microsoft.Xna.Framework;
using Nez.PhysicsShapes;

namespace Nez.ECS.Components.Renderables.Particles
{
    public class FufParticle
    {
        private static Circle _circleCollisionShape = new Circle(0);

        public Vector2 position;
        public Vector2 spawnPosition;
        public Color color = Color.White;
        public float rotation = 0f;
        public float scale = 1f;
        public float alpha;

        private float _timeToLive;

        private float _startScale;
        private float _endScale;

        private float _startAlpha;
        private float _endAlpha;

        private Color _startColor;
        private Color _endColor;

        private FufParticleCreatorConfig _emitterConfig;

        public void initialize(FufParticleCreatorConfig emitterConfig, Vector2 spawnPosition)
        {
            this._emitterConfig = emitterConfig;
            this.spawnPosition = spawnPosition;
            position = Vector2.Zero;
            color = Color.White;
            rotation = 0f;
            scale = 1f;

            _timeToLive = emitterConfig.Life.NextValue();
            rotation = emitterConfig.ParticleAngle.NextValue();

            (_startScale, _endScale) = emitterConfig.Scale.NextValues();
            (_startAlpha, _endAlpha) = emitterConfig.Alpha.NextValues();
            (_startColor, _endColor) = emitterConfig.Color.NextValues();

            scale = _startScale;
            alpha = _startAlpha;
            color = _startColor;
        }

        public bool update()
        {
            // TODO: update values
            
            return false;
        }
    }
}