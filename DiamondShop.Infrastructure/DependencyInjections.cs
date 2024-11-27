using Azure.Storage.Blobs;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Deliveries;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Application.Services.Interfaces.JewelryReviews;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.LocationRepo;
using DiamondShop.Domain.Repositories.JewelryReviewRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Interceptors;
using DiamondShop.Infrastructure.Databases.Repositories;
using DiamondShop.Infrastructure.Databases.Repositories.CustomizeRequestRepo;
using DiamondShop.Infrastructure.Databases.Repositories.DeliveryRepo;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo;
using DiamondShop.Infrastructure.Databases.Repositories.LocationRepo;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo.JewelryReviewRepo;
using DiamondShop.Infrastructure.Databases.Repositories.OrderRepo;
using DiamondShop.Infrastructure.Databases.Repositories.PromotionsRepo;
using DiamondShop.Infrastructure.Databases.Repositories.TransactionRepo;
using DiamondShop.Infrastructure.Identity.Models;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Options.Setups;
using DiamondShop.Infrastructure.Outbox;
using DiamondShop.Infrastructure.Securities;
using DiamondShop.Infrastructure.Securities.Authentication;
using DiamondShop.Infrastructure.Services;
using DiamondShop.Infrastructure.Services.ApplicationConfigurations;
using DiamondShop.Infrastructure.Services.Blobs;
using DiamondShop.Infrastructure.Services.Deliveries;
using DiamondShop.Infrastructure.Services.Excels;
using DiamondShop.Infrastructure.Services.Locations.Locally;
using DiamondShop.Infrastructure.Services.Payments.Paypals;
using DiamondShop.Infrastructure.Services.Payments.Zalopays;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Quartz;
using DiamondShop.Domain.Repositories.BlogRepo;
using DiamondShop.Infrastructure.Databases.Repositories.BlogRepo;
using DiamondShop.Application.Services.Interfaces.Blogs;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Infrastructure.Services.Pdfs;
using FluentValidation.Internal;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.Runtime.Loader;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig;
using DiamondShop.Infrastructure.Services.AdminConfigurations;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.PromotionRuleConfig;
namespace DiamondShop.Infrastructure
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMyOptionsConfiguration(configuration);
            services.AddPersistance(configuration);
            services.AddMyIdentity(configuration);
            services.AddSecurity(configuration);
            services.AddBackgroundJobs(configuration);
            services.AddPayments(configuration);
            services.AddServices(configuration);
            services.AddMappingExtension(configuration);
            //Validator name resolver
            ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression) => CamelCasePropertyNameResolver.ResolvePropertyName(type, memberInfo, expression);
            //startup
            services.AddHostedService<StartupServices>();
            return services;
        }
        public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            services.AddMemoryCache();

            services.AddDbContext<DiamondShopDbContext>(opt =>
            {
                //opt.UseNpgsql("Host=localhost;Port=5432;Database=DiamondShopTest;Username=postgres;Password=12345;Include Error Detail=true");
                opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<DomainEventsPublishserInterceptors>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountRoleRepository, AccountRoleRepository>();

            services.AddScoped<IDiamondRepository, DiamondRepository>();
            services.AddScoped<IDiamondShapeRepository, DiamondShapeRepository>();
            services.AddScoped<IDiamondCriteriaRepository, DiamondCriteriaRepository>();
            services.AddScoped<IDiamondPriceRepository, DiamondPriceRepository>();


            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IGiftRepository, GiftRepository>();
            services.AddScoped<IRequirementRepository, RequirementRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderLogRepository, OrderLogRepository>();

            services.AddScoped<ICustomizeRequestRepository, CustomizeRequestRepository>();
            services.AddScoped<IDiamondRequestRepository, DiamondRequestRepository>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();

            services.AddScoped<IJewelryRepository, JewelryRepository>();
            services.AddScoped<IJewelryReviewRepository, JewelryReviewRepository>();

            services.AddScoped<IJewelryModelRepository, JewelryModelRepository>();
            services.AddScoped<IJewelryModelCategoryRepository, JewelryModelCategoryRepository>();
            services.AddScoped<IMainDiamondRepository, MainDiamondRepository>();
            services.AddScoped<ISideDiamondRepository, SideDiamondRepository>();
            services.AddScoped<ISizeRepository, SizeRepository>();
            services.AddScoped<IMetalRepository, MetalRepository>();
            services.AddScoped<ISizeMetalRepository, SizeMetalRepository>();

            services.AddScoped<IDeliveryFeeRepository, DeliveryFeeRepository>();
            services.AddScoped<IWarrantyRepository, WarrantyRepository>();

            services.AddScoped<IPaymentService, ZalopayPaymentService>();
            services.AddScoped<ILocationRepository,LocationRepository>();

            services.AddScoped<IBlogRepository, BlogRepository>();

            services.AddSingleton<ICartService, CartService>();

            // file service persist
            services.AddSingleton((serviceProvider) =>
            {
                IOptions<ExternalUrlsOptions> getOption = serviceProvider.GetRequiredService<IOptions<ExternalUrlsOptions>>();
                if (getOption is null)
                    throw new ArgumentNullException();
                ExternalUrlsOptions option = getOption.Value;
                var newClient = new BlobServiceClient(option.Azure.ConnectionString,new BlobClientOptions() { });
                return newClient;
            });
            services.AddScoped<IBlobFileServices, AzureBlobContainerService>();
            return services;
        }
        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer()
            .AddGoogle();
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();


            return services;
        }
        public static IServiceCollection AddMyIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<CustomIdentityUser, CustomIdentityRole>()
               .AddEntityFrameworkStores<DiamondShopDbContext>()
               .AddRoleManager<CustomRoleManager>()
               .AddSignInManager<CustomSigninManager>()
               .AddUserManager<CustomUserManager>()
               .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                var userOpt = options.User;
                var passOpt = options.Password;
                var tokenOpt = options.Tokens;
                userOpt.RequireUniqueEmail = true;

                passOpt.RequireNonAlphanumeric = false;
                passOpt.RequireDigit = true;
                passOpt.RequireLowercase = false;
                passOpt.RequireUppercase = false;
                passOpt.RequiredLength = 3;
            });
            return services;
        }
        public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz();
            //quartz optoin is configured inside the ConfigureSetup<>();
            //to inject the option of the job from appsettings
            services.AddQuartzHostedService(configure =>
            {
                configure.WaitForJobsToComplete = false;
            });
            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILocationService, LocalLocationService>();
            services.AddScoped<IDiamondFileService, DiamondFileService>();
            services.AddScoped<IExcelService, ExcelSyncfunctionService>();
            services.AddScoped<IDiamondExcelService,DiamondExcelService>();
            services.AddScoped<IJewelryModelFileService, JewelryModelFileService>();
            services.AddScoped<IJewelryReviewFileService, JewelryReviewFileService>();
            services.AddScoped<IBlogFileService, BlogFileService>();
            services.AddScoped<IApplicationSettingService, ApplicationSettingService>();
            services.AddScoped<IDeliveryFeeServices, DeliveryFeeServices>();
            services.AddScoped<IOrderFileServices, OrderFileService>();
            services.AddScoped<IPdfService, GeneratePdfService>();
            //admin confi
            services.AddScoped<IDiamondRuleConfigurationService, DiamondRuleConfigurationService>();
            services.AddScoped<IPromotionRuleConfigurationService,PromotionRuleConfigurationService>();
            var serviceProviderInstrance = services.BuildServiceProvider();
            var mailOptions = serviceProviderInstrance.GetRequiredService<IOptions<MailOptions>>().Value;
            var fluentEmailBuilder = services.AddFluentEmail(mailOptions.SenderEmail);
            if (mailOptions.IsTestServer)
            {
                fluentEmailBuilder.AddSmtpSender(
                    host: mailOptions.Host,
                    port: mailOptions.Port);
            }
            else
            {
                fluentEmailBuilder.AddSmtpSender(
                    host: mailOptions.Host,
                    port: mailOptions.Port,
                    username: mailOptions.SenderEmail,
                    password: mailOptions.AppPassword
                );
            }
            fluentEmailBuilder.AddRazorRenderer();
            services.AddTransient<IEmailService, EmailService>();
            var libraryFileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "libwkhtmltox.dll" : "libwkhtmltox.so";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Library file name: " + libraryFileName);
            Console.ResetColor();
            var wkHtmlToPdfPath = Path.Combine(Directory.GetCurrentDirectory(), libraryFileName);
            var context = new CustomAssemblyLoadContext();//Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll")
            context.LoadUnmanagedLibrary(wkHtmlToPdfPath);
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            //if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false)
            //{
            //    string shellFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "installpdfdependencies.sh");
            //    InstallDependecies(shellFilePath);
            //}
            return services;
        }
        internal static IServiceCollection AddPayments(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddSingleton<PaypalClient>();
            services.AddSingleton<ZalopayClient>();
            services.AddTransient<IPaymentService, ZalopayPaymentService>();
            //services.AddScoped<IApplicationSettingService, ApplicationSettingService>();
            return services;
        }

        public static IServiceCollection AddMyOptionsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // this configure for current time, exist throughout the app life
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JwtSection));
            services.Configure<GoogleAuthenticationOption>(configuration.GetSection(GoogleAuthenticationOption.Section));
            services.Configure<ExternalAuthenticationOptions>(configuration.GetSection(ExternalAuthenticationOptions.Section));
            services.Configure<AuthenticationRestrictionOption>(configuration.GetSection(AuthenticationRestrictionOption.Section));
            services.Configure<ExternalUrlsOptions>(configuration.GetSection(ExternalUrlsOptions.Section));
            services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.Section));
            services.Configure<VnpayOption>(configuration.GetSection(VnpayOption.Section));
            services.Configure<PaypalOption>(configuration.GetSection(PaypalOption.Section));
            services.Configure<UrlOptions>(configuration.GetSection(UrlOptions.Section));
            services.Configure<LocationOptions>(config => { });
            services.Configure<MailOptions>(configuration.GetSection(MailOptions.Section));
            services.Configure<FrontendOptions>(configuration.GetSection(FrontendOptions.Section));
            services.Configure<PublicBlobOptions>(configuration.GetSection(PublicBlobOptions.Section));
            // this also exist throughout the app life, but it is configured at the end of dependency injection,
            // allow it to inject other or override settings , also more cleaner moduler code
            services.ConfigureOptions<JwtBearerOptionSetup>();
            services.ConfigureOptions<GoogleOptionSetup>();
            services.ConfigureOptions<QuartzOptionSetup>();

            //configure changeable settings
            services.Configure<ApplicationSettingGlobal>(config => { });
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF1cX2hIfEx0QXxbf1x0ZFNMYV9bQHFPMyBoS35RckRiW3tednddRGFbVUJ/");
            return services;
        }
        // test if origin change work
        public static IServiceCollection AddMappingExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.RemoveAll<TypeAdapterConfig>();
            var config = TypeAdapterConfig.GlobalSettings;
            //services.AddSingleton(config);
            //services.AddScoped<IMapper, ServiceMapper>();
            var getOption = services.BuildServiceProvider().GetRequiredService<IOptions<ExternalUrlsOptions>>().Value;
            config.NewConfig<Media, MediaDto>()
               .Map(dest => dest.MediaPath, src => $"{getOption.Azure.BaseUrl}/{getOption.Azure.ContainerName}/{src.MediaPath}");
            config.NewConfig<GalleryTemplate, GalleryTemplateDto>()
               .Map(dest => dest.Gallery, src => src.Gallery.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            //config.NewConfig<GalleryTemplate, GalleryTemplateDto>()
            //   .Map(dest => dest.Gallery , src => src.Gallery.ToDictionary(kvp => kvp.Key, kvp => $"{getOption.Azure.BaseUrl}/{getOption.Azure.ContainerName}/{kvp.Value}"));
            services.AddSingleton(config);
            return services;
        }
        private static void InstallDependecies(string shellFilePath)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c " + shellFilePath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
    public class CamelCasePropertyNameResolver
    {

        public static string ResolvePropertyName(Type type, MemberInfo memberInfo, LambdaExpression expression)
        {
            return ToCamelCase(DefaultPropertyNameResolver(type, memberInfo, expression));
        }

        private static string DefaultPropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression)
        {
            if (expression != null)
            {
                var chain = PropertyChain.FromExpression(expression);
                if (chain.Count > 0) return chain.ToString();
            }

            if (memberInfo != null)
            {
                return memberInfo.Name;
            }

            return null;
        }

        private static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            var chars = s.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                var hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    break;
                }

                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }

            return new string(chars);
        }
    }
    internal class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
       
    }
   
}
