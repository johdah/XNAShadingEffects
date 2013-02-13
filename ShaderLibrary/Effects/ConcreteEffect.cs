using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShaderLibrary.Effects
{
    public class ConcreteEffect : Effect, IEffectLights, IEffectFog, IEffectMatrices
    {
        #region Fields
        private EffectParameter alpha;

        private EffectParameter ambientLightColor;
        private EffectParameter ambientIntensity;

        private EffectParameter bumpEnabled;

        private EffectParameter directionalLightEnabled;
        private EffectParameter directionalLightDirection;
        private EffectParameter directionalLightDiffuseColor;
        private EffectParameter directionalLightSpecularColor;       

        private EffectParameter diffuseDirection;
        private EffectParameter diffuseColor;
        private EffectParameter diffuseIntensity;

        private EffectParameter lightningEnabled;

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


        public Vector3 AmbientLightColor
        {
            get
            {
                return ambientLightColor.GetValueVector3();
            }
            set
            {
                ambientLightColor.SetValue(value);
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


        public bool LightingEnabled {
            get {
                return lightningEnabled.GetValueBoolean();
            }
            set {
                lightningEnabled.SetValue(value);
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

        public Vector3 FogColor
        {
            get
            {
                return fogColor.GetValueVector3();
            }
            set
            {
                fogColor.SetValue(value);
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
        public Vector3 DirectionalLightSpecularColor
        {
            get
            {
                return directionalLightSpecularColor.GetValueVector3();
            }
            set
            {
                directionalLightSpecularColor.SetValue(value);
            }
        }
        public Vector3 DirectionalLightDiffuseColor
        {
            get
            {
                return directionalLightDiffuseColor.GetValueVector3();
            }
            set
            {
                directionalLightDiffuseColor.SetValue(value);
            }
        }
        public Boolean DirectionalLightEnabled
        {
            get
            {
                return directionalLightEnabled.GetValueBoolean();
            }
            set
            {
                directionalLightEnabled.SetValue(value);
            }
        }
        public Vector3 DirectionalLightDirection
        {
            get
            {
                return directionalLightDirection.GetValueVector3();
            }
            set
            {
                directionalLightDirection.SetValue(value);
            }
        }

        #endregion

        public ConcreteEffect(Effect source)
            : base(source)
        {
            alpha = this.Parameters["Alpha"];
            ambientLightColor = this.Parameters["AmbientLightColor"];
            ambientIntensity = this.Parameters["AmbientIntensity"];

            bumpEnabled = this.Parameters["BumpEnabled"];

            directionalLightEnabled = this.Parameters["DirectionalLightEnabled"];
            directionalLightDirection = this.Parameters["DirectionalLightDirection"];
            directionalLightDiffuseColor = this.Parameters["DirectionalLightDiffuseColor"];
            directionalLightSpecularColor = this.Parameters["DirectionalLightSpecularColor"];

            diffuseDirection = this.Parameters["DiffuseDirection"];
            diffuseColor = this.Parameters["DiffuseColor"];
            diffuseIntensity = this.Parameters["DiffuseIntensity"];

            lightningEnabled = this.Parameters["LightningEnabled"];

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

        public void EnableDefaultLighting() {
            throw new NotImplementedException();
        }
        public DirectionalLight DirectionalLight1 {
            get { throw new NotImplementedException(); }
        }

        public DirectionalLight DirectionalLight2 {
            get { throw new NotImplementedException(); }
        }
    }
}
