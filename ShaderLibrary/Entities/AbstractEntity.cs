using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShaderLibrary.Effects;
using ShaderLibrary.Managers;

namespace ShaderLibrary
{
    public abstract class AbstractEntity
    {
        #region Fields

        private Game _game;
        protected Matrix _localWorld = Matrix.Identity;

        protected ConcreteEffect _effect;
        protected Model _model;
        protected Texture2D _texture;
        protected Boolean _visible;
        protected Vector3 _position;

        #endregion
        #region Properties

        public ConcreteEffect Effect
        {
            get
            {
                return _effect;
            }
            set
            {
                _effect = value;
            }
        }
        public Boolean Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        #endregion

        public AbstractEntity(Model model, Game game)
        {
            this._model = model;
            this._game = game;
            _visible = true;
            _position = new Vector3(0, 0, 0);
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void SetLighting(ConcreteEffect conceffect)
        {
        }

        #region Draw

        public virtual void Draw(Matrix world, Matrix view, Matrix projection, TextureCube reflectionTexture, Vector3 cameraPosition, RenderPass pass)
        {
            if (_visible)
            {
                if (_effect != null)
                {
                    if (cameraPosition != null)
                    {
                        DrawModelWithEffect(world, view, projection, reflectionTexture, cameraPosition);
                    }
                    else
                    {
                        DrawModelWithEffect(world, view, projection);
                    }
                }
                else
                {
                    DrawModel(world, view, projection, pass);
                }
            }
        }

        public virtual void DrawModel(Matrix world, Matrix view, Matrix projection, RenderPass pass)
        {
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    //basicEffect.Alpha
                    basicEffect.EnableDefaultLighting();
                    basicEffect.PreferPerPixelLighting = true;
                    basicEffect.Projection = projection;
                    basicEffect.View = view;
                    basicEffect.World = world * mesh.ParentBone.Transform * Matrix.CreateTranslation(_position);
                }
                mesh.Draw();
            }
        }

        public virtual void DrawModelWithEffect(Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _effect;
                    _effect.Projection = projection;
                    _effect.View = view;
                    _effect.World = Matrix.Identity * mesh.ParentBone.Transform * Matrix.CreateTranslation(_position);
                    _effect.WorldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                }
                mesh.Draw();
            }
        }

        public virtual void DrawModelWithEffect(Matrix world, Matrix view, Matrix projection, TextureCube reflectionTexture, Vector3 cameraPosition)
        {
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _effect;
                    _effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    _effect.Parameters["FogColor"].SetValue(Color.WhiteSmoke.ToVector3());
                    _effect.Parameters["FogEnd"].SetValue(20.0f);
                    _effect.Parameters["FogStart"].SetValue(10.0f);
                    _effect.Parameters["ModelTexture"].SetValue(_texture);
                    _effect.Parameters["Projection"].SetValue(projection);
                    _effect.Parameters["View"].SetValue(view);
                    _effect.Parameters["World"].SetValue((world * _localWorld) * mesh.ParentBone.Transform * Matrix.CreateTranslation(_position));
                    _effect.Parameters["WorldInverseTranspose"].SetValue(
                                            Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform)));
                }
                mesh.Draw();
            }
        }

        #endregion
    }
}
