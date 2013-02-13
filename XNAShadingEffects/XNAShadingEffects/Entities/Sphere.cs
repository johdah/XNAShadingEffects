using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShaderLibrary;
using Microsoft.Xna.Framework.Graphics;
using ShaderLibrary.Effects;
using ShaderLibrary.Entities;

namespace XNAShadingEffects.Entities
{
    public class Sphere : OpacityEntity
    {
        private Texture2D normalmap;

        public BoundingSphere BoundingSphere
        {
            get
            {
                return _model.Meshes["Sphere"].BoundingSphere;
            }
        }

        public Sphere(Game game, Model model, ConcreteEffect effect)
            : base(model, game)
        {
            _effect = new ConcreteEffect(effect);
            //this._effect.Parameters["AmbientIntensity"].SetValue(0);
            //this._effect.Parameters["DiffuseIntensity"].SetValue(0);
            this._effect.Parameters["ReflectionEnabled"].SetValue(true);
            this._effect.Parameters["BumpEnabled"].SetValue(true);
            //this._effect.Parameters["SpecularIntensity"].SetValue(0);
            _isDoubleSided["Sphere"] = false;

            _texture = game.Content.Load<Texture2D>("Models/Sphere/texture");
            //normalmap = game.Content.Load<Texture2D>("Models/Sphere/normalMap");
            //normalmap = game.Content.Load<Texture2D>("Models/Sphere/setts-normalmap");'
            normalmap = game.Content.Load<Texture2D>("Models/Sphere/normal-map");
        }

        public override void DrawModelWithEffect(Matrix world, Matrix view, Matrix projection, TextureCube reflectionTexture, Vector3 cameraPosition)
        {
            RasterizerState previous = _device.RasterizerState;
            _device.RasterizerState = RasterizerState.CullCounterClockwise;
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
                    //_effect.Parameters["ModelTexture"].SetValue(normalmap);
                    // Bump
                    _effect.Parameters["NormalMap"].SetValue(normalmap);
                    // Reflection
                    _effect.Parameters["ReflectionTexture"].SetValue(reflectionTexture);
                    _effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    // Fog
                    _effect.Parameters["FogColor"].SetValue(Color.White.ToVector3());
                    _effect.Parameters["FogEnd"].SetValue(20f);
                    _effect.Parameters["FogStart"].SetValue(10f);
                    // Other
                    _effect.Parameters["TextureColorDefault"].SetValue(Color.Gray.ToVector4());
                }
                mesh.Draw();
            }
            _device.RasterizerState = previous;
        }
    }
}
