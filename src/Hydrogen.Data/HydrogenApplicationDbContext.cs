using System;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Hydrogen.Core.Domain.Multitenancy;
using Hydrogen.Infrastructure.Multitenancy;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Hydrogen.Core.Domain.Subscriptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Hydrogen.Domain.Payments;
using Hydrogen.Core.Domain.Consultants;

namespace Hydrogen.Data
{
    public static class DbContextExtensions
    {
        public static void LogToConsole(this DbContext context)
        {
            var contextServices = ((IInfrastructure<IServiceProvider>)context).Instance;
            var loggerFactory = contextServices.GetService<ILoggerFactory>();
            loggerFactory.AddConsole(LogLevel.Debug);
        }
    }

    public class EfTools : IDbContextFactory<HydrogenApplicationContext>
    {
        public static void Main(params string[] args) { }

        public HydrogenApplicationContext Create()
        {
            return new HydrogenApplicationContext();
        }

        public HydrogenApplicationContext Create(DbContextFactoryOptions options)
        {
            return new HydrogenApplicationContext();
        }
    }
    public class HydrogenApplicationContext : IdentityDbContext<IdentityUser>
    {
        private readonly ApplicationTenant _tenant;
        private readonly Serilog.ILogger _log;

        internal HydrogenApplicationContext()
        {
            _tenant = new ApplicationTenant
            {
                Database = new DatabaseSettings
                {
                    ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=Hydrogen.Local;"
                }
            };
        }
        
        
        public HydrogenApplicationContext(
            IOptions<MultitenancyOptions> options, Serilog.ILogger log, ApplicationTenant tenant = null)
            :this(tenant 
                 ?? options.Value.Tenants.SingleOrDefault(x => x.Name == options.Value.DefaultTenant),
                 options.Value.MigrateDatabases, log)
        {
            
        }

        public HydrogenApplicationContext(ApplicationTenant tenant, bool migrate, Serilog.ILogger log)
        {
            _tenant = tenant;
            _log = log;

            if (_tenant == null)
            {
                throw new Exception("Unable to identify database connection for tenant.");
            }
            this.LogToConsole();
            log.Verbose("Using {tenant} database.", tenant.Name);
            InitializeDatabase(migrate);
        }

        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<ConsultantSubscription> UserSubscriptions { get; set; }
        private void InitializeDatabase(bool migrate)
        {
            if (migrate)
            {
                Database.Migrate();
            }
            else
            {
                try
                {
                    //Database.EnsureDeleted();
                    Database.EnsureCreated();
                }
                catch
                {
                    _log.Warning("Unable to delete database.");
                }
                
            }

            this.EnsureSeedData();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_tenant.Database.ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Consultant>(entity =>
            {
                entity.ToTable("Consultants");

                entity
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<Consultant>(e => e.UserId);

                entity
                .HasMany(s => s.Subscriptions)
                .WithOne(s => s.Consultant)
                .HasForeignKey(s => s.UserId);

                entity.HasMany(e => e.PaymentMethods)
                .WithOne()
                .HasForeignKey(e => e.UserId);

                entity.HasKey(c => c.UserId);

                entity.Ignore(x => x.User);
            });
            
            builder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethods");
                entity.HasOne(p => p.Consultant)
                .WithMany(c => c.PaymentMethods)
                .HasForeignKey(p => p.UserId);

                entity.HasKey(e => e.Id);
                entity.HasAlternateKey(e => new { e.UserId, e.ReferenceId });
            });
            
            builder.Entity<Subscription>(entity =>
            {
                entity.ToTable("Subscriptions");

                entity.HasKey(e => e.SubscriptionId);           

                entity
                .HasMany(s => s.Prices)
                .WithOne(s => s.Subscription)
                .HasForeignKey(s => s.SubscriptionId);
            });

            builder.Entity<SubscriptionPrice>(entity =>
            {
                entity.ToTable("SubscriptionPrices");
                entity.HasKey(x => new { x.SubscriptionId, x.Currency });
            });
            
            builder.Entity<ConsultantSubscription>(entity =>
            {
                entity.ToTable("ConsultantSubscriptions");
                entity.HasKey(x => new { x.UserId, x.SubscriptionId });

                entity
                .HasOne(x => x.Consultant)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(u => u.UserId);

                entity
                .HasOne(s => s.Subscription)
                .WithMany(s => s.Consultants)
                .HasForeignKey(s => s.SubscriptionId);
            });
        }        
    }
}
