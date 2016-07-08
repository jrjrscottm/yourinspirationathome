using Braintree;
using Hydrogen.Infrastructure.Authorization;
using Hydrogen.Integration.Cinsay;
using Hydrogen.Services;
using Hydrogen.Services.Actors;
using Hydrogen.Services.Events;
using Hydrogen.Services.Payments;
using Hydrogen.Services.Subscriptions;
using Hydrogen.Services.Users;
using Hydrogen.Services.VideoStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen
{
    public static class HydrogenExtensions
    {
        public static IServiceCollection AddHydrogenServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPaymentService, BraintreePaymentService>();
            services.AddTransient<IPaymentCommandHandler, PaymentCommandHandler>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();
            services.AddSingleton<ActorService>();
            services.AddTransient<IVideoStoreService, VideoStoreService>();
            services.AddTransient<ICinsayClient, CinsayClient>(sp =>
            new CinsayClient(

                //TODO: Configure Cinsay Options
                "http://services.bizdemo.cinsay.com/cinsay-api/",
                "3f01646e-1d22-11e6-ba75-42ce01a17d1d",
                "3eee27f5-1d22-11e6-ba75-42ce01a17d1d"
                ));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MyVideoStore", policy =>
                policy.Requirements.Add(new SubscriptionRequirement("subscription/myvideostore")));
            });

            services.AddTransient<IConsultantAppService, ConsultantApplicationService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEventBus, EventBus>();
            services.AddSingleton<IAuthorizationHandler, SubscriptionAuthorizationHandler>();


            services.AddSingleton<IBraintreeGateway>(sp => new BraintreeGateway(
                new Configuration
                {
                    MerchantId = configuration["Braintree:MerchantId"],
                    PublicKey = configuration["Braintree:PublicKey"],
                    PrivateKey = configuration["Braintree:PrivateKey"],
                    Environment = Environment.ParseEnvironment(
                            configuration["Braintree:Environment"]
                        )
                }
            ));

            services.AddTransient<IPaymentService, BraintreePaymentService>();

            return services;
        }
    }
}
