
using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Models;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class EmployeesController : Controller
    {
        private const int PageSize = 10;

        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeesController(
            IEmployeeService employeeService,
            IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
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
    }
}
