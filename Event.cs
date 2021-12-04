public record Event<T>(int Id, T LocalClock, string[] Origins, DateTime Timestamp) {
    public Event<T> From(string origin) {
        return new Event<T>(Id, LocalClock, Origins.Append(origin).ToArray(), Timestamp);
    }
    public Event<T> SetTimestamp(DateTime timestamp) {
        return new Event<T>(Id, LocalClock, Origins, timestamp);
    }
    public Event<T> SetLocalClock(T localClock) {
        return new Event<T>(Id, localClock, Origins, Timestamp);
    }
}