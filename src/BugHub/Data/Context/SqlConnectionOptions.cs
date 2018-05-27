namespace BugHub.Data.Context
{
  public class SqlConnectionOptions
  {
    public string DataSource { get; set; }
    public string InitialCatalog { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public bool UseWindowsAuthentication { get; set; }
  }
}