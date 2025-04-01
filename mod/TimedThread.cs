//this project is a retrofit, it should NOT be used as part of any example - kat

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConnectorLib.JSON;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace CrowdControl;

public sealed class TimedThread
{
    private static readonly ConcurrentDictionary<uint, TimedThread> m_threads = new();

    public EffectRequest Request { get; }
    public TimedEvent Effect { get; }
    public SITimeSpan Duration { get; }
    public SITimeSpan TimeRemaining { get; private set; }
    public bool Paused { get; private set; }

    public static bool TryEnqueue<T>(EffectRequest request, T effect, SITimeSpan duration)
        where T : TimedEvent
    {
        try
        {
            if (IsRunning<T>()) return false;
            bool result = m_threads.TryAdd(request.ID, new(request, effect, duration));
            if (result) effect.Start();
            return result;
        }
        catch (Exception e)
        {
            ModEntry.Instance.Monitor.Log(e.ToString(), LogLevel.Error);
            return false;
        }
    }

    private TimedThread(EffectRequest request, TimedEvent effect, SITimeSpan duration)
    {
        Request = request;
        Effect = effect;
        Duration = duration;
        TimeRemaining = duration;
        Paused = false;
    }

    public static bool IsRunning<T>()
        where T : TimedEvent
        => m_threads.Values.Select(t => t.Effect).OfType<T>().Any();

    public static bool IsRunning<T>(Func<T, bool> predicate)
        where T : TimedEvent
        => m_threads.Values.Select(t => t.Effect).OfType<T>().Any(predicate);

    public static void Tick(GameTime gameTime)
    {
        foreach (TimedThread thread in m_threads.Values)
        {
            if (thread.Paused) continue;
            thread.TimeRemaining -= gameTime.ElapsedGameTime;
            //ModEntry.Instance.Monitor.Log($"Time remaining: {thread.TimeRemaining.TotalSeconds}s", LogLevel.Error);
            if (thread.TimeRemaining > 0)
                thread.Effect.Tick();
            else
                thread.Stop();
        }
    }

    public void Stop()
    {
        TimeRemaining = SITimeSpan.Zero;
        m_threads.Remove(Request.ID, out _);
        ModEntry.Instance.Client?.Respond(Request, EffectStatus.Finished);
        Effect.Stop();
    }
}