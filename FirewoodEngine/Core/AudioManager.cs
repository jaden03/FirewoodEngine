using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenTK;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;
using FirewoodEngine.Components;
using System.Windows.Media.Effects;

namespace FirewoodEngine.Core
{
    using static Logging;
    internal class AudioManager
    {
        public static AudioListener audioListener;

        public static void Init()
        {
            var device = Alc.OpenDevice(null);

            var context = Alc.CreateContext(device, (int[])null);
            Alc.MakeContextCurrent(context);


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Audio Manager Initialized");
            Console.ForegroundColor = ConsoleColor.White;
        }


        public static void PlaySound(string path)
        {
            Thread thread = new Thread(() => PlaySoundThread(path));
            thread.Start();
        }



        public static void PlaySoundThread(string path)
        {
            string filename = ("../../Sounds/" + path);

            int buffer, source, state;

            buffer = AL.GenBuffer();
            source = AL.GenSource();

            int channels, bits_per_sample, sample_rate;
            byte[] sound_data = LoadWave(File.Open(filename, FileMode.Open), out channels, out bits_per_sample, out sample_rate);

            AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);

            AL.Source(source, ALSourcei.Buffer, buffer);

            Vector3 zero = Vector3.Zero;
            AL.Source(source, ALSource3f.Direction, ref zero);
            AL.Source(source, ALSource3f.Velocity, ref zero);
            AL.Source(source, ALSource3f.Position, ref zero);
            AL.Source(source, ALSourceb.SourceRelative, false);
            AL.Source(source, ALSourcef.EfxAirAbsorptionFactor, 10f);


            AL.SourcePlay(source);

            do
            {
                Thread.Sleep(250);
                AL.GetSource(source, ALGetSourcei.SourceState, out state);
            }
            while ((ALSourceState)state == ALSourceState.Playing);

            AL.SourceStop(source);
            AL.DeleteSource(source);
            AL.DeleteBuffer(buffer);
        }


        


        public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }



        public static void SetListener(AudioListener listener)
        {
            if (audioListener != null)
                Error("There can only be one audio listener at a time!");

            audioListener = listener;
        }

        public static void UpdateListener(Vector3 position, Vector3 velocity, Vector3 forward, Vector3 up)
        {
            if (audioListener == null)
                Error("There is no audio listener!");

            AL.Listener(ALListener3f.Position, ref position);
            AL.Listener(ALListener3f.Velocity, ref velocity);
            AL.Listener(ALListenerfv.Orientation, ref forward, ref up);
            AL.Listener(ALListenerf.Gain, .1f <= 0 ? 0.001f : (.1f > 1 ? 1 : .1f));
        }

    }
}
