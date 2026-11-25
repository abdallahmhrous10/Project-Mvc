using Demo.BLL.Models;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.BLL.Interfaces
{
    public interface IDepartmentService
    {
        Task<Result<PaginatedList<Department>>> GetallAsync(int pageIndex,int pageSize);

        Task<Result<Department>> GetByIdAsync(int id);

        Task<Result<Department>> CreateAsync(Department department);

       Task<Result<Department>> UpdateAsync(Department department);

        Task<Result<Department>> DeleteAsync(int id);

    }
}
