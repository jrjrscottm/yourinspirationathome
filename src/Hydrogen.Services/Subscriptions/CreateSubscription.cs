using Hydrogen.Core.Commands;
using Hydrogen.Infrastructure.Commands;
using System;

namespace Hydrogen.Services.Subscriptions
{
    public class ActivateSubscription : ICommand
    {
        public string UserId { get; set; }
        public string SubscriptionId { get; set; }
    }

    public class ActivateSubscripitionResult : ResultBase
    {

    }

    public interface ISubscriptionCreationHandler: ICommandHandler<ActivateSubscription, ActivateSubscripitionResult>
    {

    }

    public interface ISubscriptionCommandHandler :
        ISubscriptionCreationHandler
    {

    }

    public class SubscriptionCommandHandler : ISubscriptionCommandHandler
    {
        public ActivateSubscripitionResult Handle(ActivateSubscription command)
        {
            throw new NotImplementedException();
        }
    }
}
