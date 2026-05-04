using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Quiz.Services;

public class QuizApiService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "http://localhost:4100/api/v1";

    public QuizApiService()
    {
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl + "/") };
    }

    public async Task<List<CategoryData>> GetCategoriesAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<List<CategoryData>>>("categories");
            return response?.Data ?? new List<CategoryData>();
        }
        catch { return new List<CategoryData>(); }
    }

    public async Task<RoomResponseData?> CreateRoomAsync(CreateRoomRequest request)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("room", request);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadFromJsonAsync<ApiResponse<RoomResponseData>>();
                return content?.Data;
            }
        }
        catch
        {

        }
        return null;
    }

    public async Task<RoomResponseData?> JoinRoomAsync(JoinRoomRequest request)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("room/join", request);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadFromJsonAsync<ApiResponse<RoomResponseData>>();
                return content?.Data;
            }
        }
        catch { }
        return null;
    }

    public async Task<bool> StartGameAsync(int gameId)
    {
        try
        {
            var res = await _http.PostAsync($"room/{gameId}/start", null);
            return res.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<QuestionData?> GetQuestionAsync(int gameId)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<QuestionData>>($"game/{gameId}/question");
            return response?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
}
