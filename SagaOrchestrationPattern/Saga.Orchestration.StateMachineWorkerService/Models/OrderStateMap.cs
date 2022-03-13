using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Saga.Orchestration.StateMachineWorkerService.Models
{
    public class OrderStateMap : SagaClassMap<OrderStateInstance>
    {
        protected override void Configure(EntityTypeBuilder<OrderStateInstance> entity, ModelBuilder model)
        {
            entity.Property(x => x.BuyerId).HasMaxLength(100);
            entity.Property(x => x.CardName).IsRequired();
            entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
