namespace SB.Domain;

using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("p")]
public class PTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("style", "margin: 0;");
    }
}
