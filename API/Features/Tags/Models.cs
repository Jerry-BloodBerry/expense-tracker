namespace API.Features.Tags;

public class TagResponse
{
  public int Id { get; set; }
  public required string Name { get; set; }
}

public class CreateTagRequest
{
  public required string Name { get; set; }
}

public class UpdateTagRequest
{
  public required string Name { get; set; }
}