using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingBuddy.CatalogService.Api.Core.Domain;
using SellingBuddy.CatalogService.Api.Infrastructure.Context;
using System.Net;

namespace SellingBuddy.CatalogService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PicController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly CatalogContext _catalogContext;

    public PicController(IWebHostEnvironment env, CatalogContext catalogContext)
    {
        _env = env;
        _catalogContext = catalogContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok("App and Running");
    }

    [HttpGet]
    [Route("api/v1/catalog/items/{catalogItemId:int}/pic")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetImageAsync(int catalogItemId)
    {
        if (catalogItemId <= 0) 
        {
            return BadRequest();
        }

        var item = await _catalogContext.CatalogItems
            .SingleOrDefaultAsync(ci => ci.Id == catalogItemId);

        if (item != null) 
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, item.PictureFileName);

            string imageFileExtension = Path.GetExtension(item.PictureFileName);
            //string mimeType = GetImageMimeTypeFromImageFileExtension(imageFileExtension);

            var buffer = await System.IO.File.ReadAllBytesAsync(path);

            //return Ok(File(buffer, mimeType));
        }

        return NotFound();
    }

    //private string GetImageMimeTypeFromImageFileExtension(string imageFileExtension)
    //{
    //    string mimeType = null;

    //    switch (imageFileExtension)
    //    {
            
    //    }
    //}
}
