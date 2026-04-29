using BillingSystem.Handlers.Commands;
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

        // ================= LOGIN =================

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager
                .PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    false);

            if (result.Succeeded)
            {
                var user = await _userManager
                    .FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var isAdmin = await _userManager
                        .IsInRoleAsync(user, "Admin");

                    if (isAdmin)
                        return RedirectToAction("Index", "Dashboard");
                    else
                        return RedirectToAction("GenerateBill", "Agent");
                }
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        // ================= REGISTER =================

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateUserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager
                .FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("",
                    "This email is already registered.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                Role = Enum.Parse<BillingSystem.Models.Enums.UserRole>(model.Role),
                CreatedBy = "System",
                CreatedDate = DateTime.Now
            };

            var result = await _userManager
                .CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // ================= LOGOUT =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // ================= ACCESS DENIED =================

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}