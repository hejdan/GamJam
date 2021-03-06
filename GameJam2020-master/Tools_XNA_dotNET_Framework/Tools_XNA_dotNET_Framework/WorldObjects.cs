﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_XNA
{
    public class WorldObjects3D
    {
        private static Model standardGroundModel;
        private static Model fantasyGroundModel;
        private static Model scifiGroundModel;


        public static void LoadContent(ContentManager content)
        {
            // = content.Load<Model>(@"Models/");
            standardGroundModel = content.Load<Model>(@"Models/GroundStandard");
            //fantasyGroundModel = content.Load<Model>(@"Models/GroundFantasy");
            scifiGroundModel = content.Load<Model>(@"Models/GroundScifi");
        }


        public abstract class WorldSpot
        {
            public Vector3 position;

            public virtual void Draw(GameTime gameTime, Camera camera, float alpha)
            {
            }

            public virtual void Update(GameTime gameTime)
            {
            }
        }


        /// <summary>
        /// Standard ground
        /// </summary>
        public class Ground : WorldSpot
        {
            public CustomModel customModel;

            public Ground(GraphicsDevice graphicsDevice)
            {
                customModel = new CustomModel(standardGroundModel, Vector3.Zero, Vector3.Zero, Vector3.One, graphicsDevice);
            }

            public override void Draw(GameTime gameTime, Camera camera, float alpha)
            {
                customModel.Draw(camera.View, camera.Projection, camera.Position, alpha);
            }

            public override void Update(GameTime gameTime)
            {
                customModel.Position = position;
            }
        }

        public class FallingGround : Ground
        {
            /// <summary>
            /// LERP amount
            /// </summary>
            private const float fallspeed = 0.1f;
            private const float fallTo = -4000f;

            public bool falling;
            private float lastY;

            public FallingGround(GraphicsDevice graphicsDevice) : base(graphicsDevice)
            {
                falling = false;
            }

            public override void Update(GameTime gameTime)
            {
                if (falling) Fall();
                base.Update(gameTime);
            }

            private void Fall()
            {
                position.Y = MathHelper.SmoothStep(lastY, fallTo, fallspeed);
                lastY = position.Y;
            }
        }

        public class FantasyGround : FallingGround
        {
            public FantasyGround(GraphicsDevice graphicsDevice) : base(graphicsDevice)
            {
                customModel.Model = fantasyGroundModel;
            }
        }

        public class ScifiGround : FallingGround
        {
            public ScifiGround(GraphicsDevice graphicsDevice) : base(graphicsDevice)
            {
                customModel.Model = scifiGroundModel;
            }
        }

        /// <summary>
        /// An empty space in the world
        /// </summary>
        public class Air : WorldSpot
        {
            public override void Draw(GameTime gameTime, Camera camera, float alpha)
            {
            }

            public override void Update(GameTime gameTime)
            {
            }
        }
    }
}