using AutoMapper;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Rayls.Custody.HSM.DTO;
using Rayls.Custody.HSM.Service.Interface.Repositories;
using Rayls.Custody.HSM.Service.Interface.Services;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service
{
    public class TokenService : ITokenService
    {
        private readonly Erc20Console _erc20;
        private readonly ITokenRepository _tokenRepository;
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;

        public TokenService(
            ITokenRepository tokenRepository, IMapper mapper, IWalletService walletService, Erc20Console erc20)
        {
            _tokenRepository = tokenRepository;
            _mapper = mapper;
            _walletService = walletService;
            _erc20 = erc20;
        }

        public async Task<BalanceResponse> GetBalanceByTokenIdAsync(string id, string walletId)
        {
            var token = await GetTokenByIdAsync(id);
            var wallet = await _walletService.GetWalletByIdAsync(walletId);
            return new BalanceResponse()
            {
                Balance = (await _erc20.BalanceOf(token.Address, wallet.Address)).ToString()
            };
        }

        public async Task<TokenResponse> GetTokenByIdAsync(string id)
        {
            var model = await _tokenRepository.GetByIdAsync(id);
            var response = _mapper.Map<TokenResponse>(model);
            return response;
        }

        public async Task<TokenResponse> GetTokenByAddressAsync(string address)
        {
            var model = await _tokenRepository.GetByAddressAsync(address);
            var response = _mapper.Map<TokenResponse>(model);
            return response;
        }


        public async Task<(IEnumerable<TokenResponse>, int)> GetTokenListAsync(int pageNumber, int pageSize)
        {
            var list = await _tokenRepository.GetListAsync(pageNumber, pageSize);

            var response = _mapper.Map<IEnumerable<TokenResponse>>(list.Item1);

            return (response, list.Item2);
        }

        public async Task<TokenResponse> RegisterTokenByAddress(string tokenAddress)
        {
            var nameTask = _erc20.Name(tokenAddress);
            var symbolTask = _erc20.Symbol(tokenAddress);
            var decimalsTask = _erc20.Decimals(tokenAddress);
            var alreadyExistsTokenTask = GetTokenByAddressAsync(tokenAddress);
            await Task.WhenAll(nameTask, symbolTask, decimalsTask, alreadyExistsTokenTask);

            var tokenAlreadyCreated = await alreadyExistsTokenTask;
            if (tokenAlreadyCreated != null)
                return tokenAlreadyCreated;

            var token = new Interface.Repositories.Model.Token()
            {
                Name = await nameTask,
                Symbol = await symbolTask,
                Decimals = await decimalsTask,
                Address = tokenAddress.ToLower()
            };

            await _tokenRepository.SaveAsync(token);

            return _mapper.Map<TokenResponse>(token);
        }

        public async Task<string> TransferTokenAsync(string tokenAddress, Account account, string to, HexBigInteger value)
        {
            var txHash = await _erc20.Transfer(tokenAddress, account, to, value);
            return txHash;
        }

        public async Task<string> TransferTokenBridgeAsync(BigInteger toChainId, string tokenAddress, string toTokenAddress,Nethereum.Web3.Accounts.Account account, string to, BigInteger value)
        {
            var txHash = await _erc20.TransferRequest(toChainId, tokenAddress, account, to, value, toTokenAddress);
            return txHash;
        }
    }
}
