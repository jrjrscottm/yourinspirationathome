using Hydrogen.Core.Domain.Consultants;
using Hydrogen.Core.Exceptions;
using Hydrogen.Data;
using Serilog;
using System.Linq;

namespace Hydrogen.Services
{
    public interface IConsultantAppService
    {
        Consultant FindByUserId(string userId);
    }

    public class ConsultantApplicationService : IConsultantAppService
    {
        private HydrogenApplicationContext _context;
        private readonly ILogger _log;

        public ConsultantApplicationService(
            HydrogenApplicationContext context,
            ILogger log)
        {
            _context = context;
            _log = log;
        }

        public Consultant FindByUserId(string userId)
        {
            var consultant = _context.Consultants.SingleOrDefault(c => c.UserId == userId);
            if(consultant == null)
            {
                _log.Error("Unable to find consultant {consultantId}", userId);
                throw new UnknownUserException(userId);
            }

            var user = _context.Users.SingleOrDefault(c => c.Id == userId);

            consultant.User = new UserRef(user.Id, user.Email);

            return consultant;
        }
    }
}
