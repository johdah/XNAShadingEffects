using System;
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

        private EffectParameter alpha;

        private EffectParameter ambientColor;
        private EffectParameter ambientIntensity;

        private EffectParameter bumpEnabled;

        private EffectParameter directionalLightEnabled;
        private EffectParameter directionalLightDirection;
        private EffectParameter directionalLightDiffuseColor;
        private EffectParameter directionalLightSpecularColor;

        private EffectParameter diffuseDirection;
        private EffectParameter diffuseColor;
        private EffectParameter diffuseIntensity;

        private EffectParameter fogEnabled;
        private EffectParameter fogStart;
        private EffectParameter fogEnd;
        private EffectParameter fogColor;

        private EffectParameter reflectionEnabled;

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

        public Color AmbientColor
        {
            get
            {
                return new Color((Vector4)ambientColor.GetValueVector4());
            }
            set
            {
                ambientColor.SetValue(((Color)value).ToVector4());
            }
        }
        public DirectionalLight DirectionalLight0
        {
            get
            {
                return new DirectionalLight(
                    directionalLightDirection,
                    directionalLightDiffuseColor,
                    directionalLightSpecularColor,
                    (DirectionalLight)null);
            }
            set
            {
                directionalLightDiffuseColor.SetValue(value.DiffuseColor);
                directionalLightDirection.SetValue(value.Direction);
                directionalLightEnabled.SetValue(value.Enabled);
                directionalLightSpecularColor.SetValue(value.SpecularColor);
            }
        }
        public Boolean BumpEnabled
        {
            get
            {
                return bumpEnabled.GetValueBoolean();
            }
            set
            {
                bumpEnabled.SetValue(value);
            }
        }
        public Boolean ReflectionEnabled
        {
            get
            {
                return reflectionEnabled.GetValueBoolean();
            }
            set
            {
                reflectionEnabled.SetValue(value);
            }
        }

        public Boolean FogEnabled
        {
            get
            {
                return fogEnabled.GetValueBoolean();
            }
            set
            {
                fogEnabled.SetValue(value);
            }
        }
        public Color FogColor
        {
            get
            {
                return new Color((Vector3)fogColor.GetValueVector3());
            }
            set
            {
                fogColor.SetValue(((Color)value).ToVector3());
            }
        }
        public float Alpha
        {
            get
            {
                return alpha.GetValueSingle();
            }
            set
            {
                alpha.SetValue(value);
            }
        }
        public float FogStart
        {
            get
            {
                return fogStart.GetValueSingle();
            }
            set
            {
                fogStart.SetValue(value);
            }
        }
        public float FogEnd
        {
            get
            {
                return fogEnd.GetValueSingle();
            }
            set
            {
                fogEnd.SetValue(value);
            }
        }

        public Vector3 DiffuseColor
        {
            get
            {
                return diffuseColor.GetValueVector3();
            }
            set
            {
                diffuseColor.SetValue(value);
            }
        }
        public Vector3 DiffuseDirection
        {
            get
            {
                return diffuseDirection.GetValueVector3();
            }
            set
            {
                diffuseDirection.SetValue(value);
            }
        }

        public Vector4 SpecularColor
        {
            get
            {
                return specularColor.GetValueVector4();
            }
            set
            {
                specularColor.SetValue(value);
            }
        }
        public Vector3 SpecularDirection
        {
            get
            {
                return specularDirection.GetValueVector3();
            }
            set
            {
                specularDirection.SetValue(value);
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
            alpha = this.Parameters["Alpha"];
            ambientColor = this.Parameters["AmbientColor"];
            ambientIntensity = this.Parameters["AmbientIntensity"];

            bumpEnabled = this.Parameters["BumpEnabled"];

            directionalLightEnabled = this.Parameters["DirectionalLightEnabled"];
            directionalLightDirection = this.Parameters["DirectionalLightDirection"];
            directionalLightDiffuseColor = this.Parameters["DirectionalLightDiffuseColor"];
            directionalLightSpecularColor = this.Parameters["DirectionalLightSpecularColor"];

            diffuseDirection = this.Parameters["DiffuseDirection"];
            diffuseColor = this.Parameters["DiffuseColor"];
            diffuseIntensity = this.Parameters["DiffuseIntensity"];

            fogEnabled = this.Parameters["FogEnabled"];
            fogStart = this.Parameters["FogStart"];
            fogEnd = this.Parameters["FogEnd"];
            fogColor = this.Parameters["FogColor"];

            reflectionEnabled = this.Parameters["ReflectionEnabled"];

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
