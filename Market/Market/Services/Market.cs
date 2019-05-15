﻿using Contracts.Models;
using Contracts.Providers;
using Contracts.Responses;
using System.Linq;
using FailureReason = Contracts.Responses.BuyOrderFailedResponse.FailureReason;

namespace Market.Services
{
    public class Market
    {
        private readonly IOrderProvider _ordersProvider;
        private readonly IHoldingsProvider _holdingsProvider;
        private readonly IBankingProvider _bankingProvider;

        public Market(
            IOrderProvider ordersProvider,
            IHoldingsProvider holdingsProvider,
            IBankingProvider bankingProvider)
        {
            _ordersProvider = ordersProvider;
            _holdingsProvider = holdingsProvider;
            _bankingProvider = bankingProvider;
        }

        public IResponse MarketBuy(User user, Stock stock, int quantity)
        {
            var orders = _ordersProvider.GetSellOrders(stock, quantity);
            var bankAccount = _bankingProvider.GetAccount(user);
            if (orders.Sum(x => x.Price) > bankAccount.Balance)
                return new BuyOrderFailedResponse { Reason = FailureReason.InsufficientFunds };

            _holdingsProvider.TransferHoldingsToUser(user, orders);
            _bankingProvider.TransferFundsToHolders(bankAccount, orders);
            _ordersProvider.RemoveOrders(orders);

            Order unfilledOrder = null;
            var unfilled = quantity - orders.Count();
            if (unfilled > 0)
            {
                unfilledOrder = _ordersProvider.AddBuyOrder(user, stock, unfilled);
            }

            return new BuyOrderResponse
            {
                Filled = orders,
                Unfilled = unfilledOrder
            };
        }
    }
}