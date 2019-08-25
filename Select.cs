using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace gamework
{
    class Select
    {
        //每个歌曲有多个难度，有附带的一张图片
        #region 歌曲的公共设置(当前选中的歌曲，歌曲方块速度等)
        //选择的歌曲下落速度;
        public double factor = 0.7;
        List<SongResources> songList = new List<SongResources>();
        SongResources song = null;  //当前选中的歌曲
        private MediaPlayer player = null; //当前选中的歌曲的播放器;
        private bool fade = true;
        private int selectIndex;
        private int difficultyIndex;
        private void  LoadsongList()
        {
            DirectoryInfo songDir = new DirectoryInfo("Songs");
            foreach(DirectoryInfo dir in songDir.GetDirectories())
            {
                songList.Add(new SongResources(dir));
            }
        }
       
        #endregion
        public  Select()
        {
            LoadsongList();
        }
        public void OnDraw(ref CanvasHelper cv,ref bool redraw)
        {
            if (redraw)
            {
                redraw = false;
                cv.cv.Children.Clear();
                cv.Image(0, 0, cv.width, cv.height, (Brush)Helper.LoadImage("Skins/bg-black.png"));
                if (songList.Count == 0)
                {
                    cv.Text(320 - 80, 240 - 40, 80, "没有发现任何歌曲");
                    return;
                }
                //fade表示切歌了,重新载入song和bgm
                if (fade)
                {
                    if (player != null)
                    {
                        player.Stop();
                    }
                    song = songList[selectIndex];
                    song.picture.Opacity = 0.2;
                    fade = false;
                    song.Load(difficultyIndex);
                    player = null;
                }
                //歌曲名字
                cv.Text(0, 50, 30, song.name, Helper.Color("#fff"), 640);
                //歌曲picture
                cv.Text(208 - 40, 100 + 80 - 10, 40, "◀", Helper.Color("#ddd"), 40);
                cv.Image(208, 100, 224, 160, song.picture);
                cv.Text(208 + 224, 100 + 80 - 10, 40, "▶", Helper.Color("#ddd"), 40);
                //选择歌曲
                cv.Rectangle(0, 0, 640, 50, 0, null, Helper.Color("#FFC0CB", 0.4));
                cv.Text(10, 12, 40, "选择歌曲",Helper.Color("#fff"));
                //方块下落速度
                cv.Ellipse(640-75, 180-65, 150, 150, 1,null, Helper.Color("#fff"));
                cv.Ellipse(600, 180-30, 80,80, 1, null, Helper.Color("#FFC0CB"));
                cv.Text(600 + 20, 100 + 80 - 60, 40, "+", Helper.Color("#FFC0CB"));
                cv.Text(600+5, 100+80-5, 35, factor.ToString(), Helper.Color("#fff"), 40);
                cv.Text(600+20+5, 100 + 70+80-15, 40, "-", Helper.Color("#FFC0CB"));
                //歌曲数量
                cv.Text(640 - 80, 15, 30, string.Format("({0} / {1}) ", selectIndex + 1, songList.Count),Helper.Color("#fff"));
                
                //难度名显示
                for (int i = 0; i < song.difficuties.Count; ++i)
                {
                    string dname = song.difficuties[i].Name;
                    if (i == difficultyIndex)
                        cv.Text(100, 480 - 220 + 10 + 20 * i, 20, dname.Substring(0, dname.Length - 4), Helper.Color("#fff"), 640-200);
                    else
                        cv.Text(100, 480 - 220 + 10 + 20 * i, 20, dname.Substring(0, dname.Length - 4), Helper.Color("#666"), 640-200);
                }
                //开始按钮
                cv.Rectangle(260, 380, 120, 40, 3, Helper.Color("#aef"),Helper.Color("#000",0.3));
                cv.Text(260, 385, 40, "Start!",Helper.Color("#fff"), 120);       
            }
            if (player == null&& song.bgm.NaturalDuration.HasTimeSpan)
            {
                song.bgm.Position = TimeSpan.FromSeconds(song.bgm.NaturalDuration.TimeSpan.TotalSeconds / 4);
                song.bgm.Play();
                player = song.bgm;
            }
            if (player != null && player.Position.TotalSeconds >= player.NaturalDuration.TimeSpan.TotalSeconds - 5)
            {
                player.Position = TimeSpan.FromSeconds(player.NaturalDuration.TimeSpan.TotalSeconds / 4);
            } 
            //歌曲封面图淡入画面
            if (song.picture.Opacity < 0.8)
            {
                song.picture.Opacity += 0.05;
            }

        }





        public void OnMouse(ref Game.state state,ref bool redraw,Point p, double width_, double height_, ref Playing playing)
        {
            if (Helper.PointIn(p,260, 380, 260+120, 380+40))
            {
                state = Game.state.Playing;
                playing.song = song;
                playing.factor = factor;
                playing.song.LoadAll(difficultyIndex);
                state = Game.state.Playing;
                Stop(ref redraw);
            }
        }

        public void OnKey(ref Game.state nowstate , ref bool redraw,KeyEventArgs e,ref Playing playing)
        {
            if (e.Key == Key.Right)
            {
                selectIndex = (selectIndex + 1) % songList.Count;
                difficultyIndex = 0;
                redraw = true;
                fade = true;
            }else if (e.Key == Key.Left)
            {
                selectIndex = (selectIndex - 1 + songList.Count) % songList.Count;
                difficultyIndex = 0;
                redraw = true;
                fade = true;
            }else if (e.Key == Key.Space||e.Key == Key.Enter)
            {
                nowstate = Game.state.Playing;
                playing.song = song;
                playing.factor = factor;
                playing.song.LoadAll(difficultyIndex);
                Stop(ref redraw);              
            }else if (e.Key == Key.Escape)
            {
                nowstate = Game.state.Menu;
                Stop(ref redraw);
            }else if (e.Key == Key.Down)
            {
                difficultyIndex = (difficultyIndex + 1) % song.difficuties.Count();
                redraw = true;               
            }else if (e.Key == Key.Up)
            {
                difficultyIndex = (difficultyIndex - 1 + song.difficuties.Count()) % song.difficuties.Count();
                redraw = true;
            }
           else if (e.Key == Key.Add)
            {
                if (factor <= 1)
                {
                    factor += 0.05;
                    redraw = true;
                }
            }else if (e.Key == Key.Subtract)
            {
                if (factor >= 0.10)
                {
                    factor -= 0.05;
                    redraw = true;
                }
                
            }
        }

        private void Stop(ref bool redraw)
        {
            if (player!=null)
            player.Stop();
            redraw = true;
            fade = true;
        }
    }
}
