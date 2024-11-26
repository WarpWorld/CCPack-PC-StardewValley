﻿//this project is a retrofit, it should NOT be used as part of any example - kat

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConnectorLib.JSON;
using StardewModdingAPI;

namespace ControlValley;

public sealed class TimedThread
{
    private static readonly ConcurrentDictionary<uint, TimedThread> m_threads = new();

    public EffectRequest Request { get; }
    public TimedEvent Effect { get; }
    public SITimeSpan Duration { get; }
    public SITimeSpan TimeRemaining { get; private set; }
    public bool Paused { get; private set; }

    public static void Enqueue(EffectRequest request, TimedEvent effect, SITimeSpan? duration = null)
    {
        try { m_threads.TryAdd(request.ID, new(request, effect, duration)); }
        catch (Exception e) { ModEntry.Instance.Monitor.Log(e.ToString(), LogLevel.Error); }
    }

    private TimedThread(EffectRequest request, TimedEvent effect, SITimeSpan? duration = null)
    {
        Request = request;
        Effect = effect;
        Duration = duration ?? SITimeSpan.FromMilliseconds(request.duration ?? 0);
        TimeRemaining = Duration;
        Paused = false;
    }

    public static bool IsRunning<T>()
        where T : TimedEvent
        => m_threads.Values.Select(t => t.Effect).OfType<T>().Any();

    public static bool IsRunning<T>(Func<T, bool> predicate)
        where T : TimedEvent
        => m_threads.Values.Select(t => t.Effect).OfType<T>().Any(predicate);

    public static void Tick()
    {
        foreach (TimedThread thread in m_threads.Values)
        {
            if (!thread.Paused)
                thread.Effect.Tick();
        }
    }

    public static void AddTime(SITimeSpan duration, bool pause = false)
    {
        try
        {
            foreach (TimedThread thread in m_threads.Values)
            {
                thread.TimeRemaining += duration + 5;
                if (thread.Paused)
                    ModEntry.Instance.Client?.Respond(thread.Request, EffectStatus.Paused, thread.TimeRemaining);
                else
                {
                    if (pause)
                    {
                        ModEntry.Instance.Client?.Respond(thread.Request, EffectStatus.Paused, thread.TimeRemaining);
                        thread.Paused = true;
                    }
                    else
                        ModEntry.Instance.Client?.Respond(thread.Request, EffectStatus.Resumed, thread.TimeRemaining);
                }
            }
        }
        catch (Exception e) { ModEntry.Instance.Monitor.Log(e.ToString(), LogLevel.Error); }
    }

    public static void RemoveTime(SITimeSpan duration)
    {
        try
        {
            foreach (TimedThread thread in m_threads.Values)
            {
                thread.TimeRemaining -= duration;
                if (thread.TimeRemaining <= 0)
                    thread.Stop();
            }
        }
        catch (Exception e) { ModEntry.Instance.Monitor.Log(e.ToString(), LogLevel.Error); }
    }

    public static void Resume()
    {
        try
        {
            foreach (TimedThread thread in m_threads.Values)
            {
                if (thread.Paused)
                {
                    ModEntry.Instance.Client?.Respond(thread.Request, EffectStatus.Resumed, thread.TimeRemaining);
                    thread.Paused = false;
                }
            }
        }
        catch (Exception e) { ModEntry.Instance.Monitor.Log(e.ToString(), LogLevel.Error); }
    }

    public async Task Run()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        Effect.Start();

        try
        {
            await Task.Delay((int)TimeRemaining);
            Stop();
        }
        catch (Exception e) { ModEntry.Instance.Monitor.Log(e.ToString(), LogLevel.Error); }
    }

    public void Stop()
    {
        TimeRemaining = SITimeSpan.Zero;
        m_threads.Remove(Request.ID, out _);
        ModEntry.Instance.Client?.Respond(Request, EffectStatus.Finished);
        Effect.Stop();
    }
}