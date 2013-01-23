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
            RefCubeMap = new RenderTargetCube(GraphicsDevice, 256, false, SurfaceFormat.Color, DepthFormat.Depth16, 1, RenderTargetUsage.PreserveContents);

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

            skyboxTexture = Content.Load<TextureCube>("Skyboxes/Sunset");
            skybox = new Skybox(skyboxTexture, Content);

            Model snowplowModel = Content.Load<Model>("Models/snowplow");
            sceneManager.Scene.AddEntity(new Snowplow(this, snowplowModel));

            Model sphereModel = Content.Load<Model>("Models/sphere");
            Effect reflectionEffect = Content.Load<Effect>("Effects/reflection");
            sceneManager.Scene.AddEntity(new Sphere(this, sphereModel, reflectionEffect));
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

            TextureCube envMap = GetReflectionCube();

            skybox.Draw(view, projection, camera.Position);
            //renderManager.Draw(sceneManager.Scene, world, view, projection, skyboxTexture, camera.Position);
            renderManager.Draw(sceneManager.Scene, world, view, projection, envMap, camera.Position);

            base.Draw(gameTime);
        }

        private TextureCube GetReflectionCube()
        {
            Matrix viewMatrix = Matrix.Identity;

            // Render our cube map, once for each cube face( 6 times ).
            for (int i = 0; i < 6; i++)
            {
                // render the scene to all cubemap faces
                CubeMapFace cubeMapFace = (CubeMapFace)i;

                switch (cubeMapFace)
                {
                    case CubeMapFace.NegativeX:
                        {
                            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Left, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.NegativeY:
                        {
                            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Down, Vector3.Forward);
                            break;
                        }
                    case CubeMapFace.NegativeZ:
                        {
                            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Backward, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveX:
                        {
                            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Right, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveY:
                        {
                            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Up, Vector3.Backward);
                            break;
                        }
                    case CubeMapFace.PositiveZ:
                        {
                            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
                            break;
                        }
                }

                // Set the cubemap render target, using the selected face
                GraphicsDevice.SetRenderTarget(RefCubeMap, cubeMapFace);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                skybox.Draw(viewMatrix, projection, camera.Position);
                renderManager.Draw(sceneManager.Scene, world, viewMatrix, projection, skyboxTexture, camera.Position);
            }

            this.GraphicsDevice.SetRenderTarget(null);
            return RefCubeMap;
        }
    }
}
