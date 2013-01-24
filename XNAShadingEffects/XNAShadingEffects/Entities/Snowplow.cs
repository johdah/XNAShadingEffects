using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaderLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShaderLibrary.Effects;
using ShaderLibrary.Entities;

namespace XNAShadingEffects.Entities
{
    public class Snowplow : OpacityEntity
    {
        public Snowplow(Model model, Game game, GraphicsDevice device, ConcreteEffect effect)
            : base(model, game, device)
        {
            //this._effect = effect;
            _isDoubleSided["Circle"] = true;
            _isDoubleSided["Circle.004"] = true;
            _isDoubleSided["Circle.003"] = true;
        }
    }
}
