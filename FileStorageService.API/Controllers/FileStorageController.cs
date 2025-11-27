
using FileStorageService.Application.DTOs;
using FileStorageService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileStorageController : ControllerBase
    {
        private readonly IFileStorageService _service;

        public FileStorageController(IFileStorageService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id) =>
            (await _service.GetByIdAsync(id)) is var file && file != null
                ? Ok(file)
                : NotFound();

        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(int customerId) =>
            Ok(await _service.GetByCustomerIdAsync(customerId));

        [HttpPost]
        public async Task<IActionResult> Create(FileStorageCreateDto dto)
        {
            await _service.AddAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = dto.CustomerId }, null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, FileStorageUpdateDto dto)
        {
            if (await _service.GetByIdAsync(id) == null) return NotFound();
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _service.GetByIdAsync(id) == null) return NotFound();
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
