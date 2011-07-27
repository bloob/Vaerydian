﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Vaerydian.Screens
{
     class LoadingScreen : Screen  
    { 
        #region Fields  
 
        Thread backgroundThread;  
        EventWaitHandle backgroundThreadExit;  
        bool loadingIsSlow;  
        bool otherScreensAreGone;  

 
        Screen[] screensToLoad;  
        GameTime loadStartTime;  

        #endregion  
 
        #region Initialization  
 
 
        /// <summary>  
        /// The constructor is private: loading screens should  
        /// be activated via the static Load method instead.  
        /// </summary>  
        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow,  
                              Screen[] screensToLoad)  
        {  
            this.loadingIsSlow = loadingIsSlow;  
            this.screensToLoad = screensToLoad;  
 
            if (loadingIsSlow)  
            {  
                backgroundThread = new Thread(BackgroundWorkerThread);  
                backgroundThreadExit = new ManualResetEvent(false);  
            }  
 
        }  
 
 
        /// <summary>  
        /// Activates the loading screen.  
        /// </summary>  
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                params Screen[] screensToLoad)  
        {  
            // Tell all the current screens to transition off.  
            foreach (Screen screen in screenManager.Screens)  
                screen.ScreenState = ScreenState.Inactive;  
              
            // Create and activate the loading screen.  
            LoadingScreen loadingScreen = new LoadingScreen(screenManager,  
                                                            loadingIsSlow,  
                                                            screensToLoad);  
            
            //add the loading screen to the current screens
            screenManager.addScreen(loadingScreen);
        } 


        #endregion  
 
        #region Update and Draw  
 
 
        /// <summary>  
        /// Updates the loading screen.  
        /// </summary>  
        public override void Update(GameTime gameTime)  
        {  
            base.Update(gameTime);  
    
            // If all the previous screens have finished transitioning  
            // off, it is time to actually perform the load.  
            if (otherScreensAreGone)  
            {  
                if (backgroundThread != null)  
                {  
                    loadStartTime = gameTime;  
                    backgroundThread.Start();  
                }  

                ScreenManager.removeScreen(this);  
 
                foreach (Screen screen in screensToLoad)  
                {  
                    if (screen != null)  
                    {  
                        ScreenManager.addScreen(screen);  
                    }  
                }  
 
                // Signal the background thread to exit, then wait for it to do so.  
                if (backgroundThread != null)  
                {  
                    backgroundThreadExit.Set();  
                    backgroundThread.Join();  
                }  
                // Once the load has finished, we use ResetElapsedTime to tell  
                // the  game timing mechanism that we have just finished a very  
                // long frame, and that it should not try to catch up.  
                ScreenManager.Game.ResetElapsedTime();  
            }  
        }  
 
        /// <summary>  
        /// Worker thread draws the loading animation and updates the network  
        /// session while the load is taking place.  
        /// </summary>  
        void BackgroundWorkerThread()  
        {  
            long lastTime = Stopwatch.GetTimestamp();  
 
            // EventWaitHandle.WaitOne will return true if the exit signal has  
            // been triggered, or false if the timeout has expired. We use the  
            // timeout to update at regular intervals, then break out of the  
            // loop when we are signalled to exit.  
            while (!backgroundThreadExit.WaitOne(1000 / 20))  
            {  
                GameTime gameTime = GetGameTime(ref lastTime);  
 
                DrawLoadAnimation(gameTime);  
            }  
        }  
 
 
        /// <summary>  
        /// Works out how long it has been since the last background thread update.  
        /// </summary>  
        GameTime GetGameTime(ref long lastTime)  
        {  
            long currentTime = Stopwatch.GetTimestamp();  
            long elapsedTicks = currentTime - lastTime;  
            lastTime = currentTime;  
 
            TimeSpan elapsedTime = TimeSpan.FromTicks(elapsedTicks *  
                                                      TimeSpan.TicksPerSecond /  
                                                      Stopwatch.Frequency);  
 
            return new GameTime(loadStartTime.TotalGameTime + elapsedTime, elapsedTime);  
        }  
 
 
        /// <summary>  
        /// Calls directly into our Draw method from the background worker thread,  
        /// so as to update the load animation in parallel with the actual loading.  
        /// </summary>  
        void DrawLoadAnimation(GameTime gameTime)  
        {  
            if ((ScreenManager.GraphicsDevice == null) || ScreenManager.GraphicsDevice.IsDisposed)  
                return;  
 
            try 
            {  
                ScreenManager.GraphicsDevice.Clear(Color.Black);  
 
                // Draw the loading screen.  
                Draw(gameTime);  
 
                ScreenManager.GraphicsDevice.Present();  
            }  
            catch 
            {  
                // If anything went wrong (for instance the graphics device was lost  
                // or reset) we don't have any good way to recover while running on a  
                // background thread. Setting the device to null will stop us from  
                // rendering, so the main game can deal with the problem later on.  
                //ScreenManager.GraphicsDevice = null;  
            }  
        }  
 
        /// <summary>  
        /// Draws the loading screen.  
        /// </summary>  
        public override void Draw(GameTime gameTime)  
        {  
            // If we are the only active screen, that means all the previous screens  
            // must have finished transitioning off. We check for this in the Draw  
            // method, rather than in Update, because it isn't enough just for the  
            // screens to be gone: in order for the transition to look good we must  
            // have actually drawn a frame without them before we perform the load.  
            if ((ScreenState == ScreenState.Active) &&  
                (ScreenManager.Screens.Count == 1))  
            {  
                otherScreensAreGone = true;  
            }  
 
            // The gameplay screen takes a while to load, so we display a loading  
            // message while that is going on, but the menus load very quickly, and  
            // it would look silly if we flashed this up for just a fraction of a  
            // second while returning from the game to the menus. This parameter  
            // tells us how long the loading is going to take, so we know whether  
            // to bother drawing the message.  
            if (loadingIsSlow)  
            {  
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;  
                SpriteFont font = FontManager.Instance.Fonts["Loading"];  
 
                const string message = "Generating World...";  
   
               
 
                // Center the Loading text in the viewport.  
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;  
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);  
                Vector2 textSize = font.MeasureString(message);  
                Vector2 textPosition = (viewportSize - textSize)/2;
                Vector2 StatusMessageSize = font.MeasureString(screensToLoad[0].LoadingMessage);
                Vector2 statusPosition = (viewportSize - StatusMessageSize) / 2;
                statusPosition.Y = statusPosition.Y + 40;
 
                // Draw the text.  
                spriteBatch.Begin();
                
                //show the base message
                spriteBatch.DrawString(font, message, textPosition, Color.White);

                //show the loading message
                spriteBatch.DrawString(font, screensToLoad[0].LoadingMessage, statusPosition, Color.White);
                
                spriteBatch.End();  
            }  
        } 


        #endregion  
    }  
}  

