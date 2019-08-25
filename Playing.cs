using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace gamework
{
    class Playing
    {
        //需要绘制的判定UI
        string judgeUI = null;
        int judgeUITimeout = 10;
        //需要传送给Summary的数据
        int error = 0;
        int combo = 0;
        public int score = 0;
        public int maxcom = 0;      
        public int[] statenum = { 0, 0, 0, 0, 0 };
        //按下的键,位或，从低位到高位分别是DFJK
        int keyPressed = 0;
        //stage位置，按键位置;
        //private double stageOffset = 100;
        private double[] segments = new double[] { 28, 64, 95, 126, 162 };
        //下落速度;
        public double factor = 0.2;
        //所有公共资源
        private Dictionary<string, object> resources = new Dictionary<string, object>();
        public Playing()
        {
            //载入公共资源;
            resources["img-key1"] = Helper.LoadImage("Skins/key1.png");
            resources["img-key2"] = Helper.LoadImage("Skins/key2.png");
            resources["img-key1D"] = Helper.LoadImage("Skins/key1D.png");
            resources["img-key2D"] = Helper.LoadImage("Skins/key2D.png");
            resources["img-note1"] = Helper.LoadImage("Skins/mania-note1.png");
            resources["img-note1L"] = Helper.LoadImage("Skins/mania-note1L.png");
            resources["img-note2"] = Helper.LoadImage("Skins/mania-note2.png");
            resources["img-note2L"] = Helper.LoadImage("Skins/mania-note2L.png");
            resources["img-stage"] = Helper.LoadImage("Skins/mania-stage.png");
            resources["img-light"] = Helper.LoadImage("Skins/mania-stage-light.png");
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
            resources["wav-se"] = Helper.LoadMusic("Skins/se.wav");
        }
        public SongResources song = null;//选中的歌曲;

        //int idx = 0;                      
        decimal t = 0; 
        public void OnDraw( CanvasHelper cv,ref  bool redraw,ref Game.state state)
        {
            if (redraw)
            {
                cv.cv.Children.Clear();
                
                GameStart();
                song.bgm.Position = TimeSpan.Zero;
                //idx = 0;
                redraw = false;
            }
            cv.cv.Children.Clear();
            cv.Image(0, 0, cv.width, cv.height, song.picture);
            //cv.Image(stageOffset, 0, 203, 480,(Brush) resources["img-stage"]);
            cv.Rectangle(28, 0, 162 - 28, 480, 1, Helper.Color("#fff", 0.1), Helper.Color("#000", 0.1));
            foreach (double x in segments)
            {
                cv.line(x, 0, x, 406);
            }
            for (int i = 0; i < song.notes.Count; i++)
            {
                song.notes[i].Draw(cv, t, segments, resources, factor);
            }
            t = Convert.ToDecimal(song.bgm.Position.TotalMilliseconds);
            //绘制轨道光
            for (int i = 0; i < 4; ++i)
            {
                if ((keyPressed & (1 << i)) > 0)
                {
                    cv.Image(segments[i], 406 - 350, segments[i + 1] - segments[i], 350, (Brush)resources["img-light"]);
                    if (i == 0 || i == 3)
                        cv.Image(segments[i], 413, segments[i + 1] - segments[i], 480 - 413, (Brush)resources["img-key1D"]);
                    else
                        cv.Image(segments[i], 413, segments[i + 1] - segments[i], 480 - 413, (Brush)resources["img-key2D"]);
                }
                else
                {
                    if (i == 0 || i == 3)
                        cv.Image(segments[i], 413, segments[i + 1] - segments[i], 480 - 413, (Brush)resources["img-key1"]);
                    else
                        cv.Image(segments[i], 413, segments[i + 1] - segments[i], 480 - 413, (Brush)resources["img-key2"]);
                }
            }
            //绘制combo
            { int c = combo;
                //求总位数算位置
                int n = 0;
                do
                {
                    c /= 10;
                    n++;
                } while (c != 0);

                c = combo;
                int i = 0;
                do
                {
                    cv.Image(segments[2] - 22 + 12 * n - 25 * i, 70, 25, 25, (Brush)resources["img-score-" + c % 10]);
                    ++i;
                    c /= 10;
                } while (c != 0);
            }
            //绘制分数，score += combo + note.status
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
                    cv.Image(350 + 12 * n - 25 * i, 50, 25, 25, (Brush)resources["img-score-" + s % 10]);
                    ++i;
                    s /= 10;
                } while (s != 0);
            }

            //Miss判定逻辑
            foreach (Note note in song.notes)
            {
                if (note.status == Note.Status.Free)
                {
                    if (note.time < t - 200)
                    {
                        note.status = Note.Status.Miss;
                        note.endstatus = Note.Status.Miss;
                        judgeUI = "0";
                        combo = 0;
                    }
                }else if (note.type == Note.Type.Hold && note.endstatus == Note.Status.Free)
                {
                    if (note.endtime < t - 200)
                    {
                        note.endstatus = Note.Status.Miss;
                        judgeUI = "0";
                        combo = 0;
                    }
                }    
            }
           //绘制判定UI
           if (judgeUI != null)
            {
                double x1 = segments[1] - (segments[2] - segments[1]) / 2;
                Brush b = ((Brush)resources["img-hit-" + judgeUI]).Clone();
                b.Opacity = (double)judgeUITimeout / 10;
                double scale = judgeUITimeout;
                Console.WriteLine(judgeUI);
                cv.Image(x1 - scale, 300 - scale, (segments[4] + segments[3]) / 2 - x1 + 2 * scale, 20 + 2 * scale, b);
                if (--judgeUITimeout < 5)
                {
                    ResetJudgeUI();
                }
            }
            if (song.bgm != null && song.bgm.NaturalDuration.HasTimeSpan && song.bgm.Position.TotalSeconds >= song.bgm.NaturalDuration.TimeSpan.TotalSeconds )
            {
                Thread.Sleep(3000);
                
                state = Game.state.Summary;
                redraw = true;
            }
        }
        public void ResetJudgeUI()
        {
            judgeUITimeout = 13;
            judgeUI = null;
        }
        public int FindClosestFreeNote(List<Note> notes, Key key, decimal t, bool isReleasedEvent, out Note note)
        {
            int dt = int.MaxValue;
            note = null;
            foreach (Note n in song.notes)
            {
                if (!n.IsKeyValid(key, isReleasedEvent)) continue;
                if (Math.Abs((isReleasedEvent ? n.endtime : n.time) - t) < Math.Abs(dt))
                {
                    note = n;
                    dt = Convert.ToInt32(t - (isReleasedEvent ? n.endtime : n.time));
                }
            }
            return dt;
        }
        public void OnKey(ref Game.state nowstate, ref bool redraw, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                song.bgm.Stop();
                nowstate = Game.state.Selecting;
                redraw = true;
            }
            else if (e.Key == Key.Oem3)
            {
                song.bgm.Stop();
                GameStart();
                //song.bgm.Play();
            }else if (e.Key == Key.P)
            {
                song.bgm.Stop();
                nowstate = Game.state.Summary;
                redraw = true;
            }
            else
            {
                if (e.Key == Key.D) keyPressed |= 1;
                else if (e.Key == Key.F) keyPressed |= 2;
                else if (e.Key == Key.J) keyPressed |= 4;
                else if (e.Key == Key.K) keyPressed |= 8;
                else return;
            }
            decimal t = Convert.ToDecimal(song.bgm.Position.TotalMilliseconds);
            // Key effect
            ((MediaPlayer)resources["wav-se"]).Position = TimeSpan.Zero;
            ((MediaPlayer)resources["wav-se"]).Play();
            Note note;
            int dt = FindClosestFreeNote(song.notes, e.Key, t, false, out note);
            error = dt;
            if (note != null)
            {
                if (null !=note.Judge(t,false,ref combo,ref score))
                {
                    //按键判定有效,显示判定UI
                    ShowJudgeUI(note.status);
                }
            }
        }
        public void ShowJudgeUI(Note.Status status)
        {
            ResetJudgeUI();
            switch (status)
            {
                case Note.Status.Perfect:
                    judgeUI = "300g";
                    statenum[0] += 1;
                    break;
                case Note.Status.Great:
                    judgeUI = "300";
                    statenum[1] += 1;
                    break;
                case Note.Status.Good:
                    judgeUI = "200";
                    statenum[2] += 1;
                    break;
                case Note.Status.Bad:
                    judgeUI = "50";
                    statenum[3] += 1;
                    break;
                case Note.Status.Miss:
                case Note.Status.Free:
                    statenum[4] += 1;
                    judgeUI = "0";
                    break;
            }
        }
        public void ResetPlaying()
        {
            combo = 0;
            score = 0;
            for (int i = 0; i < statenum.Count(); i++)
            {
                statenum[i] = 0;
            }
            foreach (Note n in song.notes)
            {
                n.status = Note.Status.Free;
                n.endstatus = Note.Status.Free;
            }
        }
        public void OnKeyUp(KeyEventArgs e  )
        {
            if (e.Key == Key.D) keyPressed &= ~1;
            else if (e.Key == Key.F) keyPressed &= ~2;
            else if (e.Key == Key.J) keyPressed &= ~4;
            else if (e.Key == Key.K) keyPressed &= ~8;
            else return;
            //尾判处理
            decimal t = Convert.ToDecimal(song.bgm.Position.TotalMilliseconds);
            Note note;
            int dt = FindClosestFreeNote(song.notes, e.Key, t, true, out note);
            if (note != null)
            {
                if (null != note.Judge(t, true, ref combo, ref score)) 
                {
                    // 按键判定有效，显示判定 UI
                    ShowJudgeUI(note.endstatus);
                    if (combo > maxcom)
                        maxcom = combo;
                }
            }
        }
        public void GameStart()
        {
            ResetPlaying();
            song.bgm.Play();
            
        }
    }
}
