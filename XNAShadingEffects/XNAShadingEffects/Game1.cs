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

        private Vector3 cameraPosition;
        private Vector3 cameraTarget;
        private Vector3 cameraUpVector;
        private Vector3 viewVector;

        Skybox skybox;

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

            InitializeCamera();
            renderManager = new RenderManager(this);
            sceneManager = new SceneManager(this);

            base.Initialize();
        }

        private void InitializeCamera()
        {
            Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                GraphicsDevice.DisplayMode.AspectRatio,
                0.1f,
                1000.0f,
                out projection);

            cameraPosition = new Vector3(0f, 0f, 15f);
            cameraTarget = new Vector3(0f, 0f, 0f);
            cameraUpVector = Vector3.Up;

            viewVector = Vector3.Transform(cameraTarget - cameraPosition, Matrix.CreateRotationY(0));
            viewVector.Normalize();

            Matrix.CreateLookAt(
               ref cameraPosition,
               ref cameraTarget,
               ref cameraUpVector,
               out view);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureCube skyboxTexture = Content.Load<TextureCube>("Skyboxes/Sunset");
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

            sceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            //skybox.Draw(view, projection, cameraPosition);
            renderManager.Draw(sceneManager.Scene, world, view, projection, cameraPosition);

            base.Draw(gameTime);
        }
    }
}
