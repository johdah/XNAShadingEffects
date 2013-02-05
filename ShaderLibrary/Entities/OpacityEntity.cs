using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShaderLibrary.Effects;
using ShaderLibrary.Managers;

namespace ShaderLibrary.Entities
{
    public class OpacityEntity:AbstractEntity {

        private List<AccessMesh> _translucentMeshes;
        private List<AccessMesh> _opaqueMeshes;

        private List<Boolean> _isVisible;
        private Matrix[] _meshTransform;
        protected GraphicsDevice _device;
        protected Dictionary<string, Boolean> _isDoubleSided;

        public OpacityEntity(Model model, Game game)
            : base(model, game)
        {
            _device = game.GraphicsDevice;
            _translucentMeshes = new List<AccessMesh>();
            _opaqueMeshes = new List<AccessMesh>();
            _isVisible = new List<Boolean>();
            _isDoubleSided = new Dictionary<string, bool>();

            _meshTransform = new Matrix[_model.Meshes.Count];

            this.SetupModel();
        }

        private void SetupModel()
        {
            for (int i = 0; i < _meshTransform.Length; i++)
            {
                _isVisible.Add(true);
                _meshTransform[i] = Matrix.Identity;
                ModelMesh mesh = _model.Meshes[i];
                _isDoubleSided.Add(mesh.Name, false);

                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    if (PartIsTranslucent(part))
                    {
                        _translucentMeshes.Add(new AccessMesh(i, mesh));
                    }
                    else
                        _opaqueMeshes.Add(new AccessMesh(i, mesh));
                }
            }
        }                

        private bool PartIsTranslucent(ModelMeshPart modelMeshPart)
        {
            bool result = false;

            if (modelMeshPart.Effect is BasicEffect)
            {
                if ((double)((BasicEffect)modelMeshPart.Effect).Alpha < 1.0)
                    result = true;
            }
            return result;
        }

        public override void DrawModel(Matrix world, Matrix view, Matrix projection, RenderPass pass)
        {
            if (_visible)
            {
                if(pass == RenderPass.Opaque) {
                    //Draw all opaque
                    _device.BlendState = BlendState.Opaque;
                    _device.DepthStencilState = DepthStencilState.Default;
                    this.DrawMeshes(_opaqueMeshes, view, projection);
                }
                else {
                    //Draw all translucent
                    _device.BlendState = BlendState.AlphaBlend;
                    _device.DepthStencilState = DepthStencilState.DepthRead;
                    this.DrawMeshes(_translucentMeshes, view, projection);
                }
                //For future object
                _device.BlendState = BlendState.Opaque;
                _device.DepthStencilState = DepthStencilState.Default;
                _device.RasterizerState = RasterizerState.CullNone;

            }
        }

        private void DrawMeshes(List<AccessMesh> meshes, Matrix view, Matrix projection)
        {
            foreach (AccessMesh am in meshes)
            {
                if (_isVisible.ElementAt(am.MeshId))
                {
                    bool isDouble = false;
                    _isDoubleSided.TryGetValue(am.Mesh.Name, out isDouble);
                    if (isDouble)
                    {
                        _device.RasterizerState = RasterizerState.CullNone;
                        foreach (ModelMeshPart part in am.Mesh.MeshParts)
                            this.DrawMeshPart(am, part, view, projection);
                        _device.RasterizerState = RasterizerState.CullCounterClockwise;
                    }
                    else
                    {
                        _device.RasterizerState = RasterizerState.CullCounterClockwise;
                        foreach (ModelMeshPart part in am.Mesh.MeshParts)
                            this.DrawMeshPart(am, part, view, projection);
                    }

                }
            }
        }


        private void DrawMeshPart(AccessMesh am, ModelMeshPart modelMeshPart, Matrix view, Matrix projection)
        {
            if (modelMeshPart.Effect is IEffectMatrices)
            {
                IEffectMatrices effectMatrices = modelMeshPart.Effect as IEffectMatrices;
                effectMatrices.Projection = projection;
                effectMatrices.View = view;
                effectMatrices.World = _meshTransform[am.MeshId]
                    * (_model.Meshes)[am.MeshId].ParentBone.Transform
                    * Matrix.CreateTranslation(_position);
            }

            _device.SetVertexBuffer(modelMeshPart.VertexBuffer);
            _device.Indices = modelMeshPart.IndexBuffer;
            foreach (EffectPass effectPass in modelMeshPart.Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, modelMeshPart.VertexOffset,
                    modelMeshPart.StartIndex, modelMeshPart.NumVertices, modelMeshPart.StartIndex, modelMeshPart.PrimitiveCount);
            }
        }

        private struct AccessMesh
        {
            public int MeshId;
            public ModelMesh Mesh;

            public AccessMesh(int meshId, ModelMesh mesh)
            {
                this.MeshId = meshId;
                Mesh = mesh;
            }
        }

        public override void SetLighting(ConcreteEffect conceffect) {
            BasicEffect light = new BasicEffect(_device);

            light.FogEnabled = conceffect.FogEnabled;
            light.FogColor = conceffect.FogColor.ToVector3();
            light.FogStart = conceffect.FogStart;
            light.FogEnd = conceffect.FogEnd;
                        
            light.DirectionalLight0.Direction = conceffect.DirectionalLightDirection;
            light.DirectionalLight0.DiffuseColor = conceffect.DirectionalLightDiffuseColor;
            light.DirectionalLight0.SpecularColor = conceffect.DirectionalLightSpecularColor;
            light.LightingEnabled = conceffect.LightningEnabled;

            light.DirectionalLight0.Enabled = conceffect.DirectionalLightEnabled;

            foreach (ModelMesh modelMesh in _model.Meshes)
            {
                foreach (Effect effect in modelMesh.Effects)
                {
                    if (effect is IEffectFog)
                    {
                        IEffectFog fogEffect = (IEffectFog)effect;
                        fogEffect.FogColor = light.FogColor;
                        fogEffect.FogEnabled = light.FogEnabled;
                        fogEffect.FogStart = light.FogStart;
                        fogEffect.FogEnd = light.FogEnd;
                    }
                    if (effect is IEffectLights)
                    {
                        if (light.LightingEnabled)
                        {
                            IEffectLights lightEffect = (IEffectLights)effect;
                            lightEffect.AmbientLightColor = light.AmbientLightColor;
                            lightEffect.LightingEnabled = true;
                            lightEffect.DirectionalLight0.Enabled = light.DirectionalLight0.Enabled;
                            lightEffect.DirectionalLight0.Direction = light.DirectionalLight0.Direction;
                            lightEffect.DirectionalLight0.DiffuseColor = light.DirectionalLight0.DiffuseColor;
                            lightEffect.DirectionalLight0.SpecularColor = light.DirectionalLight0.SpecularColor;
                        }
                    }
                }
            }
        }
    }
}
