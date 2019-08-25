using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Windows.Media;

namespace gamework
{
    class SongResources
    {
        public List<FileInfo> difficuties;//歌曲中的各个osu难度文件
        public readonly string name;
        public readonly DirectoryInfo dir;//歌曲的大文件
        private string metadata;
        //private string timing;
        public List<Note> notes;
        public MediaPlayer bgm;
        public ImageBrush picture;
        public SongResources(DirectoryInfo songDir)
        {
            dir = songDir;
            difficuties = new List<FileInfo>(songDir.GetFiles("*.osu"));
            FileterValidMania4K();
            //载入背景图片用于选歌和之后的playing
            string events = Helper.OsuSection(metadata, "Events");
            int start = events.IndexOf("0,0,\"");
            using (StringReader sr = new StringReader(Helper.OsuSection(metadata, "Events")))
            {
                string line = sr.ReadLine();
                while ((line = sr.ReadLine())[0] == '/')
                {
                    continue;
                }
                string[] param = line.Split(new char[] { ',', '"' });
                picture = Helper.LoadImage("Songs/"+dir.ToString()+"/" + param[3]);
            }
            name = songDir.Name.Substring(songDir.Name.IndexOf(" " )+1);
        }

        //检查.osu文件是否有错
        public void FileterValidMania4K()
        {
            difficuties.RemoveAll((FileInfo f) => {
                try
                {
                    using (StreamReader sr = f.OpenText())
                    {
                        metadata = sr.ReadToEnd();
                    }
                }catch 
                {
                    Console.WriteLine("无法读取谱面文件，忽略之，相关谱面文件:{0}", f.Name);
                    return true;
                }if (Helper.OsuGetKey(metadata, "General", "Mode") != "3" || Helper.OsuGetKey(metadata, "Difficulty", "CircleSize") != "4")
                {
                    Console.WriteLine(name+difficuties+ "谱面不是osumania4K 格式");
                    return true;
                }return false;
            });
        }

        public SongResources Load(int difficultyIndex)//谱面以及音乐;用于Select界面;
        {
            using (StreamReader sr = difficuties[difficultyIndex].OpenText())
            {
                metadata = sr.ReadToEnd();
            }
            bgm = Helper.LoadMusic("Songs/" + dir.ToString() + "/" + Helper.OsuGetKey(metadata, "General", "AudioFilename"));
            return this;
        }

        public SongResources LoadAll(int difficultyIndex)//用于playing界面
        {
            Load(difficultyIndex);
            using (StringReader sr = new StringReader(Helper.OsuSection(metadata, "HitObjects")))
            {
                string line = sr.ReadLine();
                notes = new List<Note>(512);
                while ((line = sr.ReadLine()) != null)
                {
                    string[] param = line.Split(new char[] { ',', ':' });
                    Note note = null;
                    if (param[3] == "1")
                    {
                        note = new Note(Convert.ToInt32(param[0]), Convert.ToDecimal(param[2]), Note.Type.Tap);
                    }else if (param[3] == "128")
                    {
                        note = new Note(Convert.ToInt32(param[0]), Convert.ToDecimal(param[2]), Note.Type.Hold, Convert.ToDecimal(param[5]));
                    }else
                    {
                        throw new Exception("谱面包含无法解析的部分:line: "+ line);
                    }
                    notes.Add(note);
                }
            }
            return this;
        }   
    }
}
