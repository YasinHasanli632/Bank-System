using CustomerService.Application.DTOs;
using CustomerService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service) => _service = service;

      
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> GetById(int id) =>
            await _service.GetByIdAsync(id) is CustomerDto customer ? Ok(customer) : NotFound();

     
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (await _service.CustomerExistsAsync(dto.Email))
                return Conflict(new { message = "Email already exists." });

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

       
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "Route ID and DTO ID mismatch." });
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (await _service.GetByIdAsync(id) is null) return NotFound();

            await _service.UpdateAsync(dto);
            return NoContent();
        }

     
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.GetByIdAsync(id) is null ? NotFound() : await _service.DeleteAsync(id).ContinueWith(_ => NoContent());
    }
}

