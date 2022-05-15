using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace HtmlExtensionLabs.TagHelpers
{
    public class EmailTagHelper : TagHelper
    {
        private const string EmailDomain = "contoso.com";

        public string MailTo { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            var address = MailTo + "@" + EmailDomain;
            output.Attributes.SetAttribute("href", "mailto:" + address);
            output.Content.SetContent(address);
        }
    }
}
