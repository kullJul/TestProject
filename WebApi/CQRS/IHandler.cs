namespace WebApi.CQRS
{
    public interface IHandler
    {
        public T RequestType { get; set }
    }
}
