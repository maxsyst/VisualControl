using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using VueExample.Errors;

namespace VueExample.EFAbstract
{
    public abstract class EFActionErrors
    {
        private readonly List<ValidationError> _validationErrorsList = new List<ValidationError>();

        public ImmutableList<ValidationError> validationErrorsList => _validationErrorsList.ToImmutableList();
        public bool HasErrors => _validationErrorsList.Any();

        protected void AddError (string message)
        {
            _validationErrorsList.Add(new ValidationError { Message = message });
        }
    }
}