using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Miksvel.TestProject.Cache.Context
{
    public class CacheEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTimeOffset ExpiredTime { get; set; }
    }
}
