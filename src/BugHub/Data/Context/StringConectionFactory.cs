namespace BugHub.Data.Context
{
  public static class StringConectionFactory
  {
    public static string Create(SqlConnectionOptions sqlConnectionOptions)
    {
      return sqlConnectionOptions.UseWindowsAuthentication
        ? CreateWithWindowsAuthentication(sqlConnectionOptions)
        : CreateWithUserCredentials(sqlConnectionOptions);
    }

    private static string CreateWithUserCredentials(SqlConnectionOptions sqlConnectionOptions)
    {
      return $"Persist Security Info=False;Data Source={sqlConnectionOptions.DataSource};Initial Catalog={sqlConnectionOptions.InitialCatalog};User ID={sqlConnectionOptions.User};Password={sqlConnectionOptions.Password};Application Name=BugHub";
    }

    private static string CreateWithWindowsAuthentication(SqlConnectionOptions sqlConnectionOptions)
    {
      return $"Persist Security Info=False;Data Source={sqlConnectionOptions.DataSource};Initial Catalog={sqlConnectionOptions.InitialCatalog};Integrated Security=True";
    }
  }
}