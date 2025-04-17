using Microsoft.AspNetCore.Mvc;
using Azure.Core;
 using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stripe;
using Ecommerce1;
using Microsoft.AspNetCore.Http.HttpResults;
using Stripe.V2;
using System.Text.Json;
using static System.Net.WebRequestMethods;


[Route("api/Ecommerce/clsLocationAPIs")]
[ApiController]


public class clsLocationAPIs : ControllerBase
{
    private static readonly HttpClient _httpClient = new HttpClient();

    [HttpGet("GetCounties")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<string>?>> GetAllCounties()
    {
         List<string>? counties =await clsLocation.GetAllCountries();
        if(counties == null)
        {
            return StatusCode(500,new DTOGeneralResponse("We could not respond due to server error",500,"NotSet"));
        }

        return Ok(counties);
    }


    [HttpGet("GetCityies/{CountryName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<ActionResult<List<string>?>> GetAllCities(string CountryName)
    {
        int Counter = 0;

        string? CountryCoude = await clsLocation.GetCountryCode(CountryName);

        if (CountryCoude == null)
        {
            return BadRequest(new DTOGeneralResponse("Invalid Country Name", 400, "Serch failer"));
        }



         string baseUrl = clsGlobale.BaseGeoNameUrl();

        try
        {
            // Build request URL
            var url = $@"{baseUrl}?country={(CountryCoude)}&orderby=population& &featureClass=P&maxRows=900&username={clsGlobale.GetgeonamesUserName()}";

            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, new DTOGeneralResponse(
                 "GeoNames API returned an error status.",
                (uint)response.StatusCode,
                 "HttpError"
            ));
            }

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (!root.TryGetProperty("geonames", out var geonamesProp) || geonamesProp.ValueKind != JsonValueKind.Array)
            {
                return StatusCode(500, new DTOGeneralResponse(
                   "Invalid JSON structure from GeoNames API",
                   0,
                  "ParsingError"
     ));


            }

            List<string>? names = new List<string>();
            foreach (var element in geonamesProp.EnumerateArray())
            {
                if (element.TryGetProperty("toponymName", out var toponymElement))
                {
                    var name = toponymElement.GetString();
                    if (!string.IsNullOrEmpty(name)) { names.Add(name); Counter++; }
                }
            }

            return Ok(names);
        }
        catch (HttpRequestException httpEx)
        {
            return StatusCode(500, new DTOGeneralResponse(
                 httpEx.Message,
                  0,
                 "HttpRequestException"
    ));

        }
        catch (JsonException jsonEx)
        {

            return StatusCode(500, new DTOGeneralResponse(
            jsonEx.Message,
            0,
            "JsonException"
          ));
        }
        catch (Exception ex)
        {

            return StatusCode(500, new DTOGeneralResponse(
       ex.Message,
       0,
       "Unknown"
     ));

        }
    }


    [HttpGet("GetPostCodes/{CountryName}/{CityName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<string>?>> GetPostCodes(string CountryName, string CityName)
    {
        CityName = CityName.Replace(" City", "").Trim();
        string? countryCode = await clsLocation.GetCountryCode(CountryName);

        if (string.IsNullOrEmpty(countryCode))
        {
            return BadRequest(new DTOGeneralResponse("Invalid Country Name", 400, "SearchFailure"));
        }

         string baseUrl = clsGlobale.BaseGeoNameUrl();

        try
        {
            var url = $@"{baseUrl}?placename={(CityName)}&country={(countryCode)}&orderby=population&maxRows=100&username={clsGlobale.GetgeonamesUserName()}";

            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, new DTOGeneralResponse(
                    "GeoNames API returned an error status.",
                    (uint)response.StatusCode,
                    "HttpError"
                ));
            }

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (!root.TryGetProperty("postalCodes", out var postalCodesProp) || postalCodesProp.ValueKind != JsonValueKind.Array)
            {
                return StatusCode(500, new DTOGeneralResponse(
                    "Invalid JSON structure from GeoNames API",
                    0,
                    "ParsingError"
                ));
            }

            List<string> postCodes = new List<string>();

            foreach (var element in postalCodesProp.EnumerateArray())
            {
                if (element.TryGetProperty("postalCode", out var postalCodeElement)&& element.TryGetProperty("placeName", out var CityElement))
                {
                    var code = postalCodeElement.GetString();
                    if (!string.IsNullOrEmpty(code)) postCodes.Add(code+"//"+CityElement);
                }
            }

            return Ok(postCodes);
        }
        catch (HttpRequestException httpEx)
        {
            return StatusCode(500, new DTOGeneralResponse(httpEx.Message, 0, "HttpRequestException"));
        }
        catch (JsonException jsonEx)
        {
            return StatusCode(500, new DTOGeneralResponse(jsonEx.Message, 0, "JsonException"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new DTOGeneralResponse(ex.Message, 0, "Unknown"));
        }
    }



}

