namespace HtmlTagHelpers.Tests {
    public class InlineSvgTagHelperTests {

        [Fact]
        public void Test01() {
            // Arrange

            // Mock the Configuration.
            var config = new Mock<IConfiguration>
            {
                Object =
                {
                    ["InlineSvgTagHelper:SvgFilePath"] = "Assets/Icons",
                    ["InlineSvgTagHelper:Debug"] = "true"
                }
            };

            // Mock the TagHelperContext
            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var tagHelperOutput = new TagHelperOutput("inlinesvg", new TagHelperAttributeList(), (_, _) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            // Mock the Cache
            var mockCache = new Mock<IMemoryCache>();
            var mockCacheEntry = new Mock<ICacheEntry>();

            mockCache
                .Setup(mc => mc.CreateEntry(It.IsAny<object>()))
                .Callback((object k) => _ = (string)k)
                .Returns(mockCacheEntry.Object);

            mockCacheEntry
                .SetupSet(mce => mce.Value = It.IsAny<object>())
                .Callback<object>(v => _ = v);

            mockCacheEntry
                .SetupSet(mce => mce.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>())
                .Callback<TimeSpan?>(dto => _ = dto);

            var sut = new InlineSvgTagHelper(config.Object, mockCache.Object) {
                IconName = "something",
                Height = "1em",
                Width = "1em"
            };

            // Act
            sut.ProcessAsync(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.NotNull(sut);

        }
    }

}
