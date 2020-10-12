using System.Threading.Tasks;

namespace AIGuard.Broker
{
    interface IDetectPresence
    {
        public Task<IPresence> DetectPresenceAsync(string identifier);
    }
}
