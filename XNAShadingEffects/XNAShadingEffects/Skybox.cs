using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Shaders
{
    public class Skybox
    {
        public Model skybox;
        private TextureCube skyboxTexture;
        private Effect skyboxEffect;
        private float size = 50f;

        public Skybox(TextureCube skyboxTexture, ContentManager Content)
        {
            skybox = Content.Load<Model>("skyboxes/cube");
            this.skyboxTexture = skyboxTexture;
            skyboxEffect = Content.Load<Effect>("skyboxes/skybox");
        }

        public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            // Go through each pass in the effect, but we know there is only one...
            foreach (EffectPass pass in skyboxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Draw all of the components of the mesh, but we know the cube really
                // only has one mesh
                foreach (ModelMesh mesh in skybox.Meshes)
                {
                    // Assign the appropriate values to each of the parameters
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = skyboxEffect;
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(size) * Matrix.CreateTranslation(cameraPosition));
                        part.Effect.Parameters["View"].SetValue(view);
                        part.Effect.Parameters["Projection"].SetValue(projection);
                        part.Effect.Parameters["SkyboxTexture"].SetValue(skyboxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    }

                    // Draw the mesh with the skybox effect
                    mesh.Draw();
                }
            }
        }
    }
}
