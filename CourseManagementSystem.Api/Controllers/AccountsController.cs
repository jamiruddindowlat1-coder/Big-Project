using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Core.Models.Entities;

namespace CourseManagementSystem.Api.Controllers
{
    [Route("api/Accounts")]
    [ApiController]
    [Tags("Money Receipt / Receive (Collections)")]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Accounts (Ledger)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountTransaction>>> GetLedger()
        {
            return await _context.Accounts.ToListAsync();
        }

        // POST: api/Accounts (Receive Money)
        [HttpPost]
        public async Task<ActionResult<AccountTransaction>> ReceiveNewPayment(AccountTransaction transaction)
        {
            _context.Accounts.Add(transaction);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetLedger", new { id = transaction.Id }, transaction);
        }

        // GET: api/Accounts/balance
        [HttpGet("balance")]
        public async Task<ActionResult<object>> GetTotalStats()
        {
            var transactions = await _context.Accounts.ToListAsync();
            decimal totalCredit = transactions.Where(t => t.TransactionType?.ToUpper() == "CREDIT").Sum(t => t.Amount);
            decimal totalDebit = transactions.Where(t => t.TransactionType?.ToUpper() == "DEBIT").Sum(t => t.Amount);

            return new { 
                TotalReceipts = totalCredit, 
                TotalExpenses = totalDebit, 
                InHandCash = totalCredit - totalDebit 
            };
        }

        // PUT: api/Accounts/5 (Edit Entry)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntry(int id, AccountTransaction transaction)
        {
            if (id != transaction.Id) return BadRequest();
            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Accounts/5 (Delete Entry)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            var transaction = await _context.Accounts.FindAsync(id);
            if (transaction == null) return NotFound();
            _context.Accounts.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}