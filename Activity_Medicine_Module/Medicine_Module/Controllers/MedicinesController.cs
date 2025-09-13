using Microsoft.AspNetCore.Mvc;

namespace ElderCare.Api.Modules.Medical
{
    [ApiController]
    [Route("api/medical/medicines")]
    public class MedicinesController : ControllerBase
    {
        private readonly MedicinesRepository _repo;

        public MedicinesController(MedicinesRepository repo)
        {
            _repo = repo;
        }

        // GET /api/medical/medicines?kw=xx&page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? kw, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var list = await _repo.SearchAsync(kw, page, pageSize);
            return Ok(list);
        }

        // GET /api/medical/medicines/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var row = await _repo.GetByIdAsync(id);
            return row is null ? NotFound() : Ok(row);
        }
    }
}
