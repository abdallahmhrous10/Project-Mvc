using Demo.BLL.Interfaces;
using Demo.BLL.Services;
using Demo.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class DepartmentsController : Controller
    {
        
        private readonly IDepartmentService _departmentsService;
        private const int PageSize = 10;
        public DepartmentsController(IDepartmentService departmentsService)
        {
            _departmentsService = departmentsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var result = await _departmentsService.GetallAsync(pageIndex,PageSize);

            if(result.Succeeded != true || result.Data is null)
            {
                return Problem(result.Error ?? "Unable to load departments.");
            }

            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Create (Department department)
        {
            if(!ModelState.IsValid)
            {
                return View(department);
            }

            var result = await _departmentsService.CreateAsync(department);

            if(result.Succeeded != true)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create departmen");
                return View(department);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> DepartmentControllerHandlerAsync(int ? id, string viewName)
        {
            if (!id.HasValue)
                return BadRequest();

            var result = await _departmentsService.GetByIdAsync(id.Value);

           if(result.Succeeded != true || result.Data is null)
            {
                return NotFound();
            }

           return View(viewName, result.Data);
        }

        public async Task<IActionResult> Details(int? id)
        {
            return await DepartmentControllerHandlerAsync(
                id,
                nameof(Details));
        }

        public async Task<IActionResult> Edit(int? id)
         => await DepartmentControllerHandlerAsync(
        id, nameof(Edit));

            [HttpPost]
            public async Task<IActionResult> Edit(
             [FromRoute] int id,
             Department department)
            {
                if (id != department.Id)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    var result =
                        await _departmentsService.UpdateAsync(department);

                    if (result.Succeeded != true)
                        ModelState.AddModelError(
                            string.Empty, result.Error ?? "Update failed.");
                    else
                        return RedirectToAction(nameof(Index));
                }

                return View(department);
            }

        public async Task<IActionResult> Delete(int? id)
        => await DepartmentControllerHandlerAsync(
            id, nameof(Delete));

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var result =
                await _departmentsService.DeleteAsync(id.Value);

            if (result.Succeeded != true || result.Data is null)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }


    }
}
