using Mzayad.Data;

namespace Mzayad.Services
{
    public abstract class ServiceBase
    {
        private readonly IDataContextFactory _dataContextFactory;

        protected IDataContext DataContext()
        {
            return _dataContextFactory.GetContext();
        }
        
        protected ServiceBase(IDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }
    }
}
