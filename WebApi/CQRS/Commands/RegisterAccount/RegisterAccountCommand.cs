namespace WebApi.CQRS.Commands.RegisterAccount
{
    public class RegisterAccountCommand : IRequest
    {
        public string Login { get; set; }

        public string Password { get; set; }
    }
}
