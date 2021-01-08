#if DEBUG
using FS.TimeTracking.Shared.Interfaces.Application;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    public class DevTestService : IDevTestService
    {
        public Task<object> TestMethod(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object>(1);
        }

        public async Task<string> LongRunningOperation(int milliseconds, string result, CancellationToken cancellationToken = default)
        {
            //try { await Task.Delay(milliseconds, cancellationToken); }
            //catch (TaskCanceledException) { }
            await Task.Delay(milliseconds, cancellationToken);
            return $"SUCCESS: {result}";
        }
    }
}
#endif