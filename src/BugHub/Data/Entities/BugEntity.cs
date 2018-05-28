using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugHub.Data.Entities
{
  [Table("Bugs", Schema = "dbo")]
  public class BugEntity
  {
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastModificationDate { get; set; }
  }
}
