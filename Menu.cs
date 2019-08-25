using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace gamework
{
    //登陆界面，主要内容包含一个背景图,一个图标表示产品，一个bgm，一个测定范围内是否有按键以进入下一阶段Select;
    class Menu
    {
        ImageBrush bgim = new ImageBrush();
        MediaPlayer bgsong = new MediaPlayer();
        
        public void OnDraw(CanvasHelper cv,ref bool redraw  )
        {
            if (redraw)
            {
                cv.cv.Children.Clear();
                bgim = Helper.LoadImage("Skins/bg.jpg");
                cv.Image(0, 0, cv.width,cv.height, bgim);
                cv.Image(cv.width / 2-100, cv.height / 2-100, 200, 200, Helper.LoadImage("start_btn.png"));             
                //cv.Rectangle(200, 50, 100, 100);
                //cv.Text(cv.width/2, 0,30,"MENU",Helper.Color("#fff"));
                //cv.Rectangle(300, 50, 100, 100);
                //cv.Text(cv.width / 2, cv.height /2, 30, "Start", Helper.Color("#fff"));
                bgsong = Helper.LoadMusic("Skins/startup.mp3");
                bgsong.Play();          
                redraw = false;
            }
            if (bgsong !=null&&bgsong.NaturalDuration.HasTimeSpan&&bgsong.Position.TotalSeconds >=bgsong.NaturalDuration.TimeSpan.TotalSeconds - 5){
                bgsong.Position = TimeSpan.Zero;
            }
            cv.Text(0, 430, 30, "按下Tab键，查看帮助\n按下Space or Enter进入游戏",Helper.Color("#fff"),640);
        }
        public void OnMouse(ref Game.state nowstate, ref bool redraw, Point p , double width_, double height_)
        {
            if (Helper.PointIn(p, width_ / 2, height_ / 2, width_ / 2 + 200, height_ / 2 + 200))
            {
                nowstate = Game.state.Selecting;
                Stop(ref redraw);
            }
        }

        public void OnKey( ref Game.state nowstate,ref bool redraw,KeyEventArgs e)
        {
            if (e.Key == Key.Space||e.Key == Key.Enter)
            {
                nowstate = Game.state.Selecting;
                Stop(ref redraw);
            }
            else if (e.Key == Key.Tab)
            {
                nowstate = Game.state.GameHelper;
                Stop(ref redraw);
            }
        }
        private void Stop(ref bool redraw)
        {
            redraw = true;
            bgsong.Stop();
        }
    }
}
