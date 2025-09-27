using GringottsBankAPI.DTOs;
using GringottsBankAPI.Models;
using MediatR;

namespace GringottsBankAPI.Features.Accounts.Commands
{
    public class GetAccountBalanceCommand :IRequest<Account>
    {
        public AccountDto AccountDto { get; set; } = null!;
    }
}
