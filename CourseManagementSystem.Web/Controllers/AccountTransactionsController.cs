using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Infrastructure.Data;
using CourseManagementSystem.Core.Models.Entities;

namespace CourseManagementSystem.Web.Controllers
{
    [Route("api/Accounts")]
    [ApiController]
    [Tags("Money Receipt / Receive")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Accounts (Get Ledger Summary)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountTransaction>>> GetLedger()
        {
            return await _context.Accounts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountTransaction>> GetEntryLine(int id)
        {
            var transaction = await _context.Accounts.FindAsync(id);
            if (transaction == null) return NotFound();
            return transaction;
        }

        // POST: api/Accounts (Capture Money Receipt / Expense)
        [HttpPost]
        public async Task<ActionResult<AccountTransaction>> ReceiveNewMoney(AccountTransaction transaction)
        {
            _context.Accounts.Add(transaction);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetLedger", new { id = transaction.Id }, transaction);
        }

        [HttpGet("balance")]
        public async Task<ActionResult<object>> GetRealtimeBalance()
        {
            var transactions = await _context.Accounts.ToListAsync();
            decimal totalCredit = 0;
            decimal totalDebit = 0;

            foreach (var t in transactions)
            {
                if (t.TransactionType?.ToUpper() == "CREDIT")
                    totalCredit += t.Amount;
                else if (t.TransactionType?.ToUpper() == "DEBIT")
                    totalDebit += t.Amount;
            }

            return new { 
                TotalIncome = totalCredit, 
                TotalExpense = totalDebit, 
                InHandBalance = totalCredit - totalDebit 
            };
        }

        // PUT: api/Accounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMoneyEntry(int id, AccountTransaction transaction)
        {
            if (id != transaction.Id) return BadRequest();
            _context.Entry(transaction).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { if (!TransactionExists(id)) return NotFound(); else throw; }
            return NoContent();
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoneyEntry(int id)
        {
            var transaction = await _context.Accounts.FindAsync(id);
            if (transaction == null) return NotFound();
            _context.Accounts.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool TransactionExists(int id) => _context.Accounts.Any(e => e.Id == id);
    }
}
