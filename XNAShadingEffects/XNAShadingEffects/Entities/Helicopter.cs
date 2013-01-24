using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShaderLibrary;
using ShaderLibrary.Effects;

namespace XNAShadingEffects.Entities
{
    public class Helicopter : AbstractEntity
    {
        private Texture2D normalmap;

        public Helicopter(Game game, Model model, ConcreteEffect effect)
            : base(model, game)
        {
            _effect = effect;
            _texture = game.Content.Load<Texture2D>("Models/Helicopter/helicopterTexture");
            normalmap = game.Content.Load<Texture2D>("Models/Helicopter/helicopterNormalMap");
        }

        public override void DrawModelWithEffect(Matrix world, Matrix view, Matrix projection, TextureCube reflectionTexture, Vector3 cameraPosition)
        {
            foreach (var item in _effect.CurrentTechnique.Passes)
            {


                foreach (ModelMesh mesh in _model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = _effect;

                        // Basic
                        _effect.Parameters["Projection"].SetValue(projection);
                        _effect.Parameters["View"].SetValue(view);
                        _effect.Parameters["World"].SetValue((world * _localWorld) * mesh.ParentBone.Transform * Matrix.CreateTranslation(_position));
                        // Diffuse
                        _effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world)));
                        // Specular
                        _effect.Parameters["ViewVector"].SetValue(Matrix.Invert(view).Translation);
                        // Textured
                        _effect.Parameters["ModelTexture"].SetValue(_texture);
                        // Bump
                        _effect.Parameters["NormalMap"].SetValue(normalmap);
                        // Reflection
                        _effect.Parameters["SkyboxTexture"].SetValue(reflectionTexture);
                        _effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        // Fog
                        _effect.Parameters["FogColor"].SetValue(Color.White.ToVector3());
                        _effect.Parameters["FogEnd"].SetValue(20f);
                        _effect.Parameters["FogStart"].SetValue(10f);
                        /*_effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        _effect.Parameters["FogEnabled"].SetValue(true);
                        _effect.Parameters["FogDistance"].SetValue(Vector3.Distance(_position,cameraPosition));
                        _effect.Parameters["FogColor"].SetValue(Color.DarkGoldenrod.ToVector4());
                        _effect.Parameters["FogEnd"].SetValue(30.0f);
                        _effect.Parameters["FogStart"].SetValue(20.0f);
                        _effect.Parameters["ReflectedTexture"].SetValue(reflectionTexture);
                        _effect.Parameters["ModelTexture"].SetValue(_texture);
                        _effect.Parameters["NormalMap"].SetValue(normalmap);
                        _effect.Parameters["Projection"].SetValue(projection);
                        _effect.Parameters["View"].SetValue(view);
                        _effect.Parameters["ViewVector"].SetValue(view.Backward);
                        _effect.Parameters["World"].SetValue((world * _localWorld) * mesh.ParentBone.Transform * Matrix.CreateTranslation(_position));
                        _effect.Parameters["WorldInverseTranspose"].SetValue(
                                                Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform)));*/
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
