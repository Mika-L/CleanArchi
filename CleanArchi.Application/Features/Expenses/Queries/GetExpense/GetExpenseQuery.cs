using CleanArchi.Application.DTOs;
using CleanArchi.Domain.Common;
using MediatR;

namespace CleanArchi.Application.Features.Expenses.Queries.GetExpense
{
    public record GetExpenseQuery(Guid Id) : IRequest<Result<ExpenseDto>>;

    public record GetExpenseResult(
        Guid Id,
        string Description,
        decimal Amount,
        DateTime Date,
        Guid UserId
    );
}
