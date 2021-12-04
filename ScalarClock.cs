using System.Diagnostics.CodeAnalysis;

public class ScalarClockEmulator
{
    public void StartSequence()
    {
        const int REPUBLISHING_LEVEL = 1;

        var subjects = new List<ScalarClockSubject>{
            new ScalarClockSubject("first", REPUBLISHING_LEVEL),
            new ScalarClockSubject("second", REPUBLISHING_LEVEL),
            new ScalarClockSubject("third", REPUBLISHING_LEVEL),
        };
 
        // subscribe to each other
        foreach (var subject in subjects)
            foreach (var other in subjects)
                if (subject != other)
                    subject.Subscribe(other);

        // subscribe to its own data flow
        subjects.ForEach(subject => FlowProvider
            .GetFlow().Subscribe(subject));
    }
}

public class ScalarClockSubject: ISubject<Event<int>>
{
    private static readonly Random Random = new Random();
    private readonly int _republishingLevel;

    private List<IObserver<Event<int>>> observers = new List<IObserver<Event<int>>>();

    public string Name { get; init; }

    public int LocalClock { get; private set; }

    public ScalarClockSubject (string name, int republishingLevel)
    {
        Name = name;
        _republishingLevel = republishingLevel;
    }

    public void OnNext(Event<int> e)
    {
        var hash = e.GetHashCode().ToString().Substring(0, 6);
        if (e.Origins.All(x => x == "original")) {
            LocalClock++;
            Console.WriteLine($"#{hash}\t: {Name} observer receiving event #{e.Id} from base at {e.Timestamp.ToString("ffffff")}");
            Console.WriteLine($"#{hash}\t: {Name} observer clock at event is {LocalClock}. Clock incremented");
        } else {
            Console.WriteLine($"#{hash}\t: {Name} observer receiving event #{e.Id} from {e.Origins.Last()} at {e.Timestamp.ToString("ffffff")}");
            LocalClock = Math.Max(LocalClock, e.LocalClock);
            LocalClock++;
            Console.WriteLine($"#{hash}\t: {Name} observer clock at event is {LocalClock}. Clock incremented");
        }

        Publish(e);
    }

    private void Publish(Event<int> e)
    {
        if (e.Origins.Count() > _republishingLevel)
            return;
        
        var eventToPublish = e
            .From(Name)
            .SetLocalClock(LocalClock);

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
}

internal class Subscription : IDisposable
{
    public void Dispose()
    {
       
    }
}