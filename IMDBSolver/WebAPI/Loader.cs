using System.Net;
using System.Net.Http.Json;

namespace WebAPI;

public record Movie(
    string Title,
    string Year,
    string Rated,
    string Released,
    string Runtime,
    string Genre,
    string Director,
    string Writer,
    string Actors,
    string Plot,
    string Language,
    string Country,
    string Awards,
    string Poster,
    object[] Ratings,
    string Metascore,
    string imdbRating,
    string imdbVotes,
    string imdbID,
    string Type,
    string DVD,
    string BoxOffice,
    string Production,
    string Website,
    string Response
);

public class Loader
{
    private readonly string? _api = Environment.GetEnvironmentVariable("API_KEY");
    private readonly HttpClient _client = new HttpClient();

    public async Task<Movie?> GetResponse(string id)
    {
        var response =  await _client.GetAsync(
            $"http://www.omdbapi.com/?apikey={_api}&i={id}&plot=full"
            );

        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        var movie = await response.Content.ReadFromJsonAsync<Movie>();

        return movie;
    }

    public string GetPlot(Movie movie)
    {
        return movie.Plot;
    }

    public string GetImageUrl(Movie movie)
    {
        return movie.Poster;
    }

}
