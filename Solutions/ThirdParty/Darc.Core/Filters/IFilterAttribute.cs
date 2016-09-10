namespace Darc.Core.Filters
{
    public interface IFilterAttribute
    {
        void OnExecuting();
        void OnExecuted();
    }
}