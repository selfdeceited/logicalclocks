public class LogicalClock
{
    public readonly Random random = new Random();
    public void StartMagic()
    {
        var observables = Observable
            .Range(0, 10)
            .DelaySubscription(TimeSpan.FromMilliseconds(1))
            .Timestamp()
            .Select(i => new Event(i.Value, -1, i.Timestamp.DateTime));

        var evenObserver = new LogicalClockObserver("even");
        var unevenObserver = new LogicalClockObserver("uneven");

        var evenSubject = new Subject<Event>();
        evenSubject.Subscribe(evenObserver);

        var unevenSubject = new Subject<Event>();
        unevenSubject.Subscribe(unevenObserver);

        Func<Event, bool> fiftyPercent = e => random.Next(0, 2) == 1;

        unevenSubject
            .Where(fiftyPercent)
            .Subscribe(evenSubject);

        evenSubject
            .Where(fiftyPercent)
            .Subscribe(unevenSubject);


        Action<Func<Event, bool>, Subject<Event>, LogicalClockObserver, TimeSpan> sequence = (
                Func<Event, bool> predicate,
                Subject<Event> subject,
                LogicalClockObserver observer,
                TimeSpan clockShift
            ) => {
            observables
                .Where(predicate)
                // shift clocks emulating different machines
                .Select(e => new Event(e.id, e.counter, e.timestamp.Add(clockShift)))
                .Do(e => observer.IncrementLocalClock())
                // 'original' events should use LocalClock!
                // only the one coming from other observer should be 
                .Select(e => new Event(e.id, observer.LocalClock, e.timestamp))
                .Subscribe(subject);
        };

        sequence(
            e => e.id % 2 == 0,
            evenSubject,
            evenObserver,
            TimeSpan.FromMilliseconds(2)
        );

        sequence(
            e => e.id % 2 != 0,
            unevenSubject,
            unevenObserver,
            TimeSpan.FromMilliseconds(1)
        );
    }
}

public class LogicalClockObserver: IObserver<Event>
{
    public LogicalClockObserver (string name) { Name = name; }

    public string Name { get; init; }
    public int LocalClock { get; private set; }
    public void OnNext(Event e)
    {
        if (e.counter == -1)
            throw new Exception("incorrect setup!");
        var hash = e.GetHashCode();
        
        Console.WriteLine($"#{hash}: {Name} observer receiving event {e.id} at {e.timestamp.ToString("ffffff")}");
        LocalClock = Math.Max(LocalClock, e.counter);
        Console.WriteLine($"#{hash}: {Name} observer clock at event is {LocalClock}. Clock incremented");
        IncrementLocalClock();
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

    public void IncrementLocalClock()
    {
        LocalClock++;
    }
}

public record Event(int id, int counter, DateTime timestamp);