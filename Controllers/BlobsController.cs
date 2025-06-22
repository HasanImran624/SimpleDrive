using Microsoft.AspNetCore.Mvc;
using SimpleDrive.Models;
using SimpleDrive.Services;

namespace SimpleDrive.Controllers
{
    [ApiController]
    [Route("v1/blobs")]
    public class BlobsController : ControllerBase
    {
        private readonly BlobService _blobService;
        public BlobsController(BlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromBody] BlobRequest request)
        {
            await _blobService.StoreBlobAsync(request.Id, request.Data);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _blobService.GetBlobAsync(id);
            return Ok(result);
        }

    }
}
