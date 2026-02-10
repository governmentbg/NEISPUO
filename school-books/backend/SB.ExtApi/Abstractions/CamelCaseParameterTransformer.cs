namespace SB.ExtApi;

using Microsoft.AspNetCore.Routing;
using SB.Common;

public class CamelCaseParameterTransformer : IOutboundParameterTransformer
{
  public string? TransformOutbound(object? value)
  {
      return value == null ? null : StringUtils.ToCamelCase(value.ToString());
  }
}
