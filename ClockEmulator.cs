public class ClockEmulator
{
    private static readonly Random Random = new Random();
    
    public void StartSequence<T>(List<ISubject<Event<T>>> subjects, T defaultClockValue)
    {
        // subscribe to each other
        foreach (var subject in subjects)
            foreach (var other in subjects)
                if (subject != other)
                    subject.Subscribe(other);

        
        // subscribe to its own data flow
        subjects.ForEach(subject => GetFlow<T>(defaultClockValue).Subscribe(subject));
    }
    
    private static IObservable<Event<T>> GetFlow<T>(T defaultClockValue, TimeSpan? clockJitter = null)
    {
        // let's go with persistent shift by now
        var jitter = clockJitter ?? GetDefaultJitter();

        var observables = Observable
            .Range(0, 5) // todo: parametrize
            .DelaySubscription(TimeSpan.FromMilliseconds(2)) // todo: parametrize
            .Timestamp()
            .Select(i => new Event<T>(
                new object().GetHashCode(),
                defaultClockValue,
                new[] { "original" }, // geez I want discriminated unions to happen
                i.Timestamp.DateTime))
            .Select(e => e.WithTimestamp(e.Timestamp.Add(jitter)));

        return observables;
    }

    private static TimeSpan GetDefaultJitter() => TimeSpan.FromMilliseconds(Random.Next(0, 4));
}