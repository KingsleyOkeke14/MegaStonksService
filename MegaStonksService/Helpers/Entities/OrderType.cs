namespace MegaStonksService.Entities.Assets
{
    public enum OrderType
    {
        MarketOrder,
        LimitOrder,
        StopOrder
    }
    public enum OrderAction
    {
        Buy,
        Sell
    }
    public enum OrderStatus
    {
        Pending,
        Executed,
        Cancelled
    }
}
