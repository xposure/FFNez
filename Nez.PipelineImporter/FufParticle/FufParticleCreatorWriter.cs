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
            writer.Write(value.emitterConfig.Angle);
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