namespace Contracts.Models
{
    public class GetUserResponse
    {
        public User User { get; set; }

        public Reason FailureReason { get; set; }

        public enum Reason
        {
            None = 0,
            NoUserInHeader = 1,
            InvalidUserId,
            NotFound,
        }
    }
}
