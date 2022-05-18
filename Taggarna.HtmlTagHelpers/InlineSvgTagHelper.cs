using System.Diagnostics;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Taggarna.HtmlTagHelpers {
    public class InlineSvgTagHelper : TagHelper {

        public string IconName { get; set; } = string.Empty;
        public string? Height { get; set; }
        public string? Width { get; set; }
        public string Fill { get; set; } = "currentColor";
        public string? Style { get; set; } = null;
        public string? Class { get; set; } = null;

        private IMemoryCache _memoryCache;
        private IConfiguration _configuration;

        private string _iconStoreFolder; // = @"Assets/Icons"; //TODO: Should be set up to fetch from appsettings.
        private bool _debug; // = true; //TODO: Should be set up to fetch from appsettings.


        public InlineSvgTagHelper(IConfiguration configuration, IMemoryCache memoryCache) {
            _configuration = configuration;
            _memoryCache = memoryCache;
            _iconStoreFolder = _configuration["InlineSvgTagHelper:SvgFilePath"];
            _debug = Convert.ToBoolean(_configuration["InlineSvgTagHelper:Debug"]);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            try {
                var timer = new Stopwatch();
                timer.Start();

                // If there's no IconName input, throw an exception.
                if (string.IsNullOrEmpty(IconName)) {
                    throw new ArgumentNullException("IconName"); //TODO: There's probably a better exception for this, since the missing 'IconName' parameter isn't really an argument of the specific method, but missing in the TagHelper.
                }
                // If trying to traverse outside of the defined Icon Store, throw an exception.
                if (IconName.Contains("..")) {
                    throw new ArgumentOutOfRangeException(); //TODO: There's probably a better exception for this, but this works for now.
                }

                // If there is no svg store cached at all, create one.
                if (_memoryCache.Get<List<SvgResource>>("InlineSvgTagHelper") is null) {
                    _memoryCache.Set("InlineSvgTagHelper", new List<SvgResource>());
                }

                // If the requested svg is already loaded into the cache, use that. Otherwise, load it from file.
                var storedSvg = _memoryCache.Get<List<SvgResource>>("InlineSvgTagHelper").FirstOrDefault(x => x.Name == IconName);
                HtmlDocument svgDoc;
                SvgSource source;
                if (storedSvg == null) {
                    // Get the svg from file.
                    svgDoc = CreateHtmlDocument(IconName);
                    // Store the loaded svg into the cache for future use.
                    _memoryCache.Get<List<SvgResource>>("InlineSvgTagHelper").Add(new SvgResource { Name = IconName, SvgDocument = svgDoc, Created = DateTime.UtcNow });
                    source = SvgSource.File;
                } else {
                    // Get the svg from cache.
                    svgDoc = storedSvg.SvgDocument;
                    source = SvgSource.Cache;
                }

                var svgRoot = svgDoc.DocumentNode.SelectSingleNode("//svg");
                var svgContent = svgRoot.WriteContentTo();
                foreach (var itemAttribute in svgRoot.Attributes) {
                    output.Attributes.SetAttribute(itemAttribute.Name, itemAttribute.Value);
                }
                output.Content.SetHtmlContent(svgContent);

                timer.Stop();
                Console.Write($"    Loaded file '");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(IconName);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"' from ");
                if (source == SvgSource.File) {
                    Console.ForegroundColor = ConsoleColor.Red;
                } else {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write(source);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" in ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{timer.Elapsed.Milliseconds}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"ms (");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{timer.Elapsed.Ticks}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"ticks).");
                Console.WriteLine();

            } catch (FileNotFoundException e) {
                Height = 0.ToString();
                Width = 0.ToString();
                if (_debug) {
                    output.Attributes.SetAttribute("Exception", $"File not found. (Filename '{IconName}')");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(IconName);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" : {e.Message}");
                Console.WriteLine();
            } catch (Exception e) {
                Height = 0.ToString();
                Width = 0.ToString();
                if (_debug) {
                    output.Attributes.SetAttribute("Exception", e.Message);
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(IconName);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" : {e.Message}");
                Console.WriteLine();
            } finally {
                output.TagName = "svg";
                output.TagMode = TagMode.StartTagAndEndTag;
                if (Height is not null) {
                    output.Attributes.SetAttribute("height", Height);
                }
                if (Width is not null) {
                    output.Attributes.SetAttribute("width", Width);
                }
                if (Style is not null) {
                    output.Attributes.SetAttribute("style", Style);
                }
                if (Class is not null) {
                    output.Attributes.SetAttribute("class", Class);
                }
            }
            return base.ProcessAsync(context, output);
        }

        private HtmlDocument CreateHtmlDocument(string iconName) {
            var iconFilePath = Path.Combine(AppContext.BaseDirectory, _iconStoreFolder, iconName + ".svg");
            var doc = new HtmlDocument();

            try {
                doc.Load(iconFilePath);

                // Validate the svg file.
                if (!IsValidSvg(doc.ParsedText)) {
                    throw new XmlException($"The loaded file is not a valid svg file. (File '{iconName}')");
                }
            } catch (Exception e) {
                //Console.WriteLine(e);
                throw;
            } finally {

            }
            return doc;

        }
        private bool IsValidSvg(string str) {
            try {
                var svg = XDocument.Load(new StringReader(str));
                return svg.Root != null && svg.Root.Name.LocalName.Equals("svg");
            } catch (Exception e) {
                return false;
            }
        }

        private enum SvgSource {
            File = 1,
            Cache = 2
        }
    }
}
