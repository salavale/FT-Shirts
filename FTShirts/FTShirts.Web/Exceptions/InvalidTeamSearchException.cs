namespace FTShirts.Web.Exceptions
{
    public class InvalidTeamSearchException : Exception
    {
        public string Team { get; }

        public InvalidTeamSearchException(string team, string message)
            : base(message)
        {
            Team = team;
        }
    }
}