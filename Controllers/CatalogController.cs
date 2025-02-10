using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PalladiumBE.Utilities.AWS;

namespace PalladiumBE.Controllers;


[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ILogger<CatalogController> _logger;
    private readonly IMongoDatabase _database;

    public CatalogController(ILogger<CatalogController> logger) {
        _logger = logger;
        
        string connectionString = AppConfig.DBConnectionString;
        string databaseName = "catalog";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    [HttpGet(Name = "GetCatalogs")]
    public IActionResult Get()
    {
        try
        {
            var filter = Builders<CatalogItem>.Filter.Empty;
            var catalogItems = _database.GetCollection<CatalogItem>("CatalogItems").Find(filter).ToList();
            return Ok(catalogItems);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when getting catalog items: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("categories/{catalogId}", Name = "GetCategories")]
    public IActionResult GetCategories(string catalogId)
    {
        try
        {
            var filter = Builders<Item>.Filter.Eq("catalogId", catalogId);
            var items = _database.GetCollection<Item>("Items").Find(filter).ToList();
            var categories = items.Select(item => item.category).Distinct().ToList();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when getting catalog categories: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("items/{catalogId}", Name = "GetItems")]
    public IActionResult GetItems(string catalogId)
    {
        try
        {
            var filter = Builders<Item>.Filter.Eq("catalogId", catalogId);
            var items = _database.GetCollection<Item>("Items").Find(filter).ToList();

            foreach (var item in items)
            {
                item.thumbnailUrl = S3Helper.RetrieveThumbnailImage(item.id);
            }

            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when getting items: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("item/thumbnail", Name = "UploadItemThumbnail")]
    public async Task<IActionResult> UploadItemThumbnail(UploadItemThumbnailRequest request)
    {
        try
        {
            await S3Helper.UploadImages(request.ItemId, new[] { request.Image }, keyName: "thumbnail");
            return Ok("Thumbnail uploaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when uploading thumbnail: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("item/{itemId}/photos", Name = "GetItemPhotos")]
    public async Task<IActionResult> GetItemPhotos(string itemId)
    {
        try
        {
            var photos = await S3Helper.RetrieveItemPhotos(itemId);
            return Ok(photos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when uploading photos: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("item/photos", Name = "UploadItemPhotos")]
    public async Task<IActionResult> UploadItemThumbnail(UploadPicturesRequest request)
    {
        try
        {
            await S3Helper.UploadImages(request.ItemId, request.Images, "photos");
            return Ok("Pictures are uploaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when uploading pictures: {ex.Message}");
            return StatusCode(500, "Internal Server Error");
        }

    }
}