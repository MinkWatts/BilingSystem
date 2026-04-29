using BillingSystem.Models.DTOs;
using BillingSystem.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace BillingSystem.Handlers.Commands
{
    public class CreateUserCommand : IRequest<bool>
    {
        public CreateUserDto UserDto { get; set; } = null!;
    }

    public class CreateUserHandler
        : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public CreateUserHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = new User
                {
                    FullName = request.UserDto.FullName,
                    Email = request.UserDto.Email,
                    UserName = request.UserDto.Email,
                    Role = Enum.Parse<Models.Enums.UserRole>
                        (request.UserDto.Role),
                    CreatedBy = request.UserDto.CreatedBy,
                    CreatedDate = DateTime.Now
                };

                var result = await _userManager
                    .CreateAsync(user, request.UserDto.Password);

                if (result.Succeeded)
                {
                    await _userManager
                        .AddToRoleAsync(user, request.UserDto.Role);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating user");
                return false;
            }
        }
    }
}