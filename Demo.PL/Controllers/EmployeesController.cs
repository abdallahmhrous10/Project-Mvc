using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Models;
using Demo.DAL.Models;
using Demo.PL.Utilities;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.PL.Controllers
{
    public class EmployeesController : Controller
    {
        private const int PageSize = 10;
        private const int MaxImageSize = 5 * 1024 * 1024;

        private static readonly string[] AllowedImageExtensions =
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public EmployeesController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchValue,
            int pageIndex = 1)
        {
            var result = await _employeeService.GetAllAsync(
                searchValue,
                pageIndex,
                PageSize);

            if (result.Succeeded != true || result.Data is null)
                return Problem(result.Error ?? "Unable to load employees.");

            var employees = _mapper.Map<IEnumerable<EmployeeViewModel>>(
                result.Data);

            var model = PaginatedList<EmployeeViewModel>.Create(
                employees,
                result.Data.TotalCount,
                result.Data.PageIndex,
                PageSize);

            return View(model);
        }

        private async Task LoadDepartmentsAsync()
        {
            // Keep the method name used by the current project.
            var result = await _departmentService.GetallAsync(1, int.MaxValue);

            var departments = result.Data
                ?? PaginatedList<Department>.Create(
                    Enumerable.Empty<Department>(), 1, 1);

            ViewBag.Departments = new SelectList(
                departments, "Id", "Name");
        }

        private static bool IsValidImage(IFormFile? image)
        {
            if (image is null || image.Length == 0)
                return true;

            var extension = Path.GetExtension(image.FileName)
                .ToLowerInvariant();

            return AllowedImageExtensions.Contains(extension)
                && image.Length <= MaxImageSize;
        }

        private async Task<string?> UploadImageIfProvidedAsync(
            IFormFile? image)
        {
            if (image is null || image.Length == 0)
                return null;

            return await DocumentSettings.UploadFileAsync(
                image,
                "Images",
                _environment.WebRootPath);
        }

        private void DeleteImageIfPresent(string? imageName)
        {
            if (!string.IsNullOrWhiteSpace(imageName))
            {
                DocumentSettings.DeleteFile(
                    imageName,
                    "Images",
                    _environment.WebRootPath);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid || !IsValidImage(employeeVM.Image))
            {
                if (!IsValidImage(employeeVM.Image))
                {
                    ModelState.AddModelError(
                        nameof(employeeVM.Image),
                        "Choose a JPG, JPEG, PNG, or WEBP image no larger than 5 MB.");
                }

                await LoadDepartmentsAsync();
                return View(employeeVM);
            }

            string? uploadedImageName = null;
            try
            {
                uploadedImageName = await UploadImageIfProvidedAsync(
                    employeeVM.Image);

                if (uploadedImageName is not null)
                    employeeVM.ImageName = uploadedImageName;

                var employee = _mapper.Map<Employee>(employeeVM);
                var result = await _employeeService.CreateAsync(employee);

                if (result.Succeeded != true)
                {
                    DeleteImageIfPresent(uploadedImageName);

                    ModelState.AddModelError(
                        string.Empty,
                        result.Error ?? "Unable to create employee.");

                    await LoadDepartmentsAsync();
                    return View(employeeVM);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // Avoid leaving an uploaded file behind if saving fails.
                DeleteImageIfPresent(uploadedImageName);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
            => await EmployeeControllerHandlerAsync(id, nameof(Details));

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
            => await EmployeeControllerHandlerAsync(id, nameof(Edit));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [FromRoute] int id,
            EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.Id)
                return BadRequest();

            var existingResult = await _employeeService.GetByIdAsync(id);
            if (existingResult.Succeeded != true || existingResult.Data is null)
                return NotFound();

            var existingEmployee = existingResult.Data;

            if (!ModelState.IsValid || !IsValidImage(employeeVM.Image))
            {
                if (!IsValidImage(employeeVM.Image))
                {
                    ModelState.AddModelError(
                        nameof(employeeVM.Image),
                        "Choose a JPG, JPEG, PNG, or WEBP image no larger than 5 MB.");
                }

                await LoadDepartmentsAsync();
                return View(employeeVM);
            }

            var oldImageName = existingEmployee.ImageName;
            string? newImageName = null;

            try
            {
                newImageName = await UploadImageIfProvidedAsync(
                    employeeVM.Image);

                // Keep the old image when the user does not upload a replacement.
                employeeVM.ImageName = newImageName ?? oldImageName;

                // Map into the tracked entity so fields not present on the ViewModel
                // (such as audit fields) are not needlessly reset.
                _mapper.Map(employeeVM, existingEmployee);

                var result = await _employeeService.UpdateAsync(existingEmployee);

                if (result.Succeeded != true)
                {
                    DeleteImageIfPresent(newImageName);
                    ModelState.AddModelError(
                        string.Empty,
                        result.Error ?? "Unable to update employee.");

                    await LoadDepartmentsAsync();
                    return View(employeeVM);
                }

                // Remove the old image only after the database update succeeds.
                if (newImageName is not null &&
                    !string.IsNullOrWhiteSpace(oldImageName) &&
                    !string.Equals(oldImageName, newImageName, StringComparison.OrdinalIgnoreCase))
                {
                    DeleteImageIfPresent(oldImageName);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // If the operation failed before the update completed, remove the new upload.
                DeleteImageIfPresent(newImageName);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
            => await EmployeeControllerHandlerAsync(id, nameof(Delete));

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var result = await _employeeService.DeleteAsync(id.Value);

            if (result.Succeeded != true || result.Data is null)
                return NotFound();

            // Delete the stored image only after the employee row was deleted.
            DeleteImageIfPresent(result.Data.ImageName);

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> EmployeeControllerHandlerAsync(
            int? id,
            string viewName)
        {
            if (!id.HasValue)
                return BadRequest();

            if (viewName == nameof(Edit))
                await LoadDepartmentsAsync();

            var result = await _employeeService.GetByIdAsync(id.Value);

            if (result.Succeeded != true || result.Data is null)
                return NotFound();

            var employeeVM = _mapper.Map<EmployeeViewModel>(result.Data);
            return View(viewName, employeeVM);
        }
    }
}
