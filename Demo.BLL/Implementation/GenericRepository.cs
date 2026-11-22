using Demo.BLL.Interfaces;
using Demo.DAL.Data;
using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.BLL.Implementation
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {

        private readonly ApplicationDbContext _context;
        protected DbSet<TEntity> _dbset;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbset = _context.Set<TEntity>();   
        }

        public async Task CreateAsync(TEntity entity)
        => await _dbset.AddAsync(entity);

        public void Delete(TEntity entity)
            => _dbset.Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()
         => await _dbset.ToListAsync();

        public async Task<TEntity?> GetAsync(int id)
            => await _dbset.FindAsync(id);

        public void Update(TEntity entity)
            => _dbset.Update(entity);
    }
}
