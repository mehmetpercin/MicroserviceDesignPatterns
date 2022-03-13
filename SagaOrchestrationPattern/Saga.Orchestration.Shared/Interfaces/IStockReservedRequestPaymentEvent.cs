using MassTransit;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.Interfaces
{
    public interface IStockReservedRequestPaymentEvent : CorrelatedBy<Guid>
    {
        public string BuyerId { get; set; }
        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
