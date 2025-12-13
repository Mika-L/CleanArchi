using CleanArchi.Domain.Common;
using CleanArchi.Domain.Entities;
using CleanArchi.Domain.Repositories;
using MediatR;

namespace CleanArchi.Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, CreateExpenseResult>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateExpenseCommandHandler(IExpenseRepository repository, IUnitOfWork unitOfWork)
        {
            _expenseRepository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateExpenseResult> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {  
            // Validation métier (peut être déplacée dans un validator FluentValidation)
            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Description is required");

            // Création de l'entité domain
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Amount = request.Amount,
                Date = request.Date,
                UserId = request.UserId
            };

            // Sauvegarde via repository
            await _expenseRepository.AddAsync(expense, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Retour du résultat
            return new CreateExpenseResult(
                expense.Id,
                expense.Description,
                expense.Amount,
                expense.Date,
                expense.UserId
            );
        }
    }
}
