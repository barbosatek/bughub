﻿using System.Web.Http;

namespace BugHub.WebApi
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      config.MapHttpAttributeRoutes();
    }
  }
}
