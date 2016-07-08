using Akka.Actor;
using Hydrogen.Core.Domain.Consultants;
using Hydrogen.Data;
using Hydrogen.Services.Actors;
using Hydrogen.Services.Events;

namespace Hydrogen.Services.Users
{
    public interface IUserService
    {
        void ConfirmRegistration(Consultant user);
    }

    public class UserService : IUserService
    {
        readonly HydrogenApplicationContext _context;
        readonly ActorService _actorService;

        public UserService(HydrogenApplicationContext context, ActorService actorService)
        {
            _actorService = actorService;
            _context = context;
        }
        public void ConfirmRegistration(Consultant user)
        {
            _context.Consultants.Add(user);
            _context.SaveChanges();

            _actorService.OnMemberRegistered(user.FirstName, user.LastName, user.ConsultantId);

            //_bus.Publish(new UserActivated { UserId = user.UserId });
        }
    }

}
