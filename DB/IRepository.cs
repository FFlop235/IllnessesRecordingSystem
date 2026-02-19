using System.Collections.Generic;

namespace IlnessesRecordingSystem.DB;

public interface IRepository<T> where T: class
{
    List<T> GetPage(int pageIndex, int pageSize);
}