using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eCommerce.Application.Services.Interfaces;
using eCommerce.Application.DTOs.Product;
using eCommerce.Application.DTOs;
namespace eCommerce.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductServices productService) : ControllerBase
    {
        [HttpGet("all")]

        public async Task<IActionResult> GetAll()
        {
            var Data = await productService.GetAllAsync();
            return Data.Any() ? Ok(Data) : NotFound(Data);
        }

        [HttpGet("ById/{id}")]

        public async Task<IActionResult> ById(Guid id)
        {
            var Data = await productService.GetAsync(id);
            return Data != null ? Ok(Data) : NotFound();
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(CreateProduct product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var data = await productService.AddAsync(product);
            return data.Success ? Ok(data) : BadRequest(data);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateProduct product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await productService.UpdateAsync(product);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{id}")]

        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await productService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
