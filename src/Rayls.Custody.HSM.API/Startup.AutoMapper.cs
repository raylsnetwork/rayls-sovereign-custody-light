using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.Service.Interface.Repositories.Model;

namespace Rayls.Custody.HSM.API
{
    public partial class Startup
    {
        private void ConfigureAutoMapper(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Wallet Create
                cfg.CreateMap<WalletRequest, Wallet>();
                cfg.CreateMap<Wallet, WalletResponse>();
                cfg.CreateMap<Token, TokenResponse>();
            });

            IMapper mapper = config.CreateMapper();

            services.AddSingleton(mapper);
        }
    }
}
