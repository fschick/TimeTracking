#if DEBUG
using FS.TimeTracking.Shared.Interfaces.Application.Services;
using FS.TimeTracking.Shared.Interfaces.Repository;
using FS.TimeTracking.Shared.Models.TimeTracking;
using System.Threading;
using System.Threading.Tasks;

namespace FS.TimeTracking.Application.Services
{
    public class DevTestService : IDevTestService
    {
        private readonly IRepository _repository;

        public DevTestService(IRepository repository)
            => _repository = repository;

        public async Task<object> TestMethod(CancellationToken cancellationToken = default)
        {
            await _repository.Add(new Customer { ShortName = "Test" }, cancellationToken);
            await _repository.SaveChanges(cancellationToken);
            var customer = await _repository.FirstOrDefault((Customer x) => x, cancellationToken: cancellationToken);
            return Task.FromResult<object>(1);
        }
    }
}
#endif