namespace Miksvel.TestProject.Cache.Context
{
    public class CacheEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTimeOffset ExpiredTime { get; set; }
    }
}
