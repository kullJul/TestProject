namespace WebApi.CQRS.Commands
{
    public interface ICommandHandler : IHandler
    {
        public Task Execute(IRequest request);
    }
}
