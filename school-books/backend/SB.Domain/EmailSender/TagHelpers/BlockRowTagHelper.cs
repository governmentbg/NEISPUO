namespace SB.Domain;

using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("block-row")]
public class BlockRowTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        output.PreElement.SetHtmlContent(@"
                <tr>
                    <td style=""padding: 20px 20px 0; font-family: sans-serif; font-size: 15px; line-height: 20px; color: #555555;"">");

        output.PostElement.SetHtmlContent(@"
                    </td>
                </tr>");
    }
}
