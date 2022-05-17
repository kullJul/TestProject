namespace WebApi.CQRS.Commands.RegisterAccount
{
    public class RegisterAccountCommandHandler : ICommandHandler
    {
        public Task Execute(IRequest request)
        {
            var command = request as RegisterAccountCommand;

            if (command.Login.Equals("Admin", StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.FromException(new InvalidOperationException());
            }

            return Task.CompletedTask;
        }
    }
}
