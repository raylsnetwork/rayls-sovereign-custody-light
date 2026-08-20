using Rayls.Custody.HSM.DTO.Enums;

namespace Rayls.Custody.HSM.DTO.Const
{
    public class CustodyHSMRoutes
    {
        #region Health
        public const string Health = "health";
        #endregion

        #region Auth
        public const string Auth = "auth";
        #endregion

        #region Setup
        #endregion

        #region Token
        public const string CreateToken = "token";
        public const string GetTokenList = "token";
        public const string GetTokenById = "token/{id}";
        public const string GetBalanceByTokenId = "token/{id}/balance/{walletId}";
        #endregion

        #region Wallet
        public const string CreateWallet = "wallet";
        public const string GetWalletList = "wallet";
        public const string GetWalletById = "wallet/{id}";
        public const string GetWalletByAddress = "wallet/address/{address}";
        public const string AssociateWalletExternalId = "wallet/{id}/external/{external_id}";
        #endregion

        #region Transaction

        public const string CreateTransaction = "transaction";
        public const string ViewTransaction = "transaction/view";
        public const string CreateTransactionToTransferToken = "transaction/transfer_token";
        public const string CreateTransactionToTransferTokenBridge = "transaction/transfer_token_bridge";
        public const string GetTransactionList = "transaction";
        public const string GetTransactionByHash = "transaction/{hash}";
        public const string GetTransactionReceiptByHash = "transaction/receipt/{hash}";

        #endregion
    }
}
