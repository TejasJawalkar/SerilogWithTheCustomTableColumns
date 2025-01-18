namespace CustomeSerilogExample.Repository.Interfaces
{
    public interface ILogServices
    {
        public Task LogManager(string Controller, string Method, String UserId);
    }
}
