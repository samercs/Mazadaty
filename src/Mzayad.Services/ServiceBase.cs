using Mzayad.Data;

namespace Mzayad.Services
{
    public abstract class ServiceBase
    {
        protected readonly IDataContextFactory DataContextFactory;

        protected IDataContext DataContext()
        {
            return DataContextFactory.GetContext();
        }

        protected ServiceBase(IDataContextFactory dataContextFactory)
        {
            DataContextFactory = dataContextFactory;
        }
    }
}
