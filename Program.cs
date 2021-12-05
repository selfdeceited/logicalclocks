const int REPUBLISHING_LEVEL = 1;

var emulator = new ClockEmulator();
var subjectNames = new List<string> { "P1", "P2", "P3" };


var scalarSubjects = subjectNames.Select(name => new ScalarClockSubject(name, REPUBLISHING_LEVEL));
emulator.StartSequence<int>(
    subjects: scalarSubjects.Cast<ISubject<Event<int>>>().ToList(),
    defaultClockValue: -1);

var emptyVector = new Vector(subjectNames.ToDictionary(name => name, _ => 0));
var vectorSubjects = subjectNames.Select(name => new VectorClockSubject(name, REPUBLISHING_LEVEL, emptyVector.DeepClone()));

emulator.StartSequence<Vector>(
    subjects: vectorSubjects.Cast<ISubject<Event<Vector>>>().ToList(),
    defaultClockValue: emptyVector.DeepClone()
);

Console.ReadKey();