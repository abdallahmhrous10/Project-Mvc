
using Demo.BLL.Models;
using Demo.DAL.Models;

namespace Demo.BLL.Interfaces
{
    public interface IEmployeeService
    {
        Task<Result<PaginatedList<Employee>>> GetAllAsync(
            string? searchValue,
            int pageIndex,
            int pageSize);

        Task<Result<Employee>> GetByIdAsync(int id);

        Task<Result<Employee>> CreateAsync(Employee employee);

        Task<Result<Employee>> UpdateAsync(Employee employee);

        Task<Result<Employee>> DeleteAsync(int id);
    }
}
