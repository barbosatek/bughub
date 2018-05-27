using System;

namespace BugHub.Data.Entities
{
  public class BugEntity
  {
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastModificationDate { get; set; }
  }
}
