namespace FBS.Shared.Constants
{
    public static class CacheSettings
    {
        
        public const string DistributedRedisConfiguration = "127.0.0.1";

        
        public const string DistributedRedisInstanceName = "ToT";

        
        public const int SetSlidingExpiration = 5;

      
        public const int SetAbsoluteExpiration = 15;

        public const string CacheRelatedKey = "CacheRelatedKey";

        public const int TokenExpirationTime = 1440; 
    }
}