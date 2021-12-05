public class ScalarClockSubject: ClockSubject<int>
{
    public ScalarClockSubject (string name, int republishingLevel): base(name, republishingLevel, 0)
    {

    }

    protected override void OnNextInternal(Event<int> e)
    {
        if (e.Origins.Last() == "original") {
            LocalClock++;
        } else {
            LocalClock = Math.Max(LocalClock, e.LocalClock);
            LocalClock++;
        }
    }
}

