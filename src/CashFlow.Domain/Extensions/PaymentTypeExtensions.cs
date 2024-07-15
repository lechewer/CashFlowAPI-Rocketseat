

using CashFlow.Domain.Enums;
using CashFlow.Domain.Reports;

namespace CashFlow.Domain.Extensions;

public static class PaymentTypeExtensions
{
    public static string PaymentTypeToString(this PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash => ResourceReportPaymentType.CASH,
            PaymentType.CreditCard => ResourceReportPaymentType.CREDITCARD,
            PaymentType.DebitCard => ResourceReportPaymentType.DEBITCARD,
            PaymentType.EletronicTransfer => ResourceReportPaymentType.ELETRONICTRANSFER,
            _ => string.Empty
        };
    }
}