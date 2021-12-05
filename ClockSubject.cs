public abstract class ClockSubject<T>: ISubject<Event<T>>
{
    private static readonly Random Random = new Random();
    private List<IObserver<Event<T>>> observers = new List<IObserver<Event<T>>>();
    private readonly int _republishingLevel;

    public string Name { get; init; }
    public T LocalClock { get; protected set; }
    public ClockSubject(string name, int republishingLevel, T localClock) {
        if (name == "original")
            throw new ArgumentException("invalid name - 'original'!");

        Name = name;
        LocalClock = localClock;
        _republishingLevel = republishingLevel;
    }

    public void OnError(Exception error)
    {
        Console.WriteLine($"{Name} observer - OnError:");
        Console.WriteLine("\t {0}", error);
    }

    public void OnCompleted()
    {
        Console.WriteLine($"{Name} observer - OnCompleted()");
    }

    public virtual void OnNext(Event<T> e)
    {
         // for logging only
        var eventId = e.GetHashCode().ToString().Substring(0, 6);

        NotifyEventReceived(e, eventId);

        OnNextInternal(e);

        NotifyLocalClockIncremented(e, eventId);

        Publish(e);
    }
    
    public IDisposable Subscribe(IObserver<Event<T>> observer)
    {
        observers.Add(observer);
        return new Subscription();
    }

    protected void NotifyEventReceived(Event<T> e, string hash) {
        Console.WriteLine($"#{hash}\t: {Name} observer receiving event #{e.Id} from {e.Origins.Last()} at {e.Timestamp.ToString("ffffff")}");
    }

    protected void NotifyLocalClockIncremented(Event<T> e, string hash) {
        Console.WriteLine($"#{hash}\t: {Name} observer clock at event is {LocalClock}. Clock incremented");
    }

    protected virtual void Publish(Event<T> e)
    {
        if (e.Origins.Count() > _republishingLevel)
            return;
        
        var eventToPublish = e.From(Name).WithLocalClock(LocalClock);

        foreach (var observer in observers)
        {
            if (Random.Next(0, 2) == 1)
            {
                observer.OnNext(eventToPublish);
            }
        }
    }

    protected abstract void OnNextInternal(Event<T> e);

    internal class Subscription : IDisposable
    {
        public void Dispose() { }
    }
}