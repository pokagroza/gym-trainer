namespace Application.Services;

public interface IImportMetricsService
{
    void IncrementNewWorkout();
    void IncrementNewExercise();
    void IncrementSkippedExercise();
    ImportMetricsSnapshot Snapshot();
}

public record ImportMetricsSnapshot(long NewWorkouts, long NewExercises, long SkippedExercises);

public class ImportMetricsService : IImportMetricsService
{
    private long _newWorkouts; private long _newExercises; private long _skippedExercises;
    public void IncrementNewWorkout() => System.Threading.Interlocked.Increment(ref _newWorkouts);
    public void IncrementNewExercise() => System.Threading.Interlocked.Increment(ref _newExercises);
    public void IncrementSkippedExercise() => System.Threading.Interlocked.Increment(ref _skippedExercises);
    public ImportMetricsSnapshot Snapshot() => new(_newWorkouts, _newExercises, _skippedExercises);
}
