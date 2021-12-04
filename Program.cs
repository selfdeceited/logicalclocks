const int REPUBLISHING_LEVEL = 1;

var scalarSubjects = new List<ScalarClockSubject>{
    new ScalarClockSubject("first", REPUBLISHING_LEVEL),
    new ScalarClockSubject("second", REPUBLISHING_LEVEL),
    new ScalarClockSubject("third", REPUBLISHING_LEVEL),
}.Cast<ISubject<Event<int>>>().ToList();

new ClockEmulator().StartSequence<int>(scalarSubjects);

Console.ReadKey();