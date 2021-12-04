public static class FlowProvider
{
    private static readonly Random Random = new Random();
    
    public static IObservable<Event<int>> GetFlow(TimeSpan? clockJitter = null)
    {
        var observables = Observable
            .Range(0, 5)
            .DelaySubscription(TimeSpan.FromMilliseconds(1))
            .Timestamp()
            .Select(i => new Event<int>(
                new object().GetHashCode(),
                -1,
                new[] {"original"},
                i.Timestamp.DateTime))
            .Select(e => e.SetTimestamp(e.Timestamp.Add(clockJitter ?? GetDefaultJitter())));

        return observables;
    }

    private static TimeSpan GetDefaultJitter() => TimeSpan.FromMilliseconds(Random.Next(0, 4));
}