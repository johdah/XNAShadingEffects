using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShaderLibrary;
using Microsoft.Xna.Framework.Graphics;
using ShaderLibrary.Effects;

namespace XNAShadingEffects.Entities
{
    public class Sphere : AbstractEntity
    {
        public Sphere(Game game, Model model)
            : base(model, game)
        {

        }

        public Sphere(Game game, Model model, Effect effect)
            : base(model, game)
        {
            this.effect = new ConcreteEffect(effect);
        }

        public override void DrawModelWithEffect(Matrix world, Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    //effect.Parameters["ModelTexture"].SetValue(texture);
                    //effect.Parameters["NormalMap"].SetValue(normalMap);
                    //effect.Parameters["FogColor"].SetValue(Color.CornflowerBlue.ToVector3());
                    //effect.Parameters["FogEnd"].SetValue(20.0f);
                    //effect.Parameters["FogStart"].SetValue(10.0f);
                    //effect.Parameters["ReflectionTexture"].SetValue(reflectionTexture);

                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["World"].SetValue((world * localWorld) * mesh.ParentBone.Transform);
                    effect.Parameters["WorldInverseTranspose"].SetValue(
                                            Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform)));
                }
                mesh.Draw();
            }
        }
    }
}
