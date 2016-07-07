namespace Helium.Contracts.Members
{
    public class MemberRegistered
    {
        public MemberRegistered(string firstName, string lastName, string sponsorId)
        {
            FirstName = firstName;
            LastName = lastName;
            SponsorId = sponsorId;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SponsorId { get; set; }

    }
}
