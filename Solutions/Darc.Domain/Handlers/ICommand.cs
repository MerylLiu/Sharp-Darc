namespace Darc.Domain
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public interface ICommand
    {
        bool IsValid();

        ICollection<ValidationResult> ValidationResults();
    }
}