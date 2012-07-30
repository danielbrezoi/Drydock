﻿using Drydock.Control;
using Drydock.Logic.InterfaceObj;
using Drydock.Render;
using Drydock.UI;
using Drydock.UI.Button;
using Project_Forge.utilities;

namespace Drydock.Logic{
    internal class EditorLogic{
        private readonly Button _b;
        private readonly CurveControllerCollection _c;
        private readonly KeyboardHandler _keyboardHandler;

        public EditorLogic(Renderer renderer){
            MouseHandler.Init(renderer.Device);
            _keyboardHandler = new KeyboardHandler(renderer);

            UIContext.Init();

            //initalize component classes
            _c = new CurveControllerCollection();
            

           /* _b = _uiContext.Add<Button>(
                new Button(
                    x: 50,
                    y: 50,
                    width: 50, 
                    height: 50,
                    layerDepth: 0.5f,
                    textureName: "box",
                    components: new IUIElementComponent[]{
                        new DraggableComponent(),
                        //new FadeComponent(FadeComponent.FadeState.Faded)
                    }
                    )
                );
            DebugTimer.Stop();
            DebugTimer.Report(":");
            _b.OnMouseEntry.Add(_b.GetComponent<FadeComponent>().ForceFadein);
            _b.OnMouseExit.Add(_b.GetComponent<FadeComponent>().ForceFadeout);
            */
        }

        public void Update(){
            MouseHandler.UpdateMouse();
            _keyboardHandler.UpdateKeyboard();
            _c.UpdateCurves();
            UIContext.Update();
        }
    }
}