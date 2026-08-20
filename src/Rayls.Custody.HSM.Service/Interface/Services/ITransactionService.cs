using Rayls.Custody.HSM.DTO;
using System.Threading.Tasks;

namespace Rayls.Custody.HSM.Service.Interface.Services
{
    public interface ITransactionService
    {
        Task<ViewTransactionResponse> ViewTransactionAsync(ViewTransactionRequest request);
        Task<TransactionResponse> CreateTransactionAsync(TransactionRequest<TransactionEvmBase> request);
        Task<TransactionResponse> CreateTransactionAsync(TransactionRequest<TransferTokenBase> request);
        Task<TransactionResponse> CreateTransactionAsync(TransactionRequest<TransferTokenBridgeBase> request);
        Task<Nethereum.RPC.Eth.DTOs.Transaction> GetTransactionByHash(string hash);
        Task<Nethereum.RPC.Eth.DTOs.TransactionReceipt> GetTransactionReceiptByHash(string hash);
    }
}
