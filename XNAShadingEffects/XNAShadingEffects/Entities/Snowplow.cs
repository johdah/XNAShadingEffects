using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaderLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShaderLibrary.Effects;

namespace XNAShadingEffects.Entities
{
    public class Snowplow : AbstractEntity
    {
        public Snowplow(Game game, Model model)
            : base(model, game)
        {

        }

        public Snowplow(Game game, Model model, Effect effect)
            : base(model, game)
        {
            this.effect = new ConcreteEffect(effect);
        }
    }
}
