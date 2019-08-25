using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace gamework
{
    class Summary
    {
        ImageBrush summaryim = new ImageBrush();
        MediaPlayer summarysong = new MediaPlayer();
        
        int[] statenum = { 0, 0, 0, 0, 0 };
        //资源
        private Dictionary<string, object> resources = new Dictionary<string, object>();
        int score = 0 ;
        int combo=0;
        int stage = 340;
        public Summary()
        {
            foreach (string a in new string[] { "a", "b", "c", "d", "s", "ss" })
            {
                resources["img-rank-" + a] = Helper.LoadImage("Skins/rank-" + a + ".png");
            }
            foreach (string a in new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })
            {
                resources["img-score-" + a] = Helper.LoadImage("Skins/score-" + a + ".png");
            }
            foreach (string a in new string[] { "0", "50", "200", "300", "300g" })
            {
                resources["img-hit-" + a] = Helper.LoadImage("Skins/hit-" + a + ".png");
            }
            summaryim = Helper.LoadImage("Skins/bg-black.png");
        }
        public void OnDraw(ref CanvasHelper cv, ref bool redraw,SongResources song)
        {
            if (redraw)
            {
                cv.cv.Children.Clear();
                cv.Image(0, 0, cv.width, cv.height, summaryim);
                cv.Image(0, 50, stage, 480, song.picture);
                //SUMMARY
                cv.Rectangle(0, 0, 640, 50, 0, null, Helper.Color("#FFC0CB", 0.4));
                cv.Text(10, 12, 40, "Summary", Helper.Color("#fff"));
                summarysong = Helper.LoadMusic("Skins/result.mp3");
                summarysong.Play();
                redraw = false;
            }
            //绘制分数，score += combo + note.status
            cv.Text(stage, 50 + 0, 40, "SCORE", Helper.Color("#fff", 0.8));
            DrawScore(ref cv, 600, 50, 25, 25, score);
            
            //绘制hit
            {
                double op = 0.2;
                do
                {
                    cv.Image(stage, 50 + 35, 65, 30, (Brush)resources["img-hit-300g"],op);
                    cv.Image(stage, 50 + 2 * 35, 65, 30, (Brush)resources["img-hit-300"],op);
                    cv.Image(stage, 50 + 3 * 35, 65, 30, (Brush)resources["img-hit-200"],op);
                    cv.Image(stage, 50 + 4 * 35, 65, 30, (Brush)resources["img-hit-50"],op);
                    cv.Image(stage, 50 + 5 * 35, 65, 30, (Brush)resources["img-hit-0"],op);
                    op += 0.1;
                } while (op == 0.8);
                DrawScore(ref cv, 600, 50 + 35, 25, 25, statenum[0]);
                DrawScore(ref cv, 600, 50 + 35*2, 25, 25, statenum[1]);
                DrawScore(ref cv, 600, 50 + 35*3, 25, 25, statenum[2]);
                DrawScore(ref cv, 600, 50 + 35*4, 25, 25, statenum[3]);
                DrawScore(ref cv, 600, 50 + 35*5, 25, 25, statenum[4]);
            }
            
            if (summarysong != null && summarysong.NaturalDuration.HasTimeSpan && summarysong.Position.TotalSeconds >= summarysong.NaturalDuration.TimeSpan.TotalSeconds - 1)
            {
                summarysong.Position = TimeSpan.Zero;
            }
        }
        public void OnMouse(ref Game.state nowstate, ref bool redraw, Point p, double width_, double height_)
        {
            if (Helper.PointIn(p, width_ / 2, height_ / 2, width_ / 2 + 40, height_ / 2 + 40))
            {
                nowstate = Game.state.Selecting;
                summarysong.Stop();
                redraw = true;
            }
        }
        public void OnKey(ref Game.state nowstate, ref bool redraw, KeyEventArgs e)
        {
            Console.WriteLine("Key Down: "+e.Key);
            if (e.Key == Key.Escape||e.Key == Key.Enter)
            {
                nowstate = Game.state.Selecting;
                redraw = true;
                summarysong.Stop();
            }
        }
        //从playing中读取各种信息;
        public void Inputcs(int score_,int combo_ ,int[] statenum_ )
        {
            score = score_;
            combo = combo_;
            for (int i = 0; i < statenum_.Count(); i++)
            {
                statenum[i] = statenum_[i];
            }
        }

        private void DrawScore(ref CanvasHelper cv, double x1,double x2,int width_,int height_,int score)
        {
            int s = score;
            int n = 0;
            do
            {
                s /= 10;
                n++;
            } while (s != 0);
            s = score;
            int i = 0;
            do
            {
                cv.Image(x1 - 25 * i,x2, width_, height_, (Brush)resources["img-score-" + s % 10]);// +12 * n
                ++i;
                s /= 10;
            } while (s != 0);
        }
    }
}
