public record Vector(Dictionary<string, int> _) {
    public Vector DeepClone() {
        var innerClone = _
            .Select(x => (x.Key, x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

        return new Vector(innerClone);
    }

    public override string ToString()
    {
        return string.Join(',', _.Select(c => $"\n\t{c.Key} {c.Value}")) + "\n";
    }
}

public class VectorClockSubject : ClockSubject<Vector>
{
    public VectorClockSubject (string name, int republishingLevel, Vector localClock)
        : base(name, republishingLevel, localClock)
    {

    }

    protected override void OnNextInternal(Event<Vector> e)
    {
        var myVector = LocalClock._.Single(x => x.Key == this.Name);
        if (e.Origins.Last() == "original") {   
            LocalClock._[myVector.Key] = myVector.Value + 1;
        } else {
            foreach (var remoteVector in e.LocalClock._){
                var localVector = LocalClock._.Single(x => x.Key == remoteVector.Key);
                LocalClock._[remoteVector.Key] = Math.Max(localVector.Value, remoteVector.Value);
            }
            LocalClock._[myVector.Key] = myVector.Value + 1;
        }
    }
}
