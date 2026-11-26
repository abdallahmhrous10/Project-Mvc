using Demo.BLL.Interfaces;
using Demo.DAL.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.BLL.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        private readonly Lazy<IDepartmentRepository>  _departmentRepository;
        private readonly Lazy<IEmployeeRepository>   _employeeRepository;
        public IDepartmentRepository Departments => _departmentRepository.Value;

        public IEmployeeRepository Employees =>  _employeeRepository.Value;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _departmentRepository = new Lazy<IDepartmentRepository>(() => new DepartmentRepository(_context));

            _employeeRepository =  new Lazy<IEmployeeRepository>( () => new EmployeeRepository(_context));
        }

        public async Task<int> SaveChangesAsync()   =>  await _context.SaveChangesAsync();
        
    }
}
