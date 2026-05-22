using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Quiz.Models;

public class ApiResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public class Category
{
    [JsonPropertyName("id")]   public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = "";
}

public class GameInfo
{
    [JsonPropertyName("id")]          public int Id { get; set; }
    [JsonPropertyName("key")]         public string Key { get; set; } = "";
    [JsonPropertyName("name")]        public string Name { get; set; } = "";
    [JsonPropertyName("status")]      public string Status { get; set; } = "";
    [JsonPropertyName("category_id")] public int CategoryId { get; set; }
}

public class UserInfo
{
    [JsonPropertyName("id")]         public int Id { get; set; }
    [JsonPropertyName("nickname")]   public string Nickname { get; set; } = "";
    [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; } = "";
}

public class Departure
{
    [JsonPropertyName("id")]         public int Id { get; set; }
    [JsonPropertyName("game_id")]    public int GameId { get; set; }
    [JsonPropertyName("user_id")]    public int UserId { get; set; }
    [JsonPropertyName("score")]      public int Score { get; set; }
    [JsonPropertyName("hits")]       public int Hits { get; set; }
    [JsonPropertyName("total_time")] public int TotalTime { get; set; }
    [JsonPropertyName("user")]       public UserInfo? User { get; set; }
}

public class RoomResponse
{
    [JsonPropertyName("game")]      public GameInfo? Game { get; set; }
    [JsonPropertyName("user")]      public UserInfo? User { get; set; }
    [JsonPropertyName("departure")] public Departure? Departure { get; set; }
}

public class QuestionOption
{
    [JsonPropertyName("id")]          public int Id { get; set; }
    [JsonPropertyName("content")]     public string Content { get; set; } = "";
    [JsonPropertyName("is_correct")]  public bool IsCorrect { get; set; }
    [JsonPropertyName("question_id")] public int QuestionId { get; set; }
}

public class Question
{
    [JsonPropertyName("id")]          public int Id { get; set; }
    [JsonPropertyName("text")]        public string Text { get; set; } = "";
    [JsonPropertyName("media_type")]  public string MediaType { get; set; } = "";
    [JsonPropertyName("category_id")] public int CategoryId { get; set; }
    [JsonPropertyName("options")]     public List<QuestionOption> Options { get; set; } = [];
}

public class AnswerRequest
{
    [JsonPropertyName("departure_id")]  public int DepartureId { get; set; }
    [JsonPropertyName("question_id")]   public int QuestionId { get; set; }
    [JsonPropertyName("answer_id")]     public int AnswerId { get; set; }
    [JsonPropertyName("response_time")] public int ResponseTime { get; set; }
    [JsonPropertyName("game_key")]      public string GameKey { get; set; } = "";
}

public class AnswerDetail
{
    [JsonPropertyName("is_correct")]    public bool IsCorrect { get; set; }
    [JsonPropertyName("response_time")] public int ResponseTime { get; set; }
}

public class AnswerResponse
{
    [JsonPropertyName("detail")]    public AnswerDetail? Detail { get; set; }
    [JsonPropertyName("departure")] public Departure? Departure { get; set; }
}

public class CreateRoomRequest
{
    [JsonPropertyName("name")]        public string Name { get; set; } = "";
    [JsonPropertyName("nickname")]    public string Nickname { get; set; } = "";
    [JsonPropertyName("avatar_url")]  public string AvatarUrl { get; set; } = "";
    [JsonPropertyName("category_id")] public int CategoryId { get; set; }
}

public class JoinRoomRequest
{
    [JsonPropertyName("key")]        public string Key { get; set; } = "";
    [JsonPropertyName("nickname")]   public string Nickname { get; set; } = "";
    [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; } = "";
}