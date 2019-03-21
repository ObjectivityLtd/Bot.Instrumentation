namespace Bot.Ibex.Instrumentation.Communication
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHttpCommunication
    {
        Task<string> PostAsync(string baseEndpoint, string route, IDictionary<string, string> headers, byte[] data);
    }
}
