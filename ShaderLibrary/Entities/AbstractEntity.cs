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
                }
                else
                {
                    DrawModel(world, view, projection, pass);
                }
            }
        }

        public virtual void DrawModel(Matrix world, Matrix view, Matrix projection, RenderPass pass)
        {           
        }

        public virtual void DrawModelWithEffect(Matrix world, Matrix view, Matrix projection, TextureCube reflectionTexture, Vector3 cameraPosition)
        {            
        }

        #endregion
    }
}
