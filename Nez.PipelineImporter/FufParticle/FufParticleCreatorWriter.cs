using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Nez.ECS.Components.Renderables.Particles;
using Nez.PipelineRuntime.FufParticleCreator;

namespace Nez.PipelineImporter.FufParticle
{
    [ContentTypeWriter]
    public class FufParticleCreatorWriter : ContentTypeWriter<FufParticleEmitterProcessorResult>
    {
        protected override void Write(ContentWriter writer, FufParticleEmitterProcessorResult value)
        {
            try
            {
                writer.Write(value.emitterConfig.LaunchAngle.min);
                writer.Write(value.emitterConfig.LaunchAngle.max);
            
                writer.Write(value.emitterConfig.Speed.min);
                writer.Write(value.emitterConfig.Speed.max);
            
                writer.Write(value.emitterConfig.Life.min);
                writer.Write(value.emitterConfig.Life.max);
            
                writer.Write(value.emitterConfig.Color.min);
                writer.Write(value.emitterConfig.Color.max);
            
                writer.Write(value.emitterConfig.Alpha.min);
                writer.Write(value.emitterConfig.Alpha.max);
            
                writer.Write(value.emitterConfig.Scale.min);
                writer.Write(value.emitterConfig.Scale.max);
                writer.Write(value.emitterConfig.KeepScaleRatio);
            
                writer.Write(value.emitterConfig.ParticleAngle.min);
                writer.Write(value.emitterConfig.ParticleAngle.max);
                
                // write out texture data
                writer.WriteObject(value.texture);
            }
            catch (NullReferenceException e)
            {
                FufParticleCreatorProcessor.logger.LogImportantMessage(e.ToString());
                throw;
            }
        }
        
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(FufParticleCreatorConfig).AssemblyQualifiedName;
        }
        
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // This is the full namespace path and class name of the reader, along with the assembly name which is the project name by default.
            return typeof(FufParticleCreatorConfigReader).AssemblyQualifiedName;
        }
    }
}