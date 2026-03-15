using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Services.Business.Company;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.Manager;
using SalesAndDistributionSystem.Services.Business.Security;
using SalesAndDistributionSystem.Services.Business.User;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.IO;
using DinkToPdf.Contracts;
using DinkToPdf;
using SalesAndDistributionSystem.Services.ReportSpecifier;
using SalesAndDistributionSystem.Services.Business.Inventory.Manager;
using SalesAndDistributionSystem.Services.Business.Inventory.Manager.IManager;
using SalesAndDistributionSystem.Services.Business.Inventory.IManager;
using SalesAndDistributionSystem.Hubs;
using SalesAndDistributionSystem.Services.Business.Target.InMarketSales;
using SalesAndDistributionSystem.Services.Business.Target.InMarketSales.Interface;
using SalesAndDistributionSystem.Services.Business.Target.IManager;
using SalesAndDistributionSystem.Services.Business.Target.Manager;
using Org.BouncyCastle.Asn1.X509;

namespace SalesAndDistributionSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //FastReport.Utils.RegisteredObjects.AddConnection(typeof(OdbcDataConnection));
            services.AddAutoMapper(typeof(Startup));
            services.AddControllersWithViews();
            services.AddSession();
            services.AddMvc();
            //services.AddGlimpse();
            services.AddSignalR();


            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IReportSpecifier, ReportSpecifier>();



            //-------Security Module -------------------------------
            #region Security Module
            services.AddTransient<ICommonServices, CommonServices>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IMenuCategoryManager, MenuCategoryManager>();
            services.AddTransient<IMenuMasterManager, MenuMasterManager>();
            services.AddTransient<IMenuPermissionManager, MenuPermissionManager>();
            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<IRoleManager, RoleManager>();
            services.AddTransient<IUserMenuConfigManager, UserMenuConfigManager>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IReportConfigurationManager, ReportConfigurationManager>();
            services.AddTransient<IUserLogManager, UserLogManager>();
            services.AddTransient<INotificationManager, NotificationManager>();
            services.AddTransient<IEmployeeManager, EmployeeManager>();
            #endregion
            //--------------Sales And Distribution Module ----------------------


            //--Sales And Distribution Module - location services
            #region Location
            services.AddTransient<IDivisionManager, DivisionManager>();
            services.AddTransient<IRegionManager, RegionManager>();
            services.AddTransient<IAreaManager, AreaManager>();
            services.AddTransient<ICustomerPriceInfoManager, CustomerPriceInfoManager>();
            services.AddTransient<IRequisitionManager, RequisitionManager>();
            services.AddTransient<IRequisitionIssueManager, RequisitionIssueManager>();
            services.AddTransient<IRequisitionReceiveManager, RequisitionReceiveManager>();
            services.AddTransient<IRequisitionReturnReceiveManager, RequisitionReturnReceiveManager>();
            services.AddTransient<IRequisitionReturnManager, RequisitionReturnManager>();
            services.AddTransient<IStockTransferManager, StockTransferManager>();
            services.AddTransient<IStockTransferRcvManager, StockTransferRcvManager>();
            services.AddTransient<IDistributionManager, DistributionManager>();


            services.AddTransient<IMarketManager, MarketManager>();
            services.AddTransient<ITerritoryManager, TerritoryManager>();

            //--Sales And Distribution Module - location Relation services
            services.AddTransient<IDivisionRegionRelationManager, DivisionRegionRelationManager>();
            services.AddTransient<IRegionAreaRelationManager, RegionAreaRelationManager>();
            services.AddTransient<IAreaTerritoryRelationManager, AreaTerritoryRelationManager>();
            services.AddTransient<ITerritoryMarketRelationManager, TerritoryMarketRelationManager>();
            services.AddTransient<ICreditInfoManager, CreditInfoManager>();
            services.AddTransient<IDepotCustomerManager, DepotCustomerManager>();
            services.AddTransient<ISalesOrderRationingManager, SalesOrderRationingManager>();
            #endregion


            //--Sales And Distribution Module - product configuration serivces
            #region Product configuration
            services.AddTransient<IBrandManager, BrandManager>();
            services.AddTransient<IBaseProductManager, BaseProductManager>();
            services.AddTransient<ICategoryManager, CategoryManager>();
            services.AddTransient<IGroupManager, GroupManager>();
            services.AddTransient<IInvoiceTypeManager, InvoiceTypeManager>();
            services.AddTransient<IPrimaryProductManager, PrimaryProductManager>();
            services.AddTransient<IProductTypeManager, ProductTypeManager>();
            services.AddTransient<IPackSizeManager, PackSizeManager>();
            services.AddTransient<IProductSeasonManager, ProductSeasonManager>();
            services.AddTransient<IStorageManager, StorageManager>();
            services.AddTransient<IMeasuringUnitManager, MeasuringUnitManager>();
            services.AddTransient<IProductManager, ProductManager>();
            services.AddTransient<IProductPriceManager, ProductPriceManager>();
            services.AddTransient<IBonusManager, BonusManager>();
            services.AddTransient<ICustomerMarketRelationManager, CustomerMarketRelationManager>();
            services.AddTransient<IGiftItemManager, GiftItemManager>();
            services.AddTransient<IProductBonusManager, ProductBonusManager>();
            services.AddTransient<IOrderAdjustmentManager, OrderAdjustmentManager>();


            services.AddTransient<IAdjustmentInfoManager, AdjustmentInfoManager>();
            services.AddTransient<IStockAdjustmentManager, StockAdjustmentManager>();
            services.AddTransient<IBatchFreezingManager, BatchFreezingManager>();
            services.AddTransient<IBatchUnFreezingManager, BatchUnFreezingManager>();

            services.AddTransient<ICustomerTypeManager, CustomerTypeManager>();
            services.AddTransient<IPriceTypeManager, PriceTypeManager>();
            services.AddTransient<ICustomerManager, CustomerManager>();
            services.AddTransient<ISupplierManager, SupplierManager>();
            services.AddTransient<IDriverManager, DriverManager>();
            services.AddTransient<IVehicleManager, VehicleManager>();
            services.AddTransient<IDistributionRouteManager, DistributionRouteManager>();
            services.AddTransient<IMiscellaneousIssueManager, MiscellaneousIssueManager>();
            services.AddTransient<ICustomerCommissionManager, CustomerCommissionManager>();
            services.AddTransient<ISkuCommissionManager, SkuCommissionManager>();
            services.AddTransient<ISkuLoadingChargeManager, SkuLoadingChargeManager>();
            services.AddTransient<IDistributorProductRelation, DistributorProductRelation>();

            
            #endregion

            //--Sales And Distribution Module - Transections--------
            #region Transections
            services.AddTransient<IReportManager, ReportManager>();
            services.AddTransient<ISalesOrderManager, SalesOrderManager>();
            services.AddTransient<ISalesInvoiceManager, SalesInvoiceManager>();
            services.AddTransient<ISalesReturnManager, SalesReturnManager>();
            services.AddTransient<ICollectionManager, CollectionManager>();
            services.AddTransient<IGiftItemReceivingManager, GiftItemReceivingManager>();
            services.AddTransient<ICollectionReverseManager, CollectionReverseManager>();
            #endregion


            //--Inventory _Services
            #region Inventory
            services.AddTransient<SalesAndDistributionSystem.Services.Business.Inventory.IReportManager, SalesAndDistributionSystem.Services.Business.Inventory.ReportManager>();
            services.AddTransient<IFgReceiveFromProductionManager, FgReceiveFromProductionManager>();
            services.AddTransient<IFgReceiveFromOthersManager, FgReceiveFromOthersManager>();
            services.AddTransient<IDistributionDeliveryManager, DistributionDeliveryManager>();
            #endregion
            //--Refurbishment_Services
            #region Refurbishment
            services.AddTransient<IRefurbishmentManager, RefurbishmentManager>();
            
            #endregion
            //--Target
            services.AddTransient<ITargetManager, TargetManager>();
            services.AddTransient<IInMarketSales, InMarketSales>();
            services.AddTransient<ITargetReportManager, TargetReportManager>();
            services.AddTransient<IUserAccountRelationManager, UserAccountRelationManager>();
            services.AddTransient<IDashboardManager, DashBoardManager>();

            services.AddControllers(x => x.AllowEmptyInputInBodyModelBinding = true);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x => x.LoginPath = "/Security/Login/Index");
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(1440);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseGlimpse();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/NotFound404";
                    await next();
                }
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                MinimumSameSitePolicy = SameSiteMode.Strict
            });

            //app.UseFastReport();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                name: "Common",
                areaName: "Common",
                pattern: "Common/{controller=CProduct}/{action=GetProductDataFiltered}");
                
                endpoints.MapAreaControllerRoute(
                 name: "Inventory",
                 areaName: "Inventory",
                 pattern: "Inventory/{controller=FGReceive}/{action=ReceiveFromProduction}/{id?}");
                
                endpoints.MapAreaControllerRoute(
                  name: "SalesAndDistribution",
                  areaName: "SalesAndDistribution",
                  pattern: "SalesAndDistribution/{controller=Division}/{action=DivisionInfo}");
                
                endpoints.MapAreaControllerRoute(
                  name: "Security",
                  areaName: "Security",
                  pattern: "Security/{controller=Login}/{action=Index}");

                endpoints.MapAreaControllerRoute(
                    name: "Target",
                    areaName: "Target",
                    pattern: "Target/{controller}/{action}/{id?}");

                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}/{id?}");


                endpoints.MapHub<MessageHub>("/messagehub");
                endpoints.MapHub<NotificationHub>("/notificationhub");
                endpoints.MapHub<UserHub>("/userhub");
            });
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
        }
    }
}
