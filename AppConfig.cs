using DotNetEnv;
using System;

public static class AppConfig
{
    public static string DBConnectionString {get; private set;} = string.Empty;
    public static string EmailPassword {get; private set;} = string.Empty;
    public static string EmailAddress {get; private set;} = string.Empty;
    public static string AllowedOrigin {get; private set;} = string.Empty;

    public static void LoadConfiguration()
    {
        DotNetEnv.Env.Load();

        DBConnectionString = Environment.GetEnvironmentVariable("DBConnectionString") ?? "";
        EmailPassword = Environment.GetEnvironmentVariable("EmailPassword") ?? "";
        EmailAddress = Environment.GetEnvironmentVariable("EmailAddress") ?? "";
        AllowedOrigin = Environment.GetEnvironmentVariable("AllowedOrigin") ?? "";
    }
}
