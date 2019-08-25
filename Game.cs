using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections;

//现在的第一步就是连接mainwindow和Game;
namespace gamework
{
    class Game
    {
        CanvasHelper cv;
        private state nowstate = state.Menu;
        bool redraw = true;
        Menu menu = new Menu();
        Select select = new Select();
        Playing playing = new Playing();
        Summary summary = new Summary();
        GameHelper gamehelper = new GameHelper();
        ArrayList keyStatus = new ArrayList(16);
        public enum state {Menu,Playing,Summary,Selecting,GameHelper};
        public Game (CanvasHelper cv_)
        {
            cv = cv_;
            cv.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            cv.KeyDown += CanvasKeyDown;
            cv.KeyUp += Canvas_KeyUp;
            nowstate = state.Menu;
        }
        //鼠标事件以及键盘事件
        #region
        public void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Key Down: " + e.Key);
            if (keyStatus.Contains(e.Key)) return;
            else keyStatus.Add(e.Key);
            switch (nowstate)
            {
                case state.Menu:menu.OnKey(ref nowstate,ref redraw, e);
                    break;
                case state.Selecting:
                    select.OnKey(ref nowstate, ref redraw, e,ref playing);
                    break;
                case state.Playing:playing.OnKey(ref nowstate,ref redraw,e);
                    break;
                case state.Summary:summary.OnKey(ref nowstate, ref redraw, e);
                    break;
                case state.GameHelper:
                    gamehelper.OnKey(ref redraw,ref nowstate, e);
                    break;
                default:Console.WriteLine("状态出错{0}", nowstate); break;
            }
        }
        private void Canvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (keyStatus.Contains(e.Key)) keyStatus.Remove(e.Key);
            switch (nowstate)
            {
                case state.Playing:playing.OnKeyUp(e);break;
            }
        }
        private void Canvas_MouseLeftButtonUp(object sender, CanvasHelper.PointEventArg e)
        {
            switch (nowstate)
            {
                case state.Menu: menu.OnMouse(ref nowstate, ref redraw, e.point, cv.width, cv.height); break;
                case state.Playing: break;
                case state.Selecting: select.OnMouse(ref nowstate, ref redraw, e.point, cv.width, cv.height,ref playing); break;
                case state.Summary: summary.OnMouse(ref nowstate, ref redraw, e.point, cv.width, cv.height); break;
                case state.GameHelper:gamehelper.OnMouse(ref nowstate, ref redraw, e.point, cv.width, cv.height); break;
            }
        }
        #endregion
        public void initialze()
        {
            cv.SetRangle(640, 480);
            cv.Text(cv.width / 2 , cv.height / 2, 80, "Load...",Helper.Color("#fff"));            
        }
        public void ChangeState(state target)
        {
            nowstate = target;
            redraw = true;
        }



        

        public void OnUpdate()
        {
            switch (nowstate)
            {
                case state.Menu:menu.OnDraw( cv,ref  redraw); break;
                case state.Playing: playing.OnDraw(cv,ref  redraw, ref nowstate); break;
                case state.Selecting: select.OnDraw(ref cv, ref redraw); break;
                case state.Summary: summary.Inputcs(playing.score, playing.maxcom,playing.statenum); summary.OnDraw(ref cv, ref redraw,playing.song); break;
                case state.GameHelper:gamehelper.OnDraw(cv,ref redraw);break;
               
            }
        }
        
       
    }
}
