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
            /*_effect = new ConcreteEffect(effect);
            this._effect.Parameters["AmbientIntensity"].SetValue(0);
            this._effect.Parameters["DiffuseIntensity"].SetValue(0);
            this._effect.Parameters["DiffuseColor"].SetValue(new Vector4(Color.Violet.ToVector3(), 0));
            this._effect.Parameters["ReflectionEnabled"].SetValue(false);
            this._effect.Parameters["SpecularIntensity"].SetValue(0);*/
            _isDoubleSided["Circle"] = true;
            _isDoubleSided["Circle.004"] = true;
            _isDoubleSided["Circle.003"] = true;
        }
    }
}
