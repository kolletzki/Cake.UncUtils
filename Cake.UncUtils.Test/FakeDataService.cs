using Cake.Core;

namespace Cake.UncUtils.Test
{
    public class FakeDataService : ICakeDataService

    {
        public TData Get<TData>() where TData : class
        {
            throw new System.NotImplementedException();
        }

        public void Add<TData>(TData value) where TData : class
        {
            throw new System.NotImplementedException();
        }
    }
}