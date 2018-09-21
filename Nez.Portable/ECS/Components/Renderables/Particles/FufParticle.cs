#if FEATURE_ESC
using System;
using Microsoft.Xna.Framework;
using Atma.PhysicsShapes;

namespace Atma.ECS.Components.Renderables.Particles
{
    public class FufParticle
    {
        public vec2 position;
        public vec2 spawnPosition;
        public Color color = Color.White;
        public float rotation = 0f;
        public float scale = 1f;
        public float alpha;
        public vec2 velocity;

        private float _timeToLive;
        private float _lifetime;

        private float _startScale;
        private float _endScale;
        private float _scaleDelta;

        private float _startAlpha;
        private float _endAlpha;
        private float _alphaDelta;

        private Color _startColor;
        private Color _endColor;

        private FufParticleCreatorConfig _emitterConfig;

        public void initialize(FufParticleCreatorConfig emitterConfig, vec2 spawnPosition)
        {
            _emitterConfig = emitterConfig;
            this.spawnPosition = spawnPosition;
            position = vec2.Zero;
            color = Color.White;
            rotation = 0f;
            scale = 1f;

            _lifetime = _timeToLive = emitterConfig.life.nextValue();
            rotation = emitterConfig.particleRotation.nextValue();
            var launchAngle = MathHelper.ToRadians(emitterConfig.launchAngle.nextValue());
            var speed = emitterConfig.speed.nextValue();
            velocity = new vec2((float) Math.Cos(launchAngle), (float) Math.Sin(launchAngle)) * speed;

            var offset = emitterConfig.offset.nextValue();
            offset = vec2.Transform(offset, mat4.RotateZ(launchAngle));
            position += offset;

            (_startScale, _endScale) = emitterConfig.scale.nextValues();
            (_startAlpha, _endAlpha) = emitterConfig.alpha.nextValues();
            (_startColor, _endColor) = emitterConfig.color.nextValues();

            scale = _startScale;
            alpha = _startAlpha;
            color = _startColor;

            _scaleDelta = (_endScale - _startScale) / _lifetime;
            _alphaDelta = (_endAlpha - _startAlpha) / _lifetime;
        }

        public bool update()
        {
            // update values
            _timeToLive -= Time.deltaTime;
            if (_timeToLive > 0)
            {
                // TODO: update kinematics
                position += velocity * Time.deltaTime;

                // interpolate values

                ColorExt.lerp(ref _startColor, ref _endColor, out color, (_lifetime - _timeToLive) / (_lifetime));
                scale += _scaleDelta * Time.deltaTime;
                alpha += _alphaDelta * Time.deltaTime;
                color.A = (byte) (255 * alpha);
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}
#endif
