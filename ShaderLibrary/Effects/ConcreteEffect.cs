﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShaderLibrary.Effects
{
    public class ConcreteEffect : Effect
    {
        #region Fields

        private List<BasicEffect> effects = new List<BasicEffect>();

        private EffectParameter ambientColor;
        private EffectParameter ambientIntensity;

        private EffectParameter diffuseDirection;
        private EffectParameter diffuseColor;
        private EffectParameter diffuseIntensity;

        private EffectParameter fogStart;
        private EffectParameter fogEnd;

        private EffectParameter specularDirection;
        private EffectParameter specularColor;
        private EffectParameter specularIntensity;

        private EffectParameter projection;
        private EffectParameter view;
        private EffectParameter world;
        private EffectParameter worldInverseTranspose;

        #endregion
        #region Properties

        public List<BasicEffect> Effects
        {
            get
            {
                return effects;
            }
            set
            {
                effects = value;
            }
        }
        public Matrix Projection
        {
            get
            {
                return projection.GetValueMatrix();
            }
            set
            {
                projection.SetValue(value);
            }
        }
        public Matrix View
        {
            get
            {
                return view.GetValueMatrix();
            }
            set
            {
                view.SetValue(value);
            }
        }
        public Matrix World
        {
            get
            {
                return world.GetValueMatrix();
            }
            set
            {
                world.SetValue(value);
            }
        }
        public Matrix WorldInverseTransposeMatrix
        {
            get
            {
                return worldInverseTranspose.GetValueMatrix();
            }
            set
            {
                worldInverseTranspose.SetValue(value);
            }
        }

        #endregion

        public ConcreteEffect(Effect source)
            : base(source)
        {
            ambientColor = this.Parameters["AmbientColor"];
            ambientIntensity = this.Parameters["AmbientIntensity"];

            diffuseDirection = this.Parameters["DiffuseDirection"];
            diffuseColor = this.Parameters["DiffuseColor"];
            diffuseIntensity = this.Parameters["DiffuseIntensity"];

            fogStart = this.Parameters["FogStart"];
            fogEnd = this.Parameters["FogEnd"];

            specularDirection = this.Parameters["SpecularDirection"];
            specularColor = this.Parameters["SpecularColor"];
            specularIntensity = this.Parameters["SpecularIntensity"];

            projection = this.Parameters["Projection"];
            view = this.Parameters["View"];
            world = this.Parameters["World"];
            worldInverseTranspose = this.Parameters["WorldInverseTranspose"];
        }
    }
}
