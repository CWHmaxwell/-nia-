using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace gamework
{
    class GameHelper
    {
        /*ESC：返回上一个菜单
        DFJK:4K按键        `(~):重新开始游戏        P:结算游戏*/
        public void OnDraw(CanvasHelper cv,ref bool redraw)
        {
            if (redraw)
            {
                cv.cv.Children.Clear();
                cv.Image(0, 0, 640, 480, Helper.LoadImage("Skins/GameHelper.png"));
                cv.Rectangle(0, 0, 640, 50, 1, Helper.Color("#fff"),Helper.Color("#FFC0CB",0.4));
                cv.Text(10, 12, 40, "帮助", Helper.Color("#fff"));
                //返回菜单
                cv.Rectangle(253, 350, 128, 40, 3, Helper.Color("#aef"), Helper.Color("#000", 0.3));
                cv.Text(253, 350, 50, "返回主页面");
                //开始游戏
                cv.Rectangle(253 + 120+50, 350,115,40, 3, Helper.Color("#aef"), Helper.Color("#000", 0.3));
                cv.Text(253 + 120+50, 350 , 50, "开始游戏");
                //cv.Text(cv.width / 2, cv.height / 2, 50, "GameHelper",Helper.Color("#000"));
                redraw = false;
            }
            
        }

        public void OnKey(ref bool redraw,ref Game.state nowstate,KeyEventArgs e)
        {
            if (e.Key == Key.Enter||e.Key == Key.Escape)
            {
                nowstate = Game.state.Menu;
                redraw = true;
            }
        }

        public void OnMouse(ref Game.state nowstate, ref bool redraw, Point p, double width_, double height_)
        {
            if (Helper.PointIn(p, 253, 350, 253 + 128, 350 + 40))
            {
                redraw = true;
                nowstate = Game.state.Menu;
            }
            if (Helper.PointIn(p,253+120+50,350,253+120+50+115,350+40)) 
            {
                redraw = true;
                nowstate = Game.state.Selecting;
            }
        }
    }
}
