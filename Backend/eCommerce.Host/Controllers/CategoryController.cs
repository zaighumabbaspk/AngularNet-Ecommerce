using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Application.DTOs.Category;
using eCommerce.Application.DTOs;

namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryServices categoryService) : ControllerBase
    {
        [HttpGet("all")]
        [AllowAnonymous]  // Everyone can view categories
        public async Task<IActionResult> GetAll()
        {
            var data = await categoryService.GetAllAsync();
            return data.Any() ? Ok(data) : NotFound(data);
        }

        [HttpGet("ById/{id}")]
        [AllowAnonymous]  // Everyone can view category details
        public async Task<IActionResult> ById(Guid id)
        {
            var data = await categoryService.GetAsync(id);
            return data != null ? Ok(data) : NotFound();
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]  // Only Admin can add categories
        public async Task<IActionResult> Add(CreateCategory category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var data = await categoryService.AddAsync(category);
            return data.Success ? Ok(data) : BadRequest(data);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]  // Only Admin can update categories
        public async Task<IActionResult> Update(UpdateCategory category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await categoryService.UpdateAsync(category);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]  // Only Admin can delete categories
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await categoryService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
