using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HtmlExtensionLabs.TagHelpers
{
    public class EmailTagHelper : TagHelper
    {
        private const string EmailDomain = "contoso.com";

        public string MailTo { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;

            var address = MailTo + "@" + EmailDomain;
            output.Attributes.SetAttribute("href", "mailto:" + address);
            output.Content.SetContent(address);
        }
    }
}
