using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using FacebookAdsLibrary.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace FacebookAdsLibrary.Controllers;

public class FacebookScraperController : Controller
{
    #region Const
    private const string WebhookUrl = "http://localhost:5678/webhook-test/scrape-books";
    #endregion

    #region Field
    private readonly IHttpClientFactory _httpFactory;
    #endregion

    public FacebookScraperController(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            ViewBag.Error = "Search is required";
            return View();
        }

        var client = _httpFactory.CreateClient();

        var json = new StringContent(JsonSerializer.Serialize(new { search }), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(WebhookUrl, json);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var books = JsonSerializer.Deserialize<List<BookVM>>(responseJson,options);

        return View(books);
    }
}
