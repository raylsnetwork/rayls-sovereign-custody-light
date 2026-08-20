using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.DTO.Configuration;
using Rayls.Custody.HSM.DTO.Errors;
using Rayls.Custody.HSM.Service.Interface.Repositories;
using Rayls.Custody.HSM.Service.Interface.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service
{
    public class AuthService : IAuthService
    {
        private readonly ApiConfig _apiConfig;
        public AuthService(
            ApiConfig apiConfig, ITokenRepository tokenRepository, IMapper mapper, IWalletService walletService)
        {
            _apiConfig = apiConfig;
        }

        public Task<string> GenerateBearerToken(string apiKey)
        {
            if (string.IsNullOrEmpty(_apiConfig.AUTH_API_KEY))
                throw new InvalidOperationException("AUTH_API_KEY is not configured");

            if (apiKey != _apiConfig.AUTH_API_KEY)
                throw new UnauthorizedApiKeyException();

            // HMAC-SHA256 requires a key of at least 256 bits; a shorter secret makes
            // SigningCredentials throw (IDX10653) on every request, which reads as an
            // auth failure unless it is called out explicitly here.
            var secret = _apiConfig.AUTH_JWT_SECRET ?? string.Empty;
            if (Encoding.UTF8.GetByteCount(secret) < 32)
                throw new InvalidOperationException(
                    "AUTH_JWT_SECRET must be at least 32 bytes (256 bits) for HmacSha256; got "
                    + Encoding.UTF8.GetByteCount(secret) + " bytes");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken("CustodyHSM",
              "CustodyHSM",
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return Task.FromResult<string>(new JwtSecurityTokenHandler().WriteToken(Sectoken));
        }

    }
}