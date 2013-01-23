using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShaderLibrary.Effects;

namespace ShaderLibrary
{
    public abstract class AbstractEntity
    {
        #region Fields

        private Game game;
        protected Matrix localWorld = Matrix.Identity;

        protected ConcreteEffect effect;
        protected Model model;
        protected Texture2D texture;

        #endregion
        #region Properties

        public ConcreteEffect Effect
        {
            get
            {
                return effect;
            }
            set
            {
                effect = value;
            }
        }

        #endregion

        public AbstractEntity(Model model, Game game)
        {
            this.model = model;
            this.game = game;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        #region Draw

        public virtual void Draw(Matrix world, Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            if (effect != null) {
                if (cameraPosition != null)
                {
                    DrawModelWithEffect(world, view, projection, cameraPosition);
                }
                else
                {
                    DrawModelWithEffect(world, view, projection);
                }
            }
            else
            {
                DrawModel(world, view, projection);
            }
        }

        public virtual void DrawModel(Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    //basicEffect.Alpha
                    basicEffect.EnableDefaultLighting();
                    basicEffect.PreferPerPixelLighting = true;
                    basicEffect.Projection = projection;
                    basicEffect.View = view;
                    basicEffect.World = world * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }
        }

        public virtual void DrawModelWithEffect(Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Projection = projection;
                    effect.View = view;
                    effect.World = Matrix.Identity * mesh.ParentBone.Transform;
                    effect.WorldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                }
                mesh.Draw();
            }
        }

        public virtual void DrawModelWithEffect(Matrix world, Matrix view, Matrix projection, Vector3 camPos)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["CameraPosition"].SetValue(camPos);
                    effect.Parameters["FogColor"].SetValue(Color.WhiteSmoke.ToVector3());
                    effect.Parameters["FogEnd"].SetValue(20.0f);
                    effect.Parameters["FogStart"].SetValue(10.0f);
                    effect.Parameters["ModelTexture"].SetValue(texture);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["World"].SetValue((world * localWorld) * mesh.ParentBone.Transform);
                    effect.Parameters["WorldInverseTranspose"].SetValue(
                                            Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform)));
                }
                mesh.Draw();
            }
        }

        #endregion
    }
}
