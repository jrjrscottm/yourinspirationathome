using System;
using Akka.Actor;
using Hydrogen.Core.Domain.Consultants;
using Hydrogen.Data;
using Hydrogen.Services.Actors;
using Hydrogen.Services.Events;
using Hydrogen.Services.Payments;

namespace Hydrogen.Services.Users
{
    public interface IUserService
    {
        void ConfirmRegistration(Consultant user);
        void SubscribeNewUser(Consultant user, string paymentMethodNonce);
    }

    public class UserService : IUserService
    {
        readonly HydrogenApplicationContext _context;
        readonly ActorService _actorService;
        readonly IPaymentService _paymentService;

        public UserService(HydrogenApplicationContext context, 
            ActorService actorService, 
            IPaymentService paymentService)
        {
            _actorService = actorService;
            _context = context;
            _paymentService = paymentService;
        }
        public void ConfirmRegistration(Consultant user)
        {
            _context.Consultants.Add(user);
            _context.SaveChanges();

            _actorService.OnMemberRegistered(user.FirstName, user.LastName, user.ConsultantId);

            //_bus.Publish(new UserActivated { UserId = user.UserId });
        }

        public void SubscribeNewUser(Consultant user, string paymentMethodNonce)
        {
            _paymentService.CreatePaymentAccountSubscription(user.UserId, user.ConsultantId, user.FirstName, user.LastName, user.EmailAddress, paymentMethodNonce);
        }
    }

}
