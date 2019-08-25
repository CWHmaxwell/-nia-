using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace gamework
{
    class Note
    {
        public enum Type { Tap,Hold};
        public enum Status { Free,Perfect,Great,Good,Miss,Bad};
        public Status status = Status.Free;
        //长压尾判;
        public Status endstatus = Status.Free;
        public Type type;
        public readonly decimal time, endtime;
        public readonly int  column;
        public bool catched = false;
        public Note(int x,decimal t,Type notetype,decimal endt = 0)
        {
            time = t;//在什么时候生成矩形
            endtime = endt;//如果是Hold才有，表示什么时候Hold方块结束;
            type = notetype;
            column = x / 128;
        }
        public bool IsKeyValid(Key key,bool isReleasedEvent = false)
        {
            if (key != new Key[] { Key.D, Key.F, Key.J, Key.K }[column]) return false;
            if (isReleasedEvent)
            {
                return type == Type.Hold && endstatus == Status.Free;
            }
            else return status == Status.Free;
        }
        public Status? Judge(decimal t , bool isReleasedEvent ,ref int combo,ref int score)
        {
            decimal dt = t - (isReleasedEvent ? endtime : time);
            Console.WriteLine("dt:{0}", dt);
            //松手判断
            if (isReleasedEvent)
            {
                //头判没判上
                if (status == Status.Free || status == Status.Miss) return null;
                if (dt < -200)
                {
                    combo = 0;
                    endstatus = Status.Miss;
                    return endstatus;
                }
                return JudgeInterval(ref endstatus, ref combo, dt ,ref score);
            }
            else
            {
                if (dt < -200)
                {
                    return null;
                }return JudgeInterval(ref status, ref combo, dt,ref score);
            }
        }

        //不Miss判断
        private Status? JudgeInterval(ref Status s,ref int combo,decimal _dt,ref int score)
        {
            decimal dt = Math.Abs(_dt);
            if (dt <= 20)
            {
                s = Status.Perfect;
                score += 300;
            }
            else if (dt <= 50)
            {
                s = Status.Great;
                score += 300;
            }
            else if (dt <= 100)
            {
                s = Status.Good;
                score += 200;
            }
            else if (dt <= 200)
            {
                s = Status.Bad;
                score += 50;
                combo = -1;
            }
            else
            {
                s = Status.Miss;
                combo = -1;
            }
            combo++;
            score += combo;
            return s;
        }
        public bool Draw(CanvasHelper cv,decimal now,double[] segments,Dictionary<string,object> resources ,double factor = 0.7)
        {
            if (!OnScreen(now, factor)) return false;
            if (type == Type.Tap)
            {
                if (status == Status.Free) 
                    if (column == 0||column == 3)
                        cv.Image(GetNoteX(segments), GetNoteY(time, now, factor), GetNoteWidth(segments), GetNoteLength(factor), (Brush)resources["img-note1"]);
                    else
                        cv.Image(GetNoteX(segments), GetNoteY(time, now, factor), GetNoteWidth(segments), GetNoteLength(factor), (Brush)resources["img-note2"]);
            }
            else
            {
                Brush b = null;
                if (column == 0 || column == 3)
                    b = (Brush)resources["img-note1L"];
                else
                    b = (Brush)resources["img-note2L"];
                if (status == Status.Miss || endstatus == Status.Miss)
                {
                    b = b.Clone();
                    b.Opacity = 0.6;
                }
                cv.Image(GetNoteX(segments), GetNoteY(endtime, now, factor), GetNoteWidth(segments), GetNoteLength(factor), b);
            }
            return true;
        }
        private double  GetNoteLength(double factor)
        {
            if (type == Type.Tap)
            {
                return 10;
            }
            return Convert.ToDouble(endtime - time) * factor;
        }
        private double GetNoteWidth(double [] segments)
        {
            return segments[column + 1] - GetNoteX(segments);
        }
        private double GetNoteX(double[] segments)
        {
            return segments[column];
        }
        private double GetNoteY(decimal noteTime, decimal now, double factor)
        {
            return 480 - 30 - Convert.ToDouble(noteTime - now) * factor;
        }
        public bool OnScreen(decimal now, double factor)
        {
            if (GetNoteY(time, now, factor) > -30 && GetNoteY(time, now, factor) < 480 + 30)
            {
                return true;
            }
            if (type == Type.Tap) return false;
            return GetNoteY(endtime, now, factor) < 480 + 30;
        }
       
    }
}
