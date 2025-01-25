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
        /// <param name="Exception"></param>
        /// <param name="MethodType">Used to store the http method type like Get/Post/Put/Delete</param>
        /// <param name="Namespaces"></param>
        /// <returns></returns>
        public Task LogManager(string Controller, string Method, String UserId, Int32 Flag, String Exception, String MethodType, string Namespaces);
    }
}
