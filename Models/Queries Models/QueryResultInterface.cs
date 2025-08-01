namespace SpotifyRequestManagement.Models.Queries_Models
{
    public interface QueryResultInterface<T>
    {
        T[] GetResult();
    }
}
