using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShaderLibrary.Managers
{
    public class SceneManager
    {
        #region Fields

        protected Game game;
        protected Scene currentScene;

        #endregion
        #region Properties

        public Scene Scene
        {
            get { return this.currentScene }
            set { this.currentScene = value }
        }

        #endregion

        public SceneManager(Game game)
        {
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            foreach (AbstractEntity entity in currentScene.Entities)
            {
                entity.Update(gameTime);
            }
        }
    }
}
