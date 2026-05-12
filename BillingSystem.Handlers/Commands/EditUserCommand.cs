using BillingSystem.Models.DTOs;
using BillingSystem.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace BillingSystem.Handlers.Commands
{
    public class EditUserCommand : IRequest<bool>
    {
        public EditUserDto UserDto { get; set; } = null!;
    }

    public class EditUserHandler
        : IRequestHandler<EditUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public EditUserHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(
            EditUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager
                    .FindByIdAsync(request.UserDto.Id);

                if (user == null) return false;

                user.FullName = request.UserDto.FullName;
                user.Email = request.UserDto.Email;
                user.UserName = request.UserDto.Email;
                user.Role = Enum.Parse<Models.Enums.UserRole>
                    (request.UserDto.Role);
                user.UpdatedBy = request.UserDto.UpdatedBy;
                user.UpdatedDate = DateTime.Now;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error editing user");
                return false;
            }
        }
    }
}