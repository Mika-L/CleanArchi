using CleanArchi.Application.Common.Interfaces;
using CleanArchi.Domain.Aggregates.ExpenseAggregate;
using CleanArchi.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchi.Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, CreateExpenseResult>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateExpenseCommandHandler> _logger;

        public CreateExpenseCommandHandler(IExpenseRepository repository, IUnitOfWork unitOfWork, ILogger<CreateExpenseCommandHandler> logger)
        {
            _expenseRepository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CreateExpenseResult> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            //// Validation métier (peut être déplacée dans un validator FluentValidation)
            //if (request.Amount <= 0)
            //    throw new ArgumentException("Amount must be greater than zero");

            //if (string.IsNullOrWhiteSpace(request.Description))
            //    throw new ArgumentException("Description is required");

            //// Création de l'entité domain
            //var expense = new Expense
            //{
            //    Id = Guid.NewGuid(),
            //    Description = request.Description,
            //    Amount = request.Amount,
            //    Date = request.Date,
            //    UserId = request.UserId
            //};

            _logger.LogDebug("Creating a new expense for UserId: {UserId}", request.UserId);

            var expense = Expense.Create(
                request.Description,
                request.Amount,
                request.Date,
                request.UserId
            );

            // Sauvegarde via repository
            await _expenseRepository.AddAsync(expense, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogDebug("Expense created {@Expense}", expense);

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
