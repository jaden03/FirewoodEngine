using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace FirewoodEngine.Core
{
    using static Logging;
    internal class AudioManager
    {

        public static void Init()
        {
            var device = Alc.OpenDevice(null);
            var context = Alc.CreateContext(device, (int[])null);

            Alc.MakeContextCurrent(context);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Audio Manager Initialized");
            Console.ForegroundColor = ConsoleColor.White;


            int buffers, source;
            AL.GenBuffers(1, out buffers);
            AL.GenSources(1, out source);

            int sampleFreq = 44100;
            double dt = 2 * Math.PI / sampleFreq;
            double amp = 0.5;

            int freq = 440;
            var dataCount = sampleFreq / freq;

            var sinData = new short[dataCount];
            for (int i = 0; i < sinData.Length; ++i)
            {
                sinData[i] = (short)(amp * short.MaxValue * Math.Sin(i * dt * freq));
            }
            AL.BufferData(buffers, ALFormat.Mono16, sinData, sinData.Length, sampleFreq);
            AL.Source(source, ALSourcei.Buffer, buffers);
            AL.Source(source, ALSourceb.Looping, true);

            AL.SourcePlay(source);
            Console.ReadKey();

            ///Dispose
            if (context != ContextHandle.Zero)
            {
                Alc.MakeContextCurrent(ContextHandle.Zero);
                Alc.DestroyContext(context);
            }
            context = ContextHandle.Zero;

            if (device != IntPtr.Zero)
            {
                Alc.CloseDevice(device);
            }
            device = IntPtr.Zero;
        }
    }
}
