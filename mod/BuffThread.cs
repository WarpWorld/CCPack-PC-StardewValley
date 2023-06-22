using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using StardewValley;

namespace ControlValley
{
    public class BuffThread
    {
        public static List<BuffThread> threads = new List<BuffThread>();

        private readonly Buff buff;
        public int duration;
        public int remain;
        private int id;
        private bool paused;

        public static void addTime(int duration)
        {
            try
            {
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        Interlocked.Add(ref thread.duration, duration+5);
                        if (!thread.paused)
                        {
                            int time = Volatile.Read(ref thread.remain);
                            new TimedResponse(thread.id, time, CrowdResponse.Status.STATUS_PAUSE).Send(ControlClient.Socket);
                            thread.paused = true;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                UI.ShowError(e.ToString());
            }
        }

        public static void tickTime(int duration)
        {
            try
            {
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        int time = Volatile.Read(ref thread.remain);
                        time -= duration;
                        if (time < 0) time = 0;
                        Volatile.Write(ref thread.remain, time);
                    }
                }
            }
            catch (Exception e)
            {
                UI.ShowError(e.ToString());
            }
        }

        public static void unPause()
        {
            try
            {
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        if (thread.paused)
                        {
                            int time = Volatile.Read(ref thread.remain);
                            new TimedResponse(thread.id, time, CrowdResponse.Status.STATUS_RESUME).Send(ControlClient.Socket);
                            thread.paused = false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                UI.ShowError(e.ToString());
            }
        }    
        
        public BuffThread(int id, int buff, int duration)
        {
            this.buff = new Buff(buff);
            this.duration = duration;
            this.remain = duration;
            this.id = id;
            paused = false;

            try
            {
                lock (threads)
                {
                    threads.Add(this);
                }
            }
            catch (Exception e)
            {
                UI.ShowError(e.ToString());
            }
        }

        public void Run()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            buff.addBuff();

            try
            {
                int time = Volatile.Read(ref duration); ;
                while (time > 0)
                {
                    Interlocked.Add(ref duration, -time);
                    Thread.Sleep(time);

                    time = Volatile.Read(ref duration);
                }
                buff.removeBuff();
                lock (threads)
                {
                    threads.Remove(this);
                }
                new TimedResponse(id, 0, CrowdResponse.Status.STATUS_STOP).Send(ControlClient.Socket);
            }
            catch (Exception e)
            {
                UI.ShowError(e.ToString());
            }
        }
    }
}
