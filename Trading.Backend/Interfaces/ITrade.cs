using Trading.Common.Models;

namespace Trading.Backend.Interfaces
{
    public interface ITrade
    {
        Task ReceiveRecords(List<TradeRecord> records, CancellationToken ct = default);
    }
}
