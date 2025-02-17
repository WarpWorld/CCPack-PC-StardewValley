namespace CrowdControl;

public abstract class TimedEvent
{
    public virtual void Start() { }

    public virtual void Tick() { }

    public virtual void Stop() { }
}