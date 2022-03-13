using Automatonymous;
using Saga.Orchestration.Shared;
using Saga.Orchestration.Shared.Events;
using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.StateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }
        public Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<IStockNotReservedEvent> StockNotReservedEvent { get; set; }
        public Event<IPaymentFailedEvent> PaymentFailedEvent { get; set; }
        public State OrderCreated { get; private set; }
        public State StockReserved { get; private set; }
        public State PaymentCompleted { get; private set; }
        public State StockNotReserved { get; private set; }
        public State PaymentFailed { get; private set; }
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            // aynı order id degeri varsa herhangi bir sey yapma, yok ise yeni correlation id ile kaydet
            Event(() => OrderCreatedRequestEvent, y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId).SelectId(context => Guid.NewGuid()));
            Event(() => StockReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
            Event(() => PaymentCompletedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
            Event(() => StockNotReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            Initially(When(OrderCreatedRequestEvent)
            .Then(context =>
            {
                context.Instance.CreatedDate = DateTime.Now;
                context.Instance.BuyerId = context.Data.BuyerId;
                context.Instance.OrderId = context.Data.OrderId;
                context.Instance.CardNumber = context.Data.Payment.CardNumber;
                context.Instance.CardName = context.Data.Payment.CardName;
                context.Instance.CVV = context.Data.Payment.CVV;
                context.Instance.Expiration = context.Data.Payment.Expiration;
                context.Instance.TotalPrice = context.Data.Payment.TotalPrice;
            })
            .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent before : {context.Instance}"); })
            .Publish(context => new OrderCreatedEvent(context.Instance.CorrelationId) { OrderItems = context.Data.OrderItems })
            .TransitionTo(OrderCreated)
            .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent after : {context.Instance}"); }));

            During(OrderCreated, When(StockReservedEvent)
            .TransitionTo(StockReserved)
            .Send(new Uri($"queue:{RabbitMqSettingsConstants.PaymentStockReservedRequestQueueName}"),
                    context => new StockReservedRequestPaymentEvent(context.Instance.CorrelationId)
                    {
                        OrderItems = context.Data.OrderItems,
                        Payment = new PaymentMessage
                        {
                            CardName = context.Instance.CardName,
                            CardNumber = context.Instance.CardNumber,
                            CVV = context.Instance.CVV,
                            Expiration = context.Instance.Expiration,
                            TotalPrice = context.Instance.TotalPrice
                        },
                        BuyerId = context.Instance.BuyerId
                    }
                )
            .Then(context => { Console.WriteLine($"StockReservedEvent after : {context.Instance}"); }),

            When(StockNotReservedEvent)
            .TransitionTo(StockNotReserved)
            .Publish(context => new OrderFailedRequestEvent { OrderId = context.Instance.OrderId, FailMessage = context.Data.FailMessage })
            .Then(context => { Console.WriteLine($"OrderFailedRequestEvent after : {context.Instance}"); })
            );


            During(StockReserved, When(PaymentCompletedEvent)
            .TransitionTo(PaymentCompleted)
            .Publish(context => new OrderCompletedRequestEvent { OrderId = context.Instance.OrderId })
            .Then(context => { Console.WriteLine($"OrderCompletedRequestEvent after : {context.Instance}"); })
            .Finalize(),

             When(PaymentFailedEvent)
            .Publish(context => new OrderFailedRequestEvent { OrderId = context.Instance.OrderId, FailMessage = context.Data.FailMessage })
            .Send(new Uri($"queue:{RabbitMqSettingsConstants.StockRollBackMessageQueueName}"),
                    context => new StockRollBackMessage()
                    {
                        OrderItems = context.Data.OrderItems
                    }
                )
            .TransitionTo(PaymentFailed)
            .Then(context => { Console.WriteLine($"StockRollBackMessage after : {context.Instance}"); })
            );
        }


    }
}
