using Hydrogen.Core.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Data
{
    public static class SeedDataExtensions
    {
        public static void EnsureSeedData(this HydrogenApplicationContext context)
        {
            if(context.AllMigrationsApplied())
            {
                if(!context.Subscriptions.Any())
                {
                    context.Subscriptions.Add(new Subscription
                    {
                        SubscriptionId = "subscription/myvideostore",
                        Name = "Cinsay Video Store",
                        DisplayName = "My Video Store"
                    });
                }

                context.SaveChanges();
            }
        }

        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }
    }
}
