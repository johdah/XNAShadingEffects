using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShaderLibrary.Effects;

namespace ShaderLibrary.Managers
{
    public class Scene
    {
        #region Fields

        protected List<AbstractEntity> entities;
        protected Game game;

        #endregion
        #region Properties

        public List<AbstractEntity> Entities
        {
            get
            {
                return entities;
            }
            set
            {
                entities = value;
            }
        }

        #endregion

        public Scene(Game game)
        {
            this.game = game;
            this.entities = new List<AbstractEntity>();
        }

        public void SetLightning(ConcreteEffect effect) {
            foreach(AbstractEntity item in entities) {
                item.SetLighting(effect);
            }
        }


        public void AddEntity(AbstractEntity entity)
        {
            entities.Add(entity);
        }
    }
}
