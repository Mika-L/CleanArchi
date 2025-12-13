using MediatR;

namespace CleanArchi.Application.Features.Expenses.Commands.CreateExpense
{
    public record CreateExpenseCommand(
        string Description,
        decimal Amount,
        DateTime Date,
        Guid UserId
    ) : IRequest<CreateExpenseResult>;

    public record CreateExpenseResult(
        Guid Id,
        string Description,
        decimal Amount,
        DateTime Date,
        Guid UserId
    );
}
