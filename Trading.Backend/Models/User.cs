using Trading.Common.Models;

namespace Trading.Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? UserId {  get; set; }
        public string? Name { get; set; }
        public List<TradeRecord>? TradeRecords { get; set; }
    }
}
