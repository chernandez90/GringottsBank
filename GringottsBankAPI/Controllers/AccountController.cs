using Microsoft.AspNetCore.Mvc;
using GringottsBankAPI.DTOs;
using GringottsBankAPI.Models;
using MediatR;
using GringottsBankAPI.Features.Accounts.Commands;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GringottsBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        // GET: api/<AccountController>
        [HttpGet]
        public async Task<ActionResult<Account>> GetAccountBalance([FromBody] AccountDto account)
        {
            var command = new GetAccountBalanceCommand { AccountDto = account };
            var claim =await _mediator.Send(command);
            return Ok();
        }

        // GET api/<AccountController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AccountController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
