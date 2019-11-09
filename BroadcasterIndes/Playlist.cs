using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
namespace Indes2
{
    class Playlist
    {
        private List<MediaElement> VideoNames = new List<MediaElement>();
        private int nextIndex;
        private int nextIndexLive;

        public Playlist()
        {
            NextIndex = 0;
            NextIndexLive = 0;
        }
        public void AddVideo(MediaElement name)
        {
            this.VideoNames.Add(name);
        }
        public void DelVideo(MediaElement name)
        {
            this.VideoNames.RemoveAt(GetIndex(name));
        }

        public List<MediaElement> GetVideoList()
        {
            return this.VideoNames;
        }

        public int Count()
        {
            return VideoNames.Count;
        }
        public int GetIndex(MediaElement name)
        {
            return VideoNames.FindIndex(a => a == name);
        }
        public int GetIndex(String name)
        {
            return VideoNames.FindIndex(a => a.Source.Segments.GetValue(a.Source.Segments.Count() - 1).ToString() == name);
        }
        public bool CheckIfPlaylistDone()
        {
            if(NextIndex >= Count())
            {
                return true;
            }
            return false;
        }
        public bool CheckIfPlaylistNotNull()
        {
            if (Count() == 0)
            {
                return false;
            }
            return true;
        }

        public bool CheckIfPlaylistLiveDone()
        {
            if (NextIndexLive >= Count())
            {
                return true;
            }
            return false;
        }


        public int NextIndex { get => nextIndex; set => nextIndex = value; }
        public int NextIndexLive { get => nextIndexLive; set => nextIndexLive = value; }
    }
}
