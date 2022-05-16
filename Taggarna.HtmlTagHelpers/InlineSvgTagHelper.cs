using HtmlAgilityPack;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Xml;
using System.Xml.Linq;

namespace Taggarna.HtmlTagHelpers
{
    public class InlineSvgTagHelper : TagHelper
    {
        //private const string IconStoreFolder = @"D:\iconStore\"; //TODO: Should be set up to fetch from appsettings.
        private const string IconStoreFolder = @"Assets/Icons"; //TODO: Should be set up to fetch from appsettings.
        private const bool Debug = true; //TODO: Should be set up to fetch from appsettings.

        public string IconName { get; set; } = string.Empty;
        public string Height { get; set; } = "1em";
        public string Width { get; set; } = "1em";
        public string Fill { get; set; } = "currentColor";
        public string? Style { get; set; } = null;
        public string? Class { get; set; } = null;

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var iconFilePath = Path.Combine(AppContext.BaseDirectory, IconStoreFolder, IconName + ".svg");
            var doc = new HtmlDocument();

            try
            {
                // If there's no IconName, throw an exception.
                if (string.IsNullOrEmpty(IconName))
                {
                    throw new ArgumentNullException("IconName"); //TODO: There's probably a better exception for this, since the missing 'IconName' parameter isn't really an argument of the specific method, but missing in the TagHelper.
                }

                // Prevent traversing outside of the defined Icon Store.
                if (IconName.Contains(".."))
                {
                    throw new ArgumentOutOfRangeException(); //TODO: There's probably a better exception for this, too.
                }

                doc.Load(iconFilePath);

                // Validate the svg file.
                if (!IsValidSvg(doc.ParsedText))
                {
                    throw new XmlException($"The loaded file is not a valid svg file. (File '{IconName}')");
                }

                var svgNode = doc.DocumentNode.SelectSingleNode("//svg");
                var svgContent = svgNode.WriteContentTo();

                foreach (var itemAttribute in svgNode.Attributes)
                {
                    output.Attributes.SetAttribute(itemAttribute.Name, itemAttribute.Value);
                }

                output.Attributes.SetAttribute("fill", Fill);
                output.Content.SetHtmlContent(svgContent);

            }
            catch (ArgumentNullException e)
            {
                Height = 0.ToString();
                Width = 0.ToString();
                if (Debug)
                {
                    output.Attributes.SetAttribute("Exception", e.Message);
                }
            }
            catch (FileNotFoundException e)
            {
                Height = 0.ToString();
                Width = 0.ToString();
                if (Debug)
                {
                    output.Attributes.SetAttribute("Exception", $"File not found. (Filename '{IconName}')");
                }
            }
            catch (XmlException e)
            {
                Height = 0.ToString();
                Width = 0.ToString();
                if (Debug)
                {
                    output.Attributes.SetAttribute("Exception", e.Message);
                }
            }
            catch (Exception e)
            {
                Height = 0.ToString();
                Width = 0.ToString();
                if (Debug)
                {
                    output.Attributes.SetAttribute("Exception", e.Message);
                }
            }
            finally
            {
                output.TagName = "svg";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.SetAttribute("height", Height);
                output.Attributes.SetAttribute("width", Width);
                if (Style is not null)
                {
                    output.Attributes.SetAttribute("style", Style);
                }
                if (Class is not null)
                {
                    output.Attributes.SetAttribute("class", Class);
                }
            }
            return base.ProcessAsync(context, output);
        }

        private static bool IsValidSvg(string str)
        {
            try
            {
                var svg = XDocument.Load(new StringReader(str));
                return svg.Root != null && svg.Root.Name.LocalName.Equals("svg");
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
