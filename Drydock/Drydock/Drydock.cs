using Drydock.Logic;
using Drydock.Render;
using Microsoft.Xna.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace Drydock{
    public class Drydock : Game{
        private readonly GraphicsDeviceManager _graphics;
        // private EditorLogic _editorLogic;
        private EditorLogic _editorLogic;
        private Renderer _renderer;

        public Drydock(){
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this)
                        {
                            PreferredBackBufferWidth = 800,
                            PreferredBackBufferHeight = 480,
                            SynchronizeWithVerticalRetrace = false,
                        };
        }

        protected override void Initialize(){
            //   _editorLogic = new EditorLogic();
            _renderer = new Renderer(_graphics.GraphicsDevice, Content);
            _editorLogic = new EditorLogic(_renderer);
            IsMouseVisible =true;
            //var cur = new Bitmap("D:/Projects/assets/untitled-4.png", true);
            //Graphics g = Graphics.FromImage(cur);
           // IntPtr ptr = cur.GetHicon();
           // var c = new Cursor(ptr);
          //  System.Windows.Forms.Control.FromHandle(this.Window.Handle).Cursor = c;
            base.Initialize();
        }

        protected override void LoadContent(){
        }

        protected override void UnloadContent(){
        }


        protected override void Update(GameTime gameTime){
            _editorLogic.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime){
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            _renderer.Draw();
            base.Draw(gameTime);
        }
    }
}