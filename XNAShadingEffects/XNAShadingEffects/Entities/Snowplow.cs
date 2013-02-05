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
        public Snowplow(Model model, Game game, ConcreteEffect effect)
            : base(model, game)
        {
           // _effect = new ConcreteEffect(effect);
            //this.SetLighting(effect);
            //_effect.ReflectionEnabled = true;
            //_effect.BumpEnabled = true;
            //_effect.FogEnabled = false;

            _isDoubleSided["Circle"] = true;
            _isDoubleSided["Circle.004"] = true;
            _isDoubleSided["Circle.003"] = true;
        }
    }
}
