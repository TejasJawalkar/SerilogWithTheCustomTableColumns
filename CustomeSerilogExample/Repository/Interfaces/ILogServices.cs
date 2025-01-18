namespace CustomeSerilogExample.Repository.Interfaces
{
    public interface ILogServices
    {
        /// <summary>
        /// Created Interface for loose coupling
        /// Flag property is created to store the log as per the flag 
        /// </summary>
        /// <param name="Controller"></param>
        /// <param name="Method"></param>
        /// <param name="UserId"></param>
        /// <param name="Flag"></param>
        /// <returns></returns>
        public Task LogManager(string Controller, string Method, String UserId, Int32 Flag, String Exception);
    }
}
