using MassTransit;
using Saga.Orchestration.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saga.Orchestration.Shared.Interfaces
{
    public interface IStockReservedEvent  : CorrelatedBy<Guid>
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
