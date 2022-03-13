namespace Saga.Orchestration.Shared
{
    public static class RabbitMqSettingsConstants
    {
        public const string OrderSaga = "order-saga-queue";
        public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
        public const string PaymentStockReservedRequestQueueName = "payment-stock-reserved-request-queue";
        public const string OrderCompletedRequestQueueName = "order-completed-request-queue";
        public const string OrderFailedRequestQueueName = "order-failed-request-queue";
        public const string StockRollBackMessageQueueName = "stock-rollback-queue";
    }
}
