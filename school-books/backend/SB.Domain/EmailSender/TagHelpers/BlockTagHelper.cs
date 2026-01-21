namespace SB.Domain;

using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("block")]
public class BlockTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        output.PreElement.SetHtmlContent(@"
                <tr>
                    <td style=""background-color: #ffffff; padding-bottom: 20px"">
                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">");

        output.PostElement.SetHtmlContent(@"
                        </table>
                    </td>
                </tr>");
    }
}
