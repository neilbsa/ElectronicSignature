using ElectronicSignature.Core.Repository.Core.Interfaces;
using ElectronicSignature.Data;
using ElectronicSignature.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ElectronicSignature.Core.Repository.Core.Implementations
{
    public abstract class Repository<T> : IRepository<T> where T : class, IBaseEntity
    {


        protected DbContext _entities { get; set; }

        public Repository() : this(new ApplicationDbContext())
        {

        }

        public Repository(DbContext context)
        {
            _entities = context;
        }

        public virtual async Task<T> AddEntityAsync(T entity)
        {
            var data = await _entities.Set<T>().AddAsync(entity);


            await SaveAsync();
            return data.Entity;
        }

        public virtual async Task<bool> DeleteEntity(Guid Id)
        {
            var result = false;
            var item = await this.GetDetailsAsync(Id);
            if (item != null)
            {
                item.IsDeleted = true;
                result = true;
                await this.UpdateEntityAsync(item, item);
            }

            return result;
        }

        public async Task<bool> PermanentDeleteEntity(Guid Id)
        {
            var result = false;
            var item = await this.GetDetailsAsync(Id);
            if (item != null)
            {
                _entities.Set<T>().Remove(item);
                await SaveAsync();
                result = true;
            }

            return result;
        }



        public virtual async Task<ICollection<T>> GetAllAsync()
        {
            var data = await _entities.Set<T>().ToListAsync();
            return data;
        }

        public virtual async Task<T> GetDetailsAsync(Guid Id)
        {
            var item = await _entities.Set<T>().FindAsync(Id);
            //_entities.Entry(item).State = EntityState.Detached;
            return item;
        }

        public async Task<T> GetDetailsAsync(Guid Id, bool DetachItem)
        {
            if (!DetachItem)
            {
                return await GetDetailsAsync(Id);
            }
            else
            {
                var item = await GetDetailsAsync(Id);
                if (item != null)
                {
                    _entities.Entry(item).State = EntityState.Detached;
                }

                return item;
            }
        }

        public T GetDetails(Expression<Func<T, bool>> filter)
        {
            var data = _entities.Set<T>().Where(filter).FirstOrDefault();
            return data;
        }

        public virtual ICollection<T> GetList(Expression<Func<T, bool>> filter)
        {
            var data = _entities.Set<T>().Where(filter).ToList();
            return data;
        }

        public bool IsExist(Func<T, bool> filter)
        {
            var data = _entities.Set<T>().Any(filter);
            return data;
        }

        public virtual async Task SaveAsync()
        {

            try
            {
                await _entities.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                var d = e.ToString();
                var dd = d;
            }



            //  _entities.SaveChanges();
        }

        public virtual async Task UpdateEntityAsync(T entity, T newEntity)
        {
            //T ent;
            //ent = entity;
            _entities.Entry(entity).CurrentValues.SetValues(newEntity);
            var data = _entities.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
        }

        public virtual async Task UpdateEntityAsync(T newEntity)
        {

            var oldEntity = await GetDetailsAsync(newEntity.Id);


            await UpdateEntityAsync(oldEntity, newEntity);
        }

        public virtual async Task UpdateTransaction(T ent)
        {
            var oldEntity = await GetDetailsAsync(ent.Id);
            await UpdateEntityAsync(oldEntity, ent);
        }



    }
}
