namespace IllnessesRecordingSystem.DB;

public interface SRepository<T> where T: class
{
    T Get();
}