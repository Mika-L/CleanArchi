using CleanArchi.Domain.Repositories;
using MediatR;

namespace CleanArchi.Application.Features.Expenses.Queries.GetExpense
{
    public class GetExpenseHandler : IRequestHandler<GetExpenseQuery, GetExpenseResult>
    {
        private readonly IExpenseRepository _expenseRepository;

        public GetExpenseHandler(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<GetExpenseResult> Handle(
            GetExpenseQuery request,
            CancellationToken cancellationToken)
        {
            var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (expense is null)
            {
                return null;
            }

            var expenseDto = new GetExpenseResult
            (
                expense.Id,
                expense.Description,
                expense.Amount,
                expense.Date,
                expense.UserId
            );

            return expenseDto;
        }
    }
}
