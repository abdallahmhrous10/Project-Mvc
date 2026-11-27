
using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Models;
using Demo.DAL.Models;
using Demo.PL.Utilities;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class EmployeesController : Controller
    {
        private const int PageSize = 10;
        private const int MaxImageSize = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
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

        // GET: Employees
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchValue,
            int pageIndex = 1)
        {
            var result = await _employeeService.GetAllAsync(
                searchValue,
                pageIndex,
                PageSize);

            if ( result.Succeeded != true || result.Data is null)
                return Problem(result.Error);

            var employees =
                _mapper.Map<IEnumerable<EmployeeViewModel>>(
                    result.Data);

            var model = PaginatedList<EmployeeViewModel>.Create(
                employees,
                result.Data.TotalCount,
                result.Data.PageIndex,
                PageSize);

            return View(model);
        }

        // Load Departments
        private async Task LoadDepartmentsAsync()
        {
            var result = await _departmentService.GetallAsync(
                1, int.MaxValue);

            var departments = result.Data
                ?? PaginatedList<Department>.Create(
                    Enumerable.Empty<Department>(), 1, 1);

            ViewBag.Departments = new SelectList(
                departments,
                "Id",
                "Name");
        }

        // GET: Employees/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsAsync();

            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync();
                return View(employeeVM);
            }

            // Validate and upload image
            if (employeeVM.Image is not null &&
                employeeVM.Image.Length > 0)
            {
                string extension = Path.GetExtension(
                    employeeVM.Image.FileName).ToLowerInvariant();

                if (!AllowedExtensions.Contains(extension) ||
                    employeeVM.Image.Length > MaxImageSize)
                {
                    ModelState.AddModelError(
                        nameof(employeeVM.Image),
                        "Please upload a valid image up to 5 MB.");

                    await LoadDepartmentsAsync();

                    return View(employeeVM);
                }

                employeeVM.ImageName =
                    await DocumentSettings.UploadFileAsync(
                        employeeVM.Image,
                        "Images",
                        _environment.WebRootPath);
            }

            // Map ViewModel to Employee
            var employee = _mapper.Map<Employee>(employeeVM);

            // Save employee
            var result = await _employeeService.CreateAsync(employee);

            if (result.Succeeded != true)
            {
                // Delete uploaded image if database save fails
                if (!string.IsNullOrWhiteSpace(employeeVM.ImageName))
                {
                    DocumentSettings.DeleteFile(
                        employeeVM.ImageName,
                        "Images",
                        _environment.WebRootPath);
                }

                ModelState.AddModelError(
                    string.Empty,
                    result.Error ?? "Unable to create employee.");

                await LoadDepartmentsAsync();

                return View(employeeVM);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Details/id
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            return await EmployeeControllerHandlerAsync(
                id,
                nameof(Details));
        }

        // GET: Employees/Edit/id
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            return await EmployeeControllerHandlerAsync(
                id,
                nameof(Edit));
        }

        // GET: Employees/Delete/id
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await EmployeeControllerHandlerAsync(
                id,
                nameof(Delete));
        }

        // Shared Handler
        private async Task<IActionResult>
            EmployeeControllerHandlerAsync(
                int? id,
                string viewName)
        {
            if (!id.HasValue)
                return BadRequest();

            if (viewName == nameof(Edit))
                await LoadDepartmentsAsync();

            var result =
                await _employeeService.GetByIdAsync(id.Value);

            if (result.Succeeded != true || result.Data is null)
                return NotFound();

            var employeeVM =
                _mapper.Map<EmployeeViewModel>(result.Data);

            return View(viewName, employeeVM);
        }
    }
}
