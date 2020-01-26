namespace API
{
    using Persistence;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.EntityFrameworkCore;
    using MediatR;
    using Application.Activities;
    using FluentValidation.AspNetCore;
    using Middleware;
    using Domain;
    using Microsoft.AspNetCore.Identity;
    using Application.Interfaces;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using AutoMapper;
    using Infrastructure.Photos;
    using SignalR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using System.Threading.Tasks;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opts =>
            {
                opts.UseLazyLoadingProxies();
                opts.UseSqlite(this.Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddCors(opts =>
            {
                opts.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials();
                });
            });

            services.AddMediatR(typeof(List.Handler).Assembly);

            services.AddAutoMapper(typeof(List.Handler));

            services.AddSignalR();

            services
                .AddControllers(opt =>
                {
                    AuthorizationPolicy policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                    opt.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddFluentValidation(cfg =>
                {
                    cfg.RegisterValidatorsFromAssemblyContaining<Create>();
                });

            IdentityBuilder builder = services.AddIdentityCore<AppUser>();
            IdentityBuilder identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("IsActivityHost", policy =>
                {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });

            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

            SymmetricSecurityKey key =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["TokenKey"]));
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };

                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            StringValues accessToken = context.Request.Query["access_token"];
                            PathString path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();

            services.Configure<CloudinarySettings>(this.Configuration.GetSection("Cloudinary"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
