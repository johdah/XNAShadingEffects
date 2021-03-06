using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ShaderLibrary.Managers;
using XNAShadingEffects.Entities;
using Shaders;
using XNAShadingEffects.Input;
using ShaderLibrary.Entities;
using ShaderLibrary.Effects;

namespace XNAShadingEffects
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderManager renderManager;
        SceneManager sceneManager;

        Matrix projection, view;
        Matrix world = Matrix.Identity;

        FlyingCamera fcamera;
        Camera camera;

        Skybox skybox;

        Model model;
        Vector3 viewVector;
        Texture2D texture;

        Helicopter helicopta;
        Sphere sphere;

        // Reflection
        RenderTargetCube RefCubeMap;
        TextureCube skyboxTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rs;

            camera = new Camera(GraphicsDevice);
            fcamera = new FlyingCamera();

            InputHandler ip = new InputHandler(this);
            Components.Add(ip);
            Services.AddService(typeof(IInputHandler), ip);
            renderManager = new RenderManager(this);
            sceneManager = new SceneManager(this);

            // Reflection
            //RefCubeMap = new RenderTargetCube(GraphicsDevice, 256, false, SurfaceFormat.Color, DepthFormat.Depth16, 1, RenderTargetUsage.PreserveContents);
            RefCubeMap = new RenderTargetCube(this.GraphicsDevice, 256, false, SurfaceFormat.Color, DepthFormat.None);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);             
            ConcreteEffect effectastic = new ConcreteEffect(Content.Load<Effect>("Effects/effectastic"));
            effectastic.AmbientLightColor = Color.Violet.ToVector3();            

            skyboxTexture = Content.Load<TextureCube>("Skyboxes/Sunset");
            skybox = new Skybox(skyboxTexture, Content);

            Snowplow plow = new Snowplow(Content.Load<Model>("Models/snowplow"), this, effectastic);
            plow.Position += new Vector3(5,0,-2);
            sceneManager.Scene.AddEntity(plow);

            Snowplow plow2 = new Snowplow(Content.Load<Model>("Models/snowplow"), this, effectastic);
            plow2.Position += new Vector3(5,0,2);
            sceneManager.Scene.AddEntity(plow2);

            Snowplow plow3 = new Snowplow(Content.Load<Model>("Models/snowplow"), this, effectastic);
            plow3.Position += new Vector3(5, 0, -20);
            sceneManager.Scene.AddEntity(plow3);

            sphere = new Sphere(this, Content.Load<Model>("Models/Sphere/sphere_mapped"), effectastic);
            sphere.Position = Vector3.Zero;
            sceneManager.Scene.AddEntity(sphere);

            //helicopta = new Helicopter(this, Content.Load<Model>("Models/Helicopter/Helicopter"), effectastic);
            //helicopta.Position = new Vector3(-4, 10, 0);
            //sceneManager.Scene.AddEntity(helicopta);

            //Sets lightning for whole scene
            sceneManager.Scene.SetLightning(effectastic);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Read input
            IInputHandler inputHandler = (IInputHandler)Services.GetService(typeof(IInputHandler));
            inputAction(inputHandler.getUnhandledActions(), gameTime.ElapsedGameTime.Milliseconds);

            //To make the camera mov   
            camera.Update(fcamera.Position, fcamera.Rotation);

            sceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        private void inputAction(List<ActionType> actions, float elapsedTime) {
            foreach(var action in actions) {
                if(action == ActionType.Quit)
                    this.Exit();
                else
                    fcamera.PerformAction(action, elapsedTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            view = camera.ViewMatrix;
            projection = camera.ProjectionMatrix;

            //TextureCube helicoptaMap = GetReflectionCube(sphere.BoundingSphere);
            TextureCube sphereMap = GetReflectionCube(sphere.BoundingSphere);

            skybox.Draw(view, projection, camera.Position);
            //renderManager.Draw(sceneManager.Scene, world, view, projection, skyboxTexture, camera.Position);
            renderManager.Draw(sceneManager.Scene, world, view, projection, sphereMap, camera.Position, RenderPass.Opaque);
            renderManager.Draw(sceneManager.Scene, world, view, projection, sphereMap, camera.Position, RenderPass.Translucent);

            base.Draw(gameTime);
        }

        private TextureCube GetReflectionCube(BoundingSphere bounds)
        {
            Matrix viewMatrix = Matrix.Identity;
            Matrix localProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 0.01f, 1000f);

            // Render our cube map, once for each cube face( 6 times ).
            for (int i = 0; i < 6; i++)
            {
                // render the scene to all cubemap faces
                CubeMapFace cubeMapFace = (CubeMapFace)i;
                Vector3 localPos = Vector3.Zero;

                switch (cubeMapFace)
                {
                    case CubeMapFace.NegativeX:
                        viewMatrix = Matrix.CreateLookAt(localPos, localPos + Vector3.Left, Vector3.Up);
                        break;
                    case CubeMapFace.NegativeY:
                        viewMatrix = Matrix.CreateLookAt(localPos, localPos + Vector3.Down, Vector3.Forward);
                        break;
                    case CubeMapFace.NegativeZ:
                        viewMatrix = Matrix.CreateLookAt(localPos, localPos + Vector3.Backward, Vector3.Up);
                        break;
                    case CubeMapFace.PositiveX:
                        viewMatrix = Matrix.CreateLookAt(localPos, localPos + Vector3.Right, Vector3.Up);
                        break;
                    case CubeMapFace.PositiveY:
                        viewMatrix = Matrix.CreateLookAt(localPos, localPos + Vector3.Up, Vector3.Backward);
                        break;
                    case CubeMapFace.PositiveZ:
                        viewMatrix = Matrix.CreateLookAt(localPos, localPos + Vector3.Forward, Vector3.Up);
                        break;
                }

                // Set the cubemap render target, using the selected face
                GraphicsDevice.SetRenderTarget(RefCubeMap, cubeMapFace);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                skybox.Draw(viewMatrix, localProjection, localPos);
                renderManager.Draw(sceneManager.Scene, world, viewMatrix, localProjection, skyboxTexture, localPos, RenderPass.Opaque);
            }

            this.GraphicsDevice.SetRenderTarget(null);
            return RefCubeMap;
        }
    }
}
