using System;

namespace BugHub.WebApi.Models.V1
{
  public class Bug
  {
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
  }
}