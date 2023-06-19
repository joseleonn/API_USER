using API_USER.Service.IServices;
using API_USER.Service.Services;
using Service.Service.Services;
namespace API_USER
{
    public class CompositeRoot
    {
        public static void DependencyInjection(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserService, UserService>();

        }

    }
}
