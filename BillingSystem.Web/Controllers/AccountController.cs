using BillingSystem.Models.DTOs;
using BillingSystem.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IMediator mediator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mediator = mediator;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(
            LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email;
            var password = model.Password;
            var remember = model.RememberMe;

            var result = await _signInManager
                .PasswordSignInAsync(
                    email,
                    password,
                    remember,
                    lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager
                    .FindByEmailAsync(email);

                if (user != null)
                {
                    var isAdmin = await _userManager
                        .IsInRoleAsync(user, "Admin");

                    if (isAdmin)
                        return RedirectToAction(
                            "Index", "Dashboard");

                    return RedirectToAction(
                        "GenerateBill", "Agent");
                }
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("",
                    "Account is locked out.");
            }
            else
            {
                ModelState.AddModelError("",
                    "Invalid email or password.");
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(
            CreateUserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email;
            var fullName = model.FullName;
            var password = model.Password;
            var role = model.Role;
            var now = DateTime.Now;

            var existingUser = await _userManager
                .FindByEmailAsync(email);

            if (existingUser != null)
            {
                ModelState.AddModelError("",
                    "Email already registered.");
                return View(model);
            }

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                UserName = email,
                Role = Enum.Parse<BillingSystem
                    .Models.Enums.UserRole>(role),
                CreatedBy = "System",
                CreatedDate = now,
                EmailConfirmed = true
            };

            var result = await _userManager
                .CreateAsync(newUser, password);

            if (result.Succeeded)
            {
                await _userManager
                    .AddToRoleAsync(newUser, role);

                TempData["Success"] =
                    "Account created! Please login.";

                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("",
                    error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}