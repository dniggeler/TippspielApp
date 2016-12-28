namespace Tippspiel.Contracts
{
    public interface IAccessStats
    {
        int GetRemoteHits();
        int GetCacheHits();
    }
}
