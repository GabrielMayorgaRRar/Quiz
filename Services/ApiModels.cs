using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Quiz.Services;

public class ApiResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public class CreateRoomRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = "";
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = "";
    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }
}

public class JoinRoomRequest
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = "";
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = "";
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = "";
}

public class GameData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("key")]
    public string Key { get; set; } = "";
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }
}

public class UserData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = "";
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = "";
}

public class DepartureData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("game_id")]
    public int GameId { get; set; }
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    [JsonPropertyName("score")]
    public int Score { get; set; }
    
    [JsonPropertyName("user")]
    public UserData? User { get; set; }
}

public class RoomResponseData
{
    [JsonPropertyName("game")]
    public GameData? Game { get; set; }
    [JsonPropertyName("user")]
    public UserData? User { get; set; }
    [JsonPropertyName("departure")]
    public DepartureData? Departure { get; set; }
    
    [JsonPropertyName("players")]
    public List<DepartureData>? Players { get; set; }
}

public class CategoryData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}

public class QuestionData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("text")]
    public string Question { get; set; } = "";

    [JsonPropertyName("options")]
    public List<OptionData> Options { get; set; } = new();
}

public class OptionData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";

    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }
}

public class SubmitAnswerRequest
{
    [JsonPropertyName("departure_id")]
    public int DepartureId { get; set; }

    [JsonPropertyName("question_id")]
    public int QuestionId { get; set; }

    [JsonPropertyName("answer_id")]
    public int AnswerId { get; set; }

    [JsonPropertyName("response_time")]
    public int ResponseTime { get; set; }

    [JsonPropertyName("game_key")]
    public string GameKey { get; set; } = "";
}