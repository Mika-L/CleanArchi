using CleanArchi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchi.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpenseController : ControllerBase
    {
        [HttpGet]
        public Expense Get(Guid guid)
        {
            return new Expense
            {
                Id = guid,
                Description = "Sample Expense",
                Amount = 100.00m,
                Date = DateTime.UtcNow,
                UserId = Guid.NewGuid()
            };
        }
    }
}
