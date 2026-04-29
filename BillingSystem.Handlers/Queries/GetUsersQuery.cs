using BillingSystem.Models.DTOs;
using BillingSystem.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace BillingSystem.Handlers.Queries
{
    public class GetUsersQuery : IRequest<List<UserDto>> { }

    public class GetUsersHandler
        : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        private readonly UserManager<User> _userManager;

        public GetUsersHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserDto>> Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var users = _userManager.Users.ToList();

                return users.Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    Role = u.Role.ToString(),
                    CreatedDate = u.CreatedDate
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting users");
                return new List<UserDto>();
            }
        }
    }
}