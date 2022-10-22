using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirewoodEngine.Core;

namespace FirewoodEngine.Components
{
    using static Logging;
    internal class AudioSource : Component
    {
        public string path;
        public bool directional = false;
        public float volume = 1f;

        public AudioSource(string path)
        {
            this.path = path;
        }
        
        public void Play()
        {
            AudioManager.PlaySoundAtPosition(path, transform.position, volume, directional);
        }

    }
}
