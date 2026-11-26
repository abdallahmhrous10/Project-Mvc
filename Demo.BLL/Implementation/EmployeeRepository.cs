using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.BLL.Implementation
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {

        public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }
        public async Task<IEnumerable<Employee>> GetAllAsync(string name)
        {
            return await _dbset.Include(e => e.Department).Where(e => e.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        }

       public async Task<IEnumerable<Employee>> GetAllWithDepartmentAsync()
        {
            return await _dbset.Include(e => e.Department).ToListAsync();
        }
    }
}
