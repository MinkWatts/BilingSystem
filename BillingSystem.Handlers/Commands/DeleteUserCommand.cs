using BillingSystem.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace BillingSystem.Handlers.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class DeleteUserHandler
        : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public DeleteUserHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager
                    .FindByIdAsync(request.UserId);

                if (user == null) return false;

                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting user");
                return false;
            }
        }
    }
}