using CleanArchi.Application.Features.Expenses.Commands.CreateExpense;
using CleanArchi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchi.Web.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpenseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Expense), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            // var result = await _mediator.Send(new GetExpenseQuery(id), cancellationToken);

            return Ok(new Expense
            {
                Id = id,
                Description = "Sample Expense",
                Amount = 100.00m,
                Date = DateTime.UtcNow,
                UserId = Guid.NewGuid()
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateExpenseResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateExpenseCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { id = result.Id },
                result
            );
        }
    }
}
