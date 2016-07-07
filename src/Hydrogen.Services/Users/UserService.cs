using Hydrogen.Core.Domain.Consultants;
using Hydrogen.Data;
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

        public UserService(HydrogenApplicationContext context)
        {
            
            _context = context;
        }
        public void ConfirmRegistration(Consultant user)
        {
            _context.Consultants.Add(user);
            _context.SaveChanges();

            //_bus.Publish(new UserActivated { UserId = user.UserId });
        }
    }

}
