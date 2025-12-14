using CleanArchi.Application.DTOs;
using CleanArchi.Domain.Common;
using CleanArchi.Domain.Repositories;
using MediatR;

namespace CleanArchi.Application.Features.Expenses.Queries.GetExpense
{
    public class GetExpenseHandler : IRequestHandler<GetExpenseQuery, Result<ExpenseDto>>
    {
        private readonly IExpenseRepository _expenseRepository;

        public GetExpenseHandler(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<Result<ExpenseDto>> Handle(
            GetExpenseQuery request,
            CancellationToken cancellationToken)
        {
            var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (expense is null)
            {
                return Result<ExpenseDto>.Failure(new Error("Expense.NotFound", $"Expense with ID {request.Id} not found."));
            }

            var expenseDto = new ExpenseDto()
            {
                Id = expense.Id,
                Description = expense.Description,
                Amount = expense.Amount,
                Date = expense.Date,
                UserId = expense.UserId
            };

            return Result<ExpenseDto>.Success(expenseDto);
        }
    }
}
