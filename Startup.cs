using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using TestSlabon.Data;
using TestSlabon.Services;
using TestSlabon.Startups;

namespace TestSlabon
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
            // Servicio para habilitar los CORS
            services.AddCors(option =>
            {
                option.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                    });
            });
            // Agregamos nuestro servicio para la auteticación con JWT en nuestra API
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecretKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            // Aquí capturamos los errores detectados  antes de entrar al controlador, para enviar el mismo modelo de respuesta
            services.AddMvc().ConfigureApiBehaviorOptions(opt =>
            {
                opt.InvalidModelStateResponseFactory = (context =>
                {
                    return Utils.Helper.ErrorResponse(context);
                });
            });
            services.AddControllers();
            // Agregamos el contexto y proveedor SQL Server para la conexión a la base de datos
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<SlabonContext>(options => options.UseSqlServer(connection));
            // Servicio para generar nuestra documentación de la API con la especificación OpenAPI 3.0.1
            ApiDocStartup.ConfigureServices(services);
            // Inyectamos nuestro servicio que sera el encargado de las operaciones con la base de datos
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Genera nuestra Swagger UI, interfaz donde podremos hacer las pruebas de nuestra API.
            ApiDocStartup.Configure(app);

            app.UseRouting();

            app.UseCors();

            //habilitar la autenticación para el token
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
