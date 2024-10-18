namespace OnionArchitecture.Application.Options;

public class MsSqlOptions
{
    public const string OptionKey = "MsSqlOptions";
    public string ConnectionString { get; set; } = string.Empty;
}
//public class PostgreOptions
//{
//    public const string OptionKey = "PostgreOptions";

//    public string ConnectionString { get; set; }
//}