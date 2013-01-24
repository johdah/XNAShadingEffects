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

        public Helicopter(Game game, Model model)
            : base(model, game)
        {
            _effect = new ConcreteEffect(game.Content.Load<Effect>("Effects/bumpReflection"));  //bumpidump
            _texture = game.Content.Load<Texture2D>("Models/Helicopter/helicopterTexture");
            normalmap = game.Content.Load<Texture2D>("Models/Helicopter/helicopterNormalMap");
        }

        public Helicopter(Game game, Model model, Effect effect)
            : base(model, game)
        {
            this._effect = new ConcreteEffect(effect);
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
                        _effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        _effect.Parameters["FogEnabled"].SetValue(true);
                        _effect.Parameters["FogDistance"].SetValue(Vector3.Distance(_position,cameraPosition));
                        _effect.Parameters["FogColor"].SetValue(Color.CornflowerBlue.ToVector4());
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
                                                Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform)));
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
