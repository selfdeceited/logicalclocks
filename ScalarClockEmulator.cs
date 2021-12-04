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