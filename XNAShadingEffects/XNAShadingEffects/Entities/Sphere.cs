﻿using System;
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
        private Texture2D normalmap;

        public Sphere(Game game, Model model)
            : base(model, game)
        {
        }

        public Sphere(Game game, Model model, Effect effect)
            : base(model, game)
        {
            this._effect = new ConcreteEffect(effect);

            _texture = game.Content.Load<Texture2D>("Models/Sphere/texture");
            normalmap = game.Content.Load<Texture2D>("Models/Sphere/normalMap");
        }

        public override void DrawModelWithEffect(Matrix world, Matrix view, Matrix projection, TextureCube reflectionTexture, Vector3 cameraPosition)
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

                    //// OLD
                    //_effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    //effect.Parameters["FogColor"].SetValue(Color.CornflowerBlue.ToVector3());
                    //effect.Parameters["FogEnd"].SetValue(20.0f);
                    //effect.Parameters["FogStart"].SetValue(10.0f);
                    //effect.Parameters["ModelTexture"].SetValue(texture);
                    //effect.Parameters["NormalMap"].SetValue(normalmap);

                    //_effect.Parameters["Projection"].SetValue(projection);
                    //_effect.Parameters["ReflectedTexture"].SetValue(reflectionTexture);
                    //_effect.Parameters["View"].SetValue(view);
                    //_effect.Parameters["World"].SetValue((world * _localWorld) * mesh.ParentBone.Transform * Matrix.CreateTranslation(_position));
                    //_effect.Parameters["WorldInverseTranspose"].SetValue(
                      //                      Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform)));
                }
                mesh.Draw();
            }
        }
    }
}
