using System.Text.Json.Serialization;
namespace SpiderEngine.Test.Model;

internal class ZhlgdGrade
{
    [JsonPropertyName("KCCJ")]
    [JsonInclude]
    private string GradeStr { get; set; } = null!;

    [JsonPropertyName("KCMC")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("DWMC")]
    public string SchoolName { get; set; } = null!;

    [JsonPropertyName("XH")]
    public string Sno { get; set; } = null!;

    [JsonPropertyName("CJLX")]
    public string GradeType { get; set; } = null!;

    [JsonPropertyName("XN")]
    public string GradeYear { get; set; } = null!;

    [JsonPropertyName("XQM")]
    public string GradeTerm { get; set; } = null!;

    public double Grade => double.Parse(GradeStr);
}
