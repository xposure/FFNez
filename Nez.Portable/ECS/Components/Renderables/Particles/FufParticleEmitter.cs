using System;
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
        public bool emitting = true;
        public bool playing = false;
        public bool simulateInWorldSpace = true;

        private float _elapsedTime = 0;
        private float _emitCounter = 0;
        private float _frequency = 0;
        private int _maxParticles;

        public FufParticleEmitter(FufParticleCreatorConfig emitterConfig, int maxParticles = 200,
            bool startEmitting = true)
        {
            this.emitterConfig = emitterConfig;
            _particles = new List<FufParticle>(maxParticles);
            Pool<FufParticle>.warmCache(maxParticles);
            _maxParticles = maxParticles;
            emitting = startEmitting;

            emitterInit();
        }

        private void emitterInit()
        {
            var blendState = new BlendState();
            material = new Material(blendState);
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            if (emitting)
            {
                play();
            }
        }

        public void play(float frequency = 20f)
        {
            playing = true;
            emitting = true;
            _frequency = frequency;

            _elapsedTime = 0;
            _emitCounter = 0;
        }

        public void stop()
        {
            playing = false;
            emitting = false;

            _elapsedTime = 0;
            _emitCounter = 0;
        }

        public void pause()
        {
            emitting = false;
        }

        public void emit(int count)
        {
            for (var i = 0; i < count; i++)
            {
                addParticle(rootPosition);
            }
        }

        public void update()
        {
            if (!playing) return;

            if (_frequency > 0)
            {
                var emitTime = 1f / _frequency;

                if (_particles.Count < _maxParticles)
                {
                    _emitCounter += Time.deltaTime;
                }

                while (emitting && _particles.Count < _maxParticles && _emitCounter > emitTime)
                {
                    addParticle(rootPosition);
                    _emitCounter -= emitTime;
                }

                _elapsedTime += Time.deltaTime;
            }

            // update particles
            for (var i = _particles.Count - 1; i >= 0; i--)
            {
                var particle = _particles[i];

                // if update is true, particle is done
                if (particle.update())
                {
                    Pool<FufParticle>.free(particle);
                    _particles.RemoveAt(i);
                }
            }
        }

        public override void render(Graphics graphics, Camera camera)
        {
            for (var i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];
                var referencePosition = simulateInWorldSpace ? particle.spawnPosition : rootPosition;
                graphics.batcher.draw(emitterConfig.Subtexture, referencePosition + particle.position, particle.color,
                    particle.rotation, emitterConfig.Subtexture.center, particle.scale, SpriteEffects.None, layerDepth);
            }
        }

        protected void addParticle(Vector2 spawnPosition)
        {
            var particle = Pool<FufParticle>.obtain();
            particle.initialize(emitterConfig, spawnPosition);
            _particles.Add(particle);
        }
    }
}