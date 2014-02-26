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

namespace SpringGameJam2013
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont spriteFont;
        Texture2D background;

        //The Player object
        Player player;
        Texture2D playerTexture;
        Texture2D playerTextureLevelUp;

        //The Keyboard states
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        //The Mouse states
        MouseState  currentMouseState;
        MouseState  previousMouseState;

        //The Bullet Objects
        List<Bullet> bullets;

        //The bullet textures
        Texture2D bulletTextureSmall;
        Texture2D bulletTextureLarge;
        Vector2 bulletPositionSmall;
        Vector2 bulletPositionLarge;

        //The current bullet level
        int bulletLevel;

        //The time for firing bullets
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        //The Enemy Objects
        List<Enemy> enemies;

        //Total enemies spawned per level
        int enemyCount;
        
        //The enemy textures
        Texture2D enemyTextureSmall;
        Texture2D enemyTextureLarge;

        //The time for spawning enemies
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        //Random variable
        Random random;

        //The amount of enemies to be spawned per level (level, enemyCount)
        Dictionary<int, int> enemiesPerLevel;

        //The current level
        int currLevel;

        //The score
        int score;

        //The play button from menu
        Texture2D playButtonTexture;
        Vector2 playButtonPosition;

        //The quit button from menu
        Texture2D quitButtonTexture;
        Vector2 quitButtonPosition;

        //The game song
        Song gameMusic;
        bool musicPlaying;

        //The sleep sound
        SoundEffect sleep;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            musicPlaying = false;

            //Bullets
            bullets = new List<Bullet>();
            bulletLevel = 1;
            previousFireTime = TimeSpan.Zero;
            fireTime = TimeSpan.FromSeconds(1.0f); //How much time between each bullet fired (one second)

            //Enemies
            enemies = new List<Enemy>();
            enemyCount = 0;
            previousSpawnTime = TimeSpan.Zero;
            enemySpawnTime = TimeSpan.FromSeconds(1.5f); //How much time between each enemy spawned (two seconds)

            //Levels;
            enemiesPerLevel = new Dictionary<int, int>();
            enemiesPerLevel.Add(1, 5);
            enemiesPerLevel.Add(2, 8);
            enemiesPerLevel.Add(3, 10);
            enemiesPerLevel.Add(4, 12);
            enemiesPerLevel.Add(5, 12);
            enemiesPerLevel.Add(6, 14);
            enemiesPerLevel.Add(7, 16);
            enemiesPerLevel.Add(8, 18);
            enemiesPerLevel.Add(9, 20);
            enemiesPerLevel.Add(10, 25);
            enemiesPerLevel.Add(11, 30);
            enemiesPerLevel.Add(12, 50);

            
            
            currLevel = 0; //Initializes the level variable (0 = main menu | 1 = level 1 | 2 = level 2 ...)

            //Initializes the random variable
            random = new Random();

            //Initializes the score variable
            score = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteFont = Content.Load<SpriteFont>("Arial");
            background = Content.Load<Texture2D>("background");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Creates Player object
            playerTexture = Content.Load<Texture2D>("player");
            playerTextureLevelUp = Content.Load<Texture2D>("player_levelup");
            Vector2 playerPosition = new Vector2(0, (GraphicsDevice.Viewport.Height - playerTexture.Height) / 2);
            player = new Player(playerTexture, playerPosition, 8.0f);

            //Gets bullet textures
            bulletTextureSmall = Content.Load<Texture2D>("bullet_small");
            bulletTextureLarge = Content.Load<Texture2D>("bullet_large");
            

            //Gets the enemy textures
            enemyTextureSmall = Content.Load<Texture2D>("enemy_small");
            enemyTextureLarge = Content.Load<Texture2D>("enemy_large");

            //Gets the menu textures
            playButtonTexture = Content.Load<Texture2D>("play");
            quitButtonTexture = Content.Load<Texture2D>("quit");

            //Gets the menu positions
            playButtonPosition = new Vector2(100, GraphicsDevice.Viewport.Height / 2);
            quitButtonPosition = new Vector2(playButtonPosition.X + playButtonTexture.Width + 200, playButtonPosition.Y);

            //Gets songs/sounds
            gameMusic = Content.Load<Song>("watchingme");
            sleep = Content.Load<SoundEffect>("gotosleep_louder");

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

            //Updates the states of the keyboard
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            //Updates the states of the mouse
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            //Ctrl+R to exit (for convenience)
            if ((currentKeyboardState.IsKeyDown(Keys.LeftControl)) && (currentKeyboardState.IsKeyDown(Keys.R)))
                this.Exit();

            if (previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released)
                MouseClicked(previousMouseState.X, previousMouseState.Y);

            if (!player.Active)
            {
                player.Health = 100;
                player.Active = true;
                currLevel = 0;
                bulletLevel = 0;
            }

            //Calls the specified update methods.
            if (currLevel != 0)
            {
                if (!musicPlaying) //If music isn't playing, play it
                {
                    musicPlaying = true;
                    PlayMusic(gameMusic);
                }
                bulletPositionSmall = new Vector2(player.Position.X + player.BoundingBox.Width - bulletTextureSmall.Width, player.Position.Y + (player.BoundingBox.Height - bulletTextureSmall.Height) / 2);
                bulletPositionLarge = new Vector2(player.Position.X + player.BoundingBox.Width - bulletTextureLarge.Width, player.Position.Y + (player.BoundingBox.Height - bulletTextureLarge.Height) / 2);
  
                UpdatePlayer(gameTime);
                UpdateBullets();
                UpdateEnemies(gameTime);
                UpdateCollision(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the player
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update();
            //Controls to move
            if (currentKeyboardState.IsKeyDown(Keys.Left))
                player.Position.X -= player.Speed;
            if (currentKeyboardState.IsKeyDown(Keys.Right))
                player.Position.X += player.Speed;
            if (currentKeyboardState.IsKeyDown(Keys.Up))
                player.Position.Y -= player.Speed;
            if (currentKeyboardState.IsKeyDown(Keys.Down))
                player.Position.Y += player.Speed; 

            //Stay in bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.BoundingBox.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.BoundingBox.Height);

            if (currLevel > 1 && enemyCount < 3)
                player.Texture = playerTextureLevelUp;
            else
                player.Texture = playerTexture;

            //Adds the bullets when pressing spacebar
            if ((currentKeyboardState.IsKeyDown(Keys.Space)) && (gameTime.TotalGameTime - previousFireTime > fireTime))
            {
                if (bulletLevel == 1)
                {
                    AddBullet(bulletTextureSmall, bulletPositionSmall, 20.0f, 10);
                }
                else if (bulletLevel == 2)
                {
                    AddBullet(bulletTextureSmall, bulletPositionSmall, 20.0f, 10);
                    fireTime = TimeSpan.FromSeconds(0.75f); //Twice the fire rate
                }
                else if (bulletLevel == 3)
                {
                    AddBullet(bulletTextureSmall, bulletPositionSmall, 20.0f, 10);
                    fireTime = TimeSpan.FromSeconds(0.75f); //Normal fire rate
                }
                /*else if (bulletLevel == 4)
                {
                    AddBullet(bulletTexture, player.Position, 20.0f, 10);
                    AddBullet(bulletTexture, player.Position, 20.0f, 10);
                    AddBullet(bulletTexture, player.Position, 20.0f, 10); //Three diagonal shot (normal damage)
                }*/
                else if (bulletLevel == 4)
                {
                    AddBullet(bulletTextureSmall, bulletPositionSmall, 20.0f, 10); //Twice the damage
                    fireTime = TimeSpan.FromSeconds(0.5f); //Twice the fire rate
                }
                else if (bulletLevel == 5)
                {
                    AddBullet(bulletTextureLarge, bulletPositionLarge, 20.0f, 20); //Twice the damage
                    fireTime = TimeSpan.FromSeconds(0.75f);
                }
                else if (bulletLevel >= 6)
                {
                    AddBullet(bulletTextureLarge, bulletPositionLarge, 20.0f, 20); //Twice the damage
                    fireTime = TimeSpan.FromSeconds(0.6f);
                }

                previousFireTime = gameTime.TotalGameTime;


            }
        }

        /// <summary>
        /// Creates and adds a bullet to the bullets list
        /// </summary>
        /// <param name="position">Starting position of the bullet</param>
        /// <param name="speed">Speed of the bullet</param>
        private void AddBullet(Texture2D texture, Vector2 position, float speed, int damage)
        {
            Bullet bullet = new Bullet(texture, position, speed, damage, GraphicsDevice.Viewport);
            bullets.Add(bullet);
        }
        
        /// <summary>
        /// Updates all of the bullets currently active
        /// </summary>
        private void UpdateBullets()
        {
            for (int i = bullets.Count() - 1; i >= 0; i--)
            {
                bullets[i].Update();
                if (!bullets[i].Active)
                    bullets.RemoveAt(i);
            }
        }

        /// <summary>
        /// Creates and adds an enemy to the enemy list
        /// </summary>
        /// <param name="texture">The texture for the enemy</param>
        /// <param name="speed">The speed of the enemy</param>
        /// <param name="damage">The damage of the enemy</param>
        /// <param name="health">The health of the enemy</param>
        private void AddEnemy(Texture2D texture, float speed, int damage, int health, int value)
        {
            Enemy enemy = new Enemy(texture, new Vector2(GraphicsDevice.Viewport.Width + texture.Width, random.Next(50, GraphicsDevice.Viewport.Height - texture.Height - 50)), speed, damage, health, value);
            enemies.Add(enemy);
            enemyCount++;
        }
        
        /// <summary>
        /// Updates all of the enemies currently active
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void UpdateEnemies(GameTime gameTime)
        {
            //If enemies per level for this level have spawned:
            if (enemiesPerLevel[currLevel] != enemyCount)
            {
                //Adds an enemy after enemy spawn time has passed
                if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
                {
                    if (currLevel == 1)
                    {
                        AddEnemy(enemyTextureSmall, 5.0f, 10, 10, 100);
                    }
                    else if (currLevel == 2)
                    {
                        AddEnemy(enemyTextureSmall, 5.0f, 10, 10, 100);
                        enemySpawnTime = TimeSpan.FromSeconds(1.0f); //Enemies spawn twice as fast
                    }
                    else if (currLevel == 3)
                    {
                        AddEnemy(enemyTextureSmall, 5.0f, 10, 10, 100);
                        enemySpawnTime = TimeSpan.FromSeconds(0.75f);
                    }
                    else if (currLevel == 4)
                    {
                        AddEnemy(enemyTextureLarge, 5.0f, 20, 20, 100);
                        enemySpawnTime = TimeSpan.FromSeconds(1.0f); //Enemies spawn twice as fast
                    }
                    else if (currLevel == 5)
                    {
                        AddEnemy(enemyTextureSmall, 5.0f, 10, 10, 100);
                        enemySpawnTime = TimeSpan.FromSeconds(0.5f); //Enemies spawn twice as fast
                    }
                    else if (currLevel == 5)
                    {
                        AddEnemy(enemyTextureLarge, 5.0f, 20, 20, 100);
                        enemySpawnTime = TimeSpan.FromSeconds(0.75f); //Enemies spawn twice as fast
                    }
                    else if (currLevel == 6)
                    {
                        AddEnemy(enemyTextureLarge, 5.0f, 20, 20, 100);
                        enemySpawnTime = TimeSpan.FromSeconds(0.5f); //Enemies spawn twice as fast
                    }
                    else if (currLevel >= 7)
                    {
                        AddEnemy(enemyTextureLarge, 5.0f, 20, 20, 100);
                        enemySpawnTime = TimeSpan.FromSeconds(0.3f); //Enemies spawn twice as fast
                    }
                    previousSpawnTime = gameTime.TotalGameTime;
                }             
            }
            else if ((enemiesPerLevel[currLevel] == enemyCount) && (enemies.Count() == 0)) //if level is over
            {
                enemyCount = 0;
                currLevel++;
                if (currLevel == 13)
                    currLevel = 0;
                //if ((bulletLevel != 4) && (currLevel % 2 == 0))
                bulletLevel++;
                }
                //Updates/Removes enemy when necessary
            for (int i = enemies.Count() - 1; i >= 0; i--)
            {
                enemies[i].Update();
                if (!enemies[i].Active)
                    enemies.RemoveAt(i);
            }
        }

        /// <summary>
        /// Detects collision between objects
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void UpdateCollision(GameTime gameTime)
        {
            foreach (Enemy enemy in enemies)
            {
                //Enemy and Bullet collision
                foreach (Bullet bullet in bullets)
                {
                    if (enemy.BoundingBox.Intersects(bullet.BoundingBox))
                    {
                        enemy.Health -= bullet.Damage;
                        sleep.Play();
                        bullet.Active = false;
                        score += enemy.Value;
                    }
                }
                //Enemy and Player Collision
                if (enemy.BoundingBox.Intersects(player.BoundingBox))
                {
                    player.Health -= enemy.Damage;
                    enemy.Active = false;
                }
          

            }
        }

        /// <summary>
        /// Detects if the mouse clicks a button
        /// </summary>
        /// <param name="x">The X-Coordinate of the mouse when clicked</param>
        /// <param name="y">The Y-Coordinate of the mouse when clicked</param>
        private void MouseClicked(int x, int y)
        {
            Rectangle mouseRect = new Rectangle(x - 5, y - 5, 10, 10);
            Rectangle playRect = new Rectangle((int)playButtonPosition.X, (int)playButtonPosition.Y, playButtonTexture.Width, playButtonTexture.Height);
            Rectangle quitRect = new Rectangle((int)quitButtonPosition.X, (int)quitButtonPosition.Y, quitButtonTexture.Width, quitButtonTexture.Height);

            if (mouseRect.Intersects(playRect))
                currLevel = 1;
            else if (mouseRect.Intersects(quitRect))
                this.Exit();
        }

        private void PlayMusic(Song song)
        {
            try
            {
                // Play the music
                MediaPlayer.Play(song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();




            if (currLevel == 0) //If in main menu
            {
                spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.Draw(playButtonTexture, playButtonPosition, Color.White);
                spriteBatch.Draw(quitButtonTexture, quitButtonPosition, Color.White);
            } else {
                spriteBatch.DrawString(spriteFont, "Level:  " + currLevel /*+ " enemyCount: " + enemyCount*/, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(spriteFont, "Health: " + player.Health, new Vector2(0, 30), Color.White);
                //Draws the player
                player.Draw(spriteBatch);

                //Draws each bullet
                foreach (Bullet bullet in bullets)
                    bullet.Draw(spriteBatch);

                //Draws each enemy
                foreach (Enemy enemy in enemies)
                    enemy.Draw(spriteBatch);
            }

            //if (currLevel > 1 && enemyCount < 3)
            //    spriteBatch.DrawString(spriteFont, "LEVEL UP", new Vector2(50, 200), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
