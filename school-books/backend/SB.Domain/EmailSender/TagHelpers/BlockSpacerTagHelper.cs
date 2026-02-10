namespace SB.Domain;

using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("block-spacer", TagStructure = TagStructure.WithoutEndTag)]
public class BlockSpacerTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        output.Content.SetHtmlContent($@"
                <!-- Clear Spacer : BEGIN -->
                <tr>
                    <td aria-hidden=""true"" height=""40"" style=""font-size: 0px; line-height: 0px;"">
                        &nbsp;
                    </td>
                </tr>
                <!-- Clear Spacer : END -->
            ");
    }
}
