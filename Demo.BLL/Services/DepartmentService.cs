using Demo.BLL.Implementation;
using Demo.BLL.Interfaces;
using Demo.BLL.Models;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.BLL.Services
{
    public class DepartmentService : IDepartmentService
    {

        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaginatedList<Department>>> GetallAsync(int pageIndex, int pageSize)
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();

            var paginatedDepartments = PaginatedList<Department>.Create(departments, pageIndex, pageSize);

            return Result<PaginatedList<Department>>.Success(paginatedDepartments);
        }

        public async Task<Result<Department>> GetByIdAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetAsync(id);

            if (department is null)
            {
                return Result<Department>.Failure("Department Not Found");
            }

            return Result<Department>.Success(department);
        }

        public async Task<Result<Department>> CreateAsync(Department department)
        {
            await _unitOfWork.Departments.CreateAsync(department);
            
            await _unitOfWork.SaveChangesAsync();
            return Result<Department>.Success(department);
        }
        public async Task<Result<Department>> UpdateAsync(Department department)
        {
            var existingDepartment = await _unitOfWork.Departments.GetAsync(department.Id);

            if (existingDepartment is null)
            {
                return Result<Department>.Failure("Department Not Found");
            }

             _unitOfWork.Departments.Update(department);

            await _unitOfWork.SaveChangesAsync();
            return Result<Department>.Success(department);
        }

        public async Task<Result<Department>> DeleteAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetAsync(id);

            if (department is null)
            {
                return Result<Department>.Failure("Department Not Found");
            }

            _unitOfWork.Departments.Delete(department);

            await _unitOfWork.SaveChangesAsync();
            return Result<Department>.Success(department);
        }

   
    }
}
