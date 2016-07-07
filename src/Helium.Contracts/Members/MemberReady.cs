namespace Helium.Contracts.Members
{
    public class MemberReady
    {
        public string MemberId { get; }
        public string MemberPath { get; }

        public MemberReady(string memberId, string path)
        {
            MemberId = memberId;
            MemberPath = path;
        }
    }
}
