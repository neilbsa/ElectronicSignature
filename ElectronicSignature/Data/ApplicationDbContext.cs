using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElectronicSignature.Models;
using ElectronicSignature.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectronicSignature.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {


        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentElementCoordinates> ElementCoordianates { get; set; }
        public DbSet<Signatories> Signatories { get; set; }
        public DbSet<FileRepository> FileRepository { get; set; }

        public DbSet<FolderStructureModel> FolderStructure { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext() :base (new DbContextOptions<ApplicationDbContext>())
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor access)
            : base(options)
        {
            _httpContextAccessor = access;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }




        private void AddDateStamp()
        {
            var ent = ChangeTracker.Entries();

            string user = _httpContextAccessor?.HttpContext?.User.FindFirst(x=>x.Type == ClaimTypes.Email)?.Value;
            foreach (var item in ent)
            {
                if (item.Entity is IChangeTracker track)
                {

                    ((IChangeTracker)item.Entity).LastModifiedDate = DateTime.Now;
                    ((IChangeTracker)item.Entity).LastModifiedUser = user == null ? "Administrator" : user;
                    Entry(track).Property(x => x.CreateDate).IsModified = false;
                    Entry(track).Property(x => x.CreateUser).IsModified = false;



                    if (item.State == EntityState.Added)
                    {


                        ((IChangeTracker)item.Entity).CreateDate = DateTime.Now;
                        ((IChangeTracker)item.Entity).CreateUser = user == null ? "Administrator" : user;
                    }
                }


            }
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddDateStamp();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }


        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddDateStamp();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddDateStamp();
            return base.SaveChangesAsync(cancellationToken);
        }


    }
}
