using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using FacebookAdsLibrary.Models;

namespace FacebookAdsLibrary.Controllers;

public class FacebookScraperController : Controller
{
    private readonly IHttpClientFactory _httpFactory;

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
    public async Task<IActionResult> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            ViewBag.Error = "Please enter a keyword.";
            return View();
        }

        var client = _httpFactory.CreateClient();
        var webhookUrl = "http://localhost:5678/webhook/scrape-books";

        var postData = new { keyword };
        var json = new StringContent(JsonSerializer.Serialize(postData), System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync(webhookUrl, json);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var books = JsonSerializer.Deserialize<List<BookVM>>(responseJson);

        return View(books);
    }
}
