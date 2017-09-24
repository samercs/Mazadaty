using Mazadaty.Data;

namespace Mazadaty.Services.Tests.Fakes
{
    public class InMemoryDataContextFactory : IDataContextFactory
    {
        private readonly IDataContext _dataContext;
        
        public InMemoryDataContextFactory(IDataContext dataContext = null)
        {
            _dataContext = dataContext ?? new InMemoryDataContext();
        }
        
        public IDataContext GetContext()
        {
            return _dataContext;
        }
    }
}
