using BillingSystem.Data;
using BillingSystem.Handlers.Commands;
using BillingSystem.Handlers.Queries;
using BillingSystem.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BillingSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;
        private readonly AppDbContext _db;

        public AdminController(
            IMediator mediator,
            AppDbContext db)
        {
            _mediator = mediator;
            _db = db;
        }

        // ── USERS ────────────────────────────────

        public async Task<IActionResult> Users()
        {
            var users = await _mediator
                .Send(new GetUsersQuery());
            return View(users);
        }

        public IActionResult CreateUser() => View();

        [HttpPost]
        public async Task<IActionResult> CreateUser(
            CreateUserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedBy = User.FindFirstValue(
                ClaimTypes.Email) ?? "System";

            var result = await _mediator.Send(
                new CreateUserCommand
                {
                    UserDto = model
                });

            if (result)
                return RedirectToAction("Users");

            ModelState.AddModelError("",
                "Failed to create user");
            return View(model);
        }

        public async Task<IActionResult> EditUser(
            string id)
        {
            var users = await _mediator
                .Send(new GetUsersQuery());

            var user = users
                .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            var model = new EditUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(
            EditUserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.UpdatedBy = User.FindFirstValue(
                ClaimTypes.Email) ?? "System";

            var result = await _mediator.Send(
                new EditUserCommand
                {
                    UserDto = model
                });

            if (result)
                return RedirectToAction("Users");

            ModelState.AddModelError("",
                "Failed to update user");
            return View(model);
        }

        public async Task<IActionResult> DeleteUser(
            string id)
        {
            await _mediator.Send(
                new DeleteUserCommand
                {
                    UserId = id
                });
            return RedirectToAction("Users");
        }

        // ── CATEGORIES ───────────────────────────

        public async Task<IActionResult> Categories()
        {
            var categories = await _mediator
                .Send(new GetCategoriesQuery());
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(
            CreateCategoryDto model)
        {
            model.CreatedBy = User.FindFirstValue(
                ClaimTypes.Email) ?? "System";

            await _mediator.Send(
                new AddCategoryCommand
                {
                    CategoryDto = model
                });

            return RedirectToAction("Categories");
        }

        [HttpPost]
        public async Task<IActionResult> AddSubCategory(
            CreateSubCategoryDto model)
        {
            model.CreatedBy = User.FindFirstValue(
                ClaimTypes.Email) ?? "System";

            await _mediator.Send(
                new AddSubCategoryCommand
                {
                    SubCategoryDto = model
                });

            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> DeleteCategory(
            int id)
        {
            var category = await _db.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category != null)
            {
                _db.SubCategories
                    .RemoveRange(category.SubCategories);
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> DeleteSubCategory(
            int id)
        {
            var sub = await _db.SubCategories
                .FindAsync(id);

            if (sub != null)
            {
                _db.SubCategories.Remove(sub);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Categories");
        }

        // ── PRODUCTS ─────────────────────────────

        public async Task<IActionResult> Products()
        {
            var products = await _mediator
                .Send(new GetProductsQuery());
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            var categories = await _mediator
                .Send(new GetCategoriesQuery());
            ViewBag.Categories = categories;
            return View(new CreateProductDto());
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(
            CreateProductDto model)
        {
            if (model.SubCategoryId == 0)
            {
                var cats1 = await _mediator
                    .Send(new GetCategoriesQuery());
                ViewBag.Categories = cats1;
                ModelState.AddModelError("",
                    "Please select a subcategory");
                return View(model);
            }

            model.CreatedBy = User.FindFirstValue(
                ClaimTypes.Email) ?? "System";

            var result = await _mediator.Send(
                new AddProductCommand
                {
                    ProductDto = model
                });

            if (result)
                return RedirectToAction("Products");

            var cats2 = await _mediator
                .Send(new GetCategoriesQuery());
            ViewBag.Categories = cats2;
            ModelState.AddModelError("",
                "Failed to save product");
            return View(model);
        }
    }
}