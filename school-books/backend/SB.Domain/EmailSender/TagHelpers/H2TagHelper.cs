namespace SB.Domain;

using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("h2")]
public class H2TagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("style", "margin: 0 0 10px 0; font-family: sans-serif; font-size: 18px; line-height: 22px; color: #333333; font-weight: bold;");
    }
}
