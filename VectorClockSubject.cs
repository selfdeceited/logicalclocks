public record Vector(Dictionary<string, int> _) // todo: consider moving to ValueTuple[]?
{
    public Vector DeepClone() {
        var innerClone = _
            .Select(x => (x.Key, x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

        return new Vector(innerClone);
    }

    public override string ToString() => string.Join(',', _.Select(c => $"\n\t{c.Key} {c.Value}")) + "\n";
    public void Set(string key, int value) =>  _[key] = value;
    public (string name, int value)[] GetAll() => _.Select(x => (x.Key, x.Value)).ToArray();
    public int Get(string name) => _[name];
}

public class VectorClockSubject : ClockSubject<Vector>
{
    public VectorClockSubject (string name, int republishingLevel, Vector localClock)
        : base(name, republishingLevel, localClock)
    {

    }

    protected override void OnNextInternal(Event<Vector> e)
    {
        var myVector = LocalClock.Get(this.Name);
        if (e.Origins.Last() == "original") {   
            LocalClock.Set(Name, myVector + 1);
        } else {
            foreach (var (remoteName, remoteValue) in e.LocalClock.GetAll()) {
                LocalClock.Set(remoteName, Math.Max(LocalClock.Get(remoteName), remoteValue));
            }
            LocalClock.Set(Name, myVector + 1);
        }
    }
}
