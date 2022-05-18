using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Taggarna.HtmlTagHelpers
{
    public class SvgResource
    {
        public string Name { get; set; }
        public HtmlDocument SvgDocument { get; set; }
        public DateTime Created { get; set; }
    }
}
