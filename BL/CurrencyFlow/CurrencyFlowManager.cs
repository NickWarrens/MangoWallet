using BL.ActionResults;
using DAL;
using Domain.Balances;
using Domain.Currencies.BaseCurrency;
using Domain.Users;

namespace BL.CurrencyFlow;

public class CurrencyFlowManager : ICurrencyFlowManager
{
    private readonly IWalletRepository _walletRepository;

    public CurrencyFlowManager(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task AddAmountToUserAsync(User user, CurrencyType currencyType, double amount)
    {
        user.UserWallet.AddCurrency(currencyType, amount);
        await _walletRepository.UpdateAsync(user.UserWallet);
    }

    public async Task SubtractAmountToUserAsync(User user, CurrencyType currencyType, double amount)
    {
        user.UserWallet.SubtractCurrency(currencyType, amount);
        await _walletRepository.UpdateAsync(user.UserWallet);
    }

    public async Task<TransferResult> TransferCurrencyAsync(User user, string walletKey, CurrencyType currencyType, double amount)
    {
        UserWallet? wallet = await _walletRepository.GetByWalletKeyAsync(walletKey);
        if (wallet == null)
            return new TransferResult(false, "Invalid wallet key");

        if (!user.UserWallet.SubtractCurrency(currencyType, amount))
            return new TransferResult(false, "Insufficient balance.");

        wallet.AddCurrency(currencyType, amount);
        await _walletRepository.UpdateAsync(user.UserWallet);
        await _walletRepository.UpdateAsync(wallet);

        return new TransferResult(true, $"Successfully transferred {amount} {currencyType} to {walletKey}");
    }

    public async Task<ExchangeResult> ExchangeCurrency(User user, CurrencyType currencyToExchange, CurrencyType targetCurrency, double amount)
    {
        if (!user.UserWallet.SubtractCurrency(currencyToExchange, amount))
            return new ExchangeResult(false, "Insufficient balance.");

        double baseRateAmount = amount * CurrencyMetaDataProvider.GetCurrencyBaseRate(currencyToExchange);
        double targetAmount = Math.Round(baseRateAmount / CurrencyMetaDataProvider.GetCurrencyBaseRate(targetCurrency), 3);

        user.UserWallet.AddCurrency(targetCurrency, targetAmount);
        await _walletRepository.UpdateAsync(user.UserWallet);
        return new ExchangeResult(true, $"Exchanged {amount} {currencyToExchange} to {targetAmount} {targetCurrency}");
    }
}