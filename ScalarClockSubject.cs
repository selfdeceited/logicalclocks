public class ScalarClockSubject: ISubject<Event<int>>
{
    private static readonly Random Random = new Random();
    private readonly int _republishingLevel;

    private List<IObserver<Event<int>>> observers = new List<IObserver<Event<int>>>();

    public string Name { get; init; }

    public int LocalClock { get; private set; }

    public ScalarClockSubject (string name, int republishingLevel)
    {
        if (name == "original")
            throw new ArgumentException("invalid name - 'original'!");

        Name = name;
        _republishingLevel = republishingLevel;
    }

    public void OnNext(Event<int> e)
    {
        // for logging only
        var eventId = e.GetHashCode().ToString().Substring(0, 6);

        NotifyEventReceived(e, eventId);

        if (e.Origins.Last() == "original") {
            LocalClock++;
        } else {
            LocalClock = Math.Max(LocalClock, e.LocalClock);
            LocalClock++;
        }

        NotifyLocalClockIncremented(e, eventId);

        Publish(e);
    }

    private void NotifyEventReceived(Event<int> e, string hash) {
        Console.WriteLine($"#{hash}\t: {Name} observer receiving event #{e.Id} from {e.Origins.Last()} at {e.Timestamp.ToString("ffffff")}");
    }

    private void NotifyLocalClockIncremented(Event<int> e, string hash) {
        Console.WriteLine($"#{hash}\t: {Name} observer clock at event is {LocalClock}. Clock incremented");
    }

    private void Publish(Event<int> e)
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
    
    public void OnError(Exception error)
    {
        Console.WriteLine($"{Name} observer - OnError:");
        Console.WriteLine("\t {0}", error);
    }

    public void OnCompleted()
    {
        Console.WriteLine($"{Name} observer - OnCompleted()");
    }

    public IDisposable Subscribe(IObserver<Event<int>> observer)
    {
        observers.Add(observer);
        return new Subscription();
    }

    internal class Subscription : IDisposable
    {
        public void Dispose() { }
    }
}

