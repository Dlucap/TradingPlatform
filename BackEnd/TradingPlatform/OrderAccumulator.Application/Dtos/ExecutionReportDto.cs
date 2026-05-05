namespace OrderAccumulator.Application.Dtos;

public class ExecutionReportDto
{
    public string ExecType { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
  
    /// <summary>
    /// Indica se a ordem foi aceita (New = '0')
    /// </summary>
    public bool IsAccepted => ExecType == "accepted";

}