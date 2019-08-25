using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Media;
using System.Windows.Media.Imaging;

namespace gamework
{
    class Helper
    {
        static  public MediaPlayer LoadMusic(string song)
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri(song,UriKind.RelativeOrAbsolute));
            return player;
        }
        static public ImageBrush LoadImage(string path)
        {
            BitmapImage img = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            return new ImageBrush(img); 
        }

        static public bool PointIn(Point p,double x1,double y1,double x2,double y2)
        {
            return x1 <= p.X && y1 <= p.Y && x2 >= p.X && y2 >= p.Y;
        }

        static public string OsuGetKey(string osudata,string section,string key)
        {
            string sec = OsuSection(osudata, section);
            int start = sec.IndexOf(key + ":");
            return sec.Substring(start + key.Length + 1, sec.IndexOf("\n", start) - start - key.Length - 1).Trim();
        }
        static public string OsuSection(string osudata ,string section)
        {
            int start = osudata.IndexOf("[" + section + "]");
            if (osudata.IndexOf("[", start + 1) < 0)
                return osudata.Substring(start).Trim();
            return osudata.Substring(start, osudata.IndexOf("\n[", start + 1) - start).Trim();//返回了这个Section中的所有Key.
        }

        static public Brush Color(string color , double op =1)
        {
            Brush a = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            a.Opacity = op;
            return a;
        }
    }
}
