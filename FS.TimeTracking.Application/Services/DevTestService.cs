#if DEBUG
using FS.TimeTracking.Shared.Interfaces.Application;
using FS.TimeTracking.Shared.Interfaces.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    public class DevTestService : IDevTestService
    {
        private readonly IRepository _repository;

        public DevTestService(IRepository repository)
            => _repository = repository;

        public Task<object> TestMethod(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<object>(1);
        }
    }
}
#endif