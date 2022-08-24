using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using AutoMapper;
using Golf.DataMapper;
using Golf.HttpApi.Host.Middleware;
using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services;
using Golf.Domain.GolferData;
using Golf.Services.Courses;
using Golf.Services.Products;
using Golf.Services.Locations;
using Golf.Services.Bookings;
using Golf.Services.Notifications;
using SignalRChat.Hubs;
using Golf.Services.messages;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Golf.Services.Events;
using Golf.Services.Tournaments;
using Golf.Domain.Shared.Setting;
using Golf.Domain.Shared.AccessToken;
using Golf.Services.CourseAdmin;
using Golf.Services.AdminService;
using Golf.Services.OdooAPI;
using Golf.Services.SystemSettings;
using Golf.Services.Reports;
using Golf.Services.Memberships;
using Golf.Services.Resource;

namespace Golf
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Repository
            services.AddTransient<GolferRepository>();
            services.AddTransient<ProfileRepository>();
            services.AddTransient<PostVoteRepository>();
            services.AddTransient<CommentRepository>();
            services.AddTransient<PostRepository>();
            services.AddTransient<SystemSettingRepository>();
            services.AddTransient<PhotoRepository>();
            services.AddTransient<RelationshipRepository>();
            services.AddTransient<GroupRepository>();
            services.AddTransient<GroupMemberRepository>();
            services.AddTransient<CourseRepository>();
            services.AddTransient<ProductRepository>();
            services.AddTransient<LocationRepository>();
            services.AddTransient<CourseReviewRepository>();
            services.AddTransient<TransactionRepository>();
            services.AddTransient<CourseExtensionRepository>();
            services.AddTransient<ScorecardRepository>();
            services.AddTransient<NotificationRepository>();
            services.AddTransient<MemberShipRepository>();
            services.AddTransient<ConversationRepository>();
            services.AddTransient<MessageRepository>();
            services.AddTransient<EventRepository>();
            services.AddTransient<TournamentMemberRepository>();
            services.AddTransient<TournamentRepository>();
            services.AddTransient<SystemSettingRepository>();
            services.AddTransient<ScorecardVoteRepository>();
            services.AddTransient<ReportRepository>();

            // Service
            services.AddTransient<AuthService>();
            services.AddTransient<ProfileService>();
            services.AddTransient<PostService>();
            services.AddTransient<PhotoService>();
            services.AddTransient<CommentService>();
            services.AddTransient<GolferService>();
            services.AddTransient<RelationshipService>();
            services.AddTransient<PostVoteService>();
            services.AddTransient<GroupService>();
            services.AddTransient<CourseService>();
            services.AddTransient<GroupMemberService>();
            services.AddTransient<ProductService>();
            services.AddTransient<LocationService>();
            services.AddTransient<CourseReviewService>();
            services.AddTransient<TransactionService>();
            services.AddTransient<CourseAdminService>();
            services.AddTransient<StatisticService>();
            services.AddTransient<ScorecardService>();
            services.AddTransient<HandicapService>();
            services.AddTransient<NewsfeedService>();
            services.AddTransient<SeedDataService>();
            services.AddTransient<CourseExtensionService>();
            services.AddTransient<CourseManagerService>();
            services.AddTransient<NotificationService>();
            services.AddTransient<ChartsService>();
            services.AddTransient<DatabaseTransaction>();
            services.AddTransient<ConversationService>();
            services.AddTransient<HubSignalR>();
            services.AddTransient<MemberShipService>();
            services.AddTransient<EventService>();
            services.AddTransient<TournamentService>();
            services.AddTransient<AccessTokenHandler>();
            services.AddSingleton<OdooAPIService>();
            services.AddTransient<AccountManageService>();
            services.AddTransient<SystemSettingService>();
            services.AddTransient<ReportService>();
            services.AddTransient<ShopService>();
            services.AddTransient<MembershipService>();
            services.AddTransient<ResourceService>();

            // services.AddSingleton<NotificationsHub>();


            //services.AddTransient<IHubContext<NotificationsHub>>();//note
            services.AddSignalR();
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddAutoMapper(c => c.AddProfile<MappingProfile>(), typeof(Startup));
            services.AddDbContext<GolfDbContext>(options =>
                 options.UseNpgsql(
                     Configuration.GetConnectionString("MyWebApiConnection"),
                     b => b.MigrationsAssembly("Golf.DbMigrator")
            ));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddIdentity<Golfer, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<GolfDbContext>();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "Golf API",
                        Description = "Golf Backend API",
                        Version = "V1"
                    });
                c.IncludeXmlComments(string.Format(@"{0}/Golf.HttpApi.Host.xml",
                           System.AppDomain.CurrentDomain.BaseDirectory));
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            }).AddSwaggerGenNewtonsoftSupport();
            services.AddDirectoryBrowser();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            app.UseCors(x => x
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .SetIsOriginAllowed(origin => true) // allow any origin
                 .AllowCredentials());

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Golf API V1");

            });

            app.UseRouting();

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Photos")),
                RequestPath = new PathString("/images")
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Photos")),
                RequestPath = new PathString("/images")
            });

            app.UseAuthorization();

            //app.UseCors(x => x.WithOrigins(Configuration.GetSection("AllowedOrigins").Value.Split(","))
            //   .AllowAnyHeader()
            //   .AllowAnyMethod()
            //   .AllowCredentials());

            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<HubSignalR>("/chatHub");
          
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //    endpoints.MapHub<NotificationsHub>("/notificationHub");
            //});
            //app.Use(async (context, next) =>
            //{
            //    var hubContext = context.RequestServices
            //                            .GetRequiredService<IHubContext<ChatHub>>();
            //    //...

            //    if (next != null)
            //    {
            //        await next.Invoke();
            //    }
            //});
            // NotificationsHub.Current = app.ApplicationServices.GetServices<IHubContext<NotificationsHub>>; 

        }
    }
}
