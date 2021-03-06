﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tools_XNA;

namespace GameJam2020_3D
{
    public class InGame
    {
        public Game1 game;
        private GraphicsDeviceManager graphics;

#if DEBUG
        private FreeCamera freeCamera;
        private bool freeCameraActive = false;
        private MouseState lastMouseState;
#endif

        public Collect collect = new Collect(0, false); 

        public World world;
        private IsometricCamera camera;
        private PlayerManager player;

        public InGame(Game1 game, GraphicsDeviceManager graphics)
        {
            this.game = game;
            this.graphics = graphics;
        }

        public void Initialize()
        {

#if DEBUG
            lastMouseState = Mouse.GetState();
            freeCamera = new FreeCamera(graphics.GraphicsDevice, MathHelper.ToRadians(153), MathHelper.ToRadians(5), new Vector3(1000, 1000, -2000));
#endif
        }

        public void LoadContent(ContentManager content)
        {
            // Load Models
            WorldObjects3D.LoadContent(content);
            Player.LoadContent(content);
            // Load World
            //LoadLevel(Level.CreateDefaultFalling(graphics.GraphicsDevice));
            collect.LoadContent(content);
        }

        public void LoadLevel(Level level)
        {
            // Set world
            world = level.World;
            player = new PlayerManager(this, level.StartingPosition); // NOTE: player needs to be created after world
            // Reset score
            collect.timeScore = 0;
            // Recalculate zoom level for isometric camera
            ConfigureCamera();
        }

        public void ConfigureCamera()
        {
            camera = null;
            camera = new IsometricCamera(Vector3.Zero, 10000f, 1f, float.MaxValue, graphics.GraphicsDevice);
            // Zoom camera to fit world
            // Get largest side
            float largestSide = (float)Math.Sqrt(Math.Pow(world.RealSize.Z, 2) + Math.Pow(world.RealSize.X, 2));
            // Get largest screen side
            int lsg = Math.Min(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            camera.Zoom = largestSide / lsg;
        }


        public void Update(GameTime gameTime)
        {
            world.Update(gameTime);
            player.Update(gameTime);
            collect.Update(gameTime);
            camera.Update();
#if DEBUG
            UpdateFreeCamera();
#endif

            if (player?.spotsLeft == 1 || Keyboard.GetState().IsKeyDown(Keys.Home))
            {
                // Win
                Victory(collect.timeScore);
            }

        }

#if DEBUG
        private void UpdateFreeCamera()
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.NumPad1)) freeCameraActive = true;
            if (keyState.IsKeyDown(Keys.NumPad2)) freeCameraActive = false;
            // Calculate how much the camera should rotate
            float deltaX = lastMouseState.X - mouseState.X;
            float deltaY = lastMouseState.Y - mouseState.Y;

            // Rotate camera
            freeCamera.Rotate(deltaX * 0.01f, deltaY * 0.01f);

            Vector3 translation = Vector3.Zero;

            if (keyState.IsKeyDown(Keys.W)) translation += Vector3.Forward * 10f;
            if (keyState.IsKeyDown(Keys.S)) translation += Vector3.Backward * 10f;
            if (keyState.IsKeyDown(Keys.A)) translation += Vector3.Left * 10f;
            if (keyState.IsKeyDown(Keys.D)) translation += Vector3.Right * 10f;
            if (keyState.IsKeyDown(Keys.Space)) translation += Vector3.Up * 10f;

            // Move camera
            freeCamera.Move(translation);

            // Update camera
            freeCamera.Update();

            // Update lastMouseState
            lastMouseState = mouseState;
        }

        /// <summary>
        /// Kills player and opens gameover screen
        /// </summary>
        public void GameOver(float score)
        {
            world = null;
            player = null;
            game.menuManager.gameStates = GameStates.Menu;
            game.menuManager.menu.PageSelection = (int)MenuManager.MenuState.GameOver;
            MenuManager.score = score;
        }

        /// <summary>
        /// Clears level and opens victory screen
        /// </summary>
        public void Victory(float score)
        {
            world = null;
            player = null;
            game.menuManager.gameStates = GameStates.Menu;
            game.menuManager.menu.PageSelection = (int)MenuManager.MenuState.Victory;
            MenuManager.score = score;
        }

#endif


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
#if DEBUG
            if (world != null && player != null)
            {

                if (freeCameraActive)
                {
                    world.Draw(gameTime, freeCamera, (int)player.WorldPosition.Y);
                    player.player.Draw(freeCamera);
                }
                else
                {
                    world.Draw(gameTime, camera, (int)player.WorldPosition.Y);
                    player.player.Draw(camera);
                    collect.Draw(spriteBatch);
                }
#endif
#if !DEBUG
            // World
            world.Draw(gameTime, camera);
            // UI
          
#endif
            }
        }
    }
}
