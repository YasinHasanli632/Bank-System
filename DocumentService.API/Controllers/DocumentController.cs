using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocumentService.Application.DTOs;
using DocumentService.Application.Services;
using DocumentService.Application.Interfaces;
namespace DocumentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _service;

        public DocumentController(IDocumentService service)
        {
            _service = service;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

       
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var document = await _service.GetByIdAsync(id);
            if (document == null)
                return NotFound($"Document with ID {id} not found.");

            return Ok(document);
        }

        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var docs = await _service.GetByCustomerIdAsync(customerId);
            return Ok(docs);
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDocumentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DocumentId }, created);
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentDto dto)
        {
            if (id != dto.DocumentId)
                return BadRequest("Mismatched document ID.");

            var updated = await _service.UpdateAsync(dto);
            if (updated == null)
                return NotFound($"Document with ID {id} not found.");

            return Ok(updated);
        }

     
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Document with ID {id} not found.");

            return NoContent();
        }
    }
}
