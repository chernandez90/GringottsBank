using GringottsBankAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GringottsBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountInfoController : ControllerBase
    {
        private readonly BankDbContext _context;
        public AccountInfoController(BankDbContext context)
        {
            _context = context;
        }
        // GET: api/<AccountController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAllAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return Ok(accounts);
        }

        // GET api/<AccountController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountBalance(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            return Ok(account);
        }
    }
}
