using GringottsBankAPI.DTOs;
using GringottsBankAPI.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GringottsBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public AccountTransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        // GET: api/<AccountTransactions>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetAllTransactions()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetTransactionsByAccountId(int accountId)
        {
            var transactions = await _transactionService.GetTransactionsByAccountIdAsync(accountId);
            return Ok(transactions);
        }
        // POST api/<AccountTransactions>
        [HttpPost]
        public async Task<ActionResult<TransactionResponseDto>> CreateTransaction([FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
            {
                return BadRequest("Transaction data is null.");
            }

            var (transaction, errorMessage) = await _transactionService.ProcessTransactionAsync(transactionDto);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }

            return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponseDto>> GetTransactionById(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }

            return Ok(transaction);
        }
    }
}
