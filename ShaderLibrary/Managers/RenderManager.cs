using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShaderLibrary.Managers
{
    public class RenderManager
    {
        protected Game game;

        public RenderManager(Game game)
        {
            this.game = game;
        }

        public void Draw(Scene scene, Matrix world, Matrix view, Matrix projection, TextureCube reflectionTexture, Vector3 cameraPosition)
        {
            foreach (AbstractEntity entity in scene.Entities)
            {
                entity.Draw(world, view, projection, reflectionTexture, cameraPosition);
            }
        }
    }
}
