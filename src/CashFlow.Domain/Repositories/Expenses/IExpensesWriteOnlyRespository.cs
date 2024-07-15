using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;

public interface IExpensesWriteOnlyRespository
{
    Task Add(Expense expense);
    /// <summary>
    /// This function returns True if the deletion was successful otherwise returns False
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Delete(long id);
}