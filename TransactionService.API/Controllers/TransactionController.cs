using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;

namespace TransactionService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionReadDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

      
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TransactionReadDto>> GetById(int id)
        {
            var transaction = await _service.GetByIdAsync(id);
            return transaction is null ? NotFound() : Ok(transaction);
        }

      
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransactionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.TransactionId }, created);
        }

       
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TransactionUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var updated = await _service.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

     
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
            => await _service.DeleteAsync(id) ? NoContent() : NotFound();

      
        [HttpGet("account/{accountId:int}")]
        public async Task<ActionResult<IEnumerable<TransactionReadDto>>> GetByAccountId(int accountId)
            => Ok(await _service.GetByAccountIdAsync(accountId));
    }
}
