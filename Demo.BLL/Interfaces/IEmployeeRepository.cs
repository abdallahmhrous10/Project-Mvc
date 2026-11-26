using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.BLL.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetAllAsync(string name);

        Task<IEnumerable<Employee>> GetAllWithDepartmentAsync();
    }
}
