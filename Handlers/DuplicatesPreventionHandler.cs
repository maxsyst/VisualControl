using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;

namespace VueExample.Handlers
{
    public class DuplicatesPreventionHandler<T> where T : class 
    {
        private readonly VisualControlContext _visualControlContext;
        public DuplicatesPreventionHandler(VisualControlContext visualControlContext)
        {
            _visualControlContext = visualControlContext;
        }
        public T AddedObject  { get; private set; }
        public string Error { get; private set; }

        public void Add(T addingObject, DbContext dbContext)
        {
            _visualControlContext.Set<T>().Add(addingObject);
            _visualControlContext.SaveChangesAsync();
        }
    }
}
