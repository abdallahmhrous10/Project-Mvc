
using Demo.BLL.Interfaces;
using Demo.BLL.Models;
using Demo.DAL.Models;

namespace Demo.BLL.Services
{
    public sealed class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Employee>> CreateAsync(
            Employee employee)
        {
            await _unitOfWork.Employees.CreateAsync(employee);

            await _unitOfWork.SaveChangesAsync();

            return Result<Employee>.Success(employee);
        }

        public async Task<Result<Employee>> DeleteAsync(int id)
        {
            var result = await GetByIdAsync(id);

            if (result.Succeeded != true || result.Data is null)
                return result;

            _unitOfWork.Employees.Delete(result.Data);

            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<Result<PaginatedList<Employee>>> GetAllAsync(
            string? searchValue,
            int pageIndex,
            int pageSize)
        {
            var employees = string.IsNullOrWhiteSpace(searchValue)
                ? await _unitOfWork.Employees.GetAllAsync()
                : await _unitOfWork.Employees.GetAllAsync(searchValue);

            var paginatedList = PaginatedList<Employee>.Create(
                employees,
                pageIndex,
                pageSize);

            return Result<PaginatedList<Employee>>.Success(
                paginatedList);
        }

        public async Task<Result<Employee>> GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetAsync(id);

            return employee is null
                ? Result<Employee>.Failure("Employee is not found.")
                : Result<Employee>.Success(employee);
        }

        public async Task<Result<Employee>> UpdateAsync(
            Employee employee)
        {
            _unitOfWork.Employees.Update(employee);

            await _unitOfWork.SaveChangesAsync();

            return Result<Employee>.Success(employee);
        }
    }
}
