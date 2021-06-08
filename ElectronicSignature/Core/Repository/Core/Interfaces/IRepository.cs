using ElectronicSignature.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ElectronicSignature.Core.Repository.Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> AddEntityAsync(TEntity entity);
        Task UpdateTransaction(TEntity ent);
        ICollection<TEntity> GetList(Expression<Func<TEntity, bool>> filter);
    
        Task<TEntity> GetDetailsAsync(Guid Id);

        TEntity GetDetails(Expression<Func<TEntity, bool>> filter);
        bool IsExist(Func<TEntity, bool> filter);
        Task<ICollection<TEntity>> GetAllAsync();
        Task<bool> PermanentDeleteEntity(Guid Id);
        Task<bool> DeleteEntity(Guid Id);
        Task UpdateEntityAsync(TEntity newEntity);
        Task SaveAsync();

    }
}
