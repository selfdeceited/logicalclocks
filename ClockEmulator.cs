public class ClockEmulator
{
    private static readonly Random Random = new Random();
    
     // todo: think how to avoid defaultClockValue. Do several event types?
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
    
    private static IObservable<Event<T>> GetFlow<T>(T defaultClockValue, FlowOptions? flowOptions = null)
    {
        var options = flowOptions ?? new FlowOptions(5, TimeSpan.FromMilliseconds(2), GetDefaultJitter());

        var observables = Observable
            .Range(0, options.NumberOfEvents)
            .DelaySubscription(options.SubscriptionDelay)
            .Timestamp()
            .Select(i => new Event<T>(
                new object().GetHashCode(),
                defaultClockValue,
                new[] { "original" }, // todo: fix code smell
                i.Timestamp.DateTime))
            // let's go with persistent shift by now
            .Select(e => e.WithTimestamp(e.Timestamp.Add(options.ClockJitter)));

        return observables;
    }

    private static TimeSpan GetDefaultJitter() => TimeSpan.FromMilliseconds(Random.Next(0, 4));
}

 public record FlowOptions(int NumberOfEvents, TimeSpan SubscriptionDelay, TimeSpan ClockJitter);