using CleanArchi.Application.Features.Expenses.Commands.CreateExpense;
using CleanArchi.Application.Features.Expenses.Queries.GetExpense;
using CleanArchi.Domain.Aggregates.ExpenseAggregate;
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
            var result = await _mediator.Send(new GetExpenseQuery(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
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
