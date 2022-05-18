namespace HtmlTagHelpers.Tests {
    public class InlineSvgTagHelperTests {

        //private IConfiguration _configuration;
        //private IMemoryCache _memoryCache;

        //public InlineSvgTagHelperTests(IConfiguration configuration, IMemoryCache memoryCache) {
        //    _configuration = configuration;
        //    _memoryCache = memoryCache;
        //}

        [Fact]
        public void Test01() {
            // Arrange

            // Mock the Configuration.
            var config = new Mock<IConfiguration>();
            config.Object["InlineSvgTagHelper:SvgFilePath"] = "Assets/Icons";
            config.Object["InlineSvgTagHelper:Debug"] = "true";

            // Mock the TagHelperContext
            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            var tagHelperOutput = new TagHelperOutput("inlinesvg", new TagHelperAttributeList(), (result, encoder) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            // Mock the Cache
            var mockCache = new Mock<IMemoryCache>();
            var mockCacheEntry = new Mock<ICacheEntry>();
            string? keyPayload = null;
            mockCache
                .Setup(mc => mc.CreateEntry(It.IsAny<object>()))
                .Callback((object k) => keyPayload = (string)k)
                .Returns(mockCacheEntry.Object);

            object? valuePayload = null;
            mockCacheEntry
                .SetupSet(mce => mce.Value = It.IsAny<object>())
                .Callback<object>(v => valuePayload = v);

            TimeSpan? expirationPayload = null;
            mockCacheEntry
                .SetupSet(mce => mce.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>())
                .Callback<TimeSpan?>(dto => expirationPayload = dto);


            var sut = new InlineSvgTagHelper(config.Object, mockCache.Object) {
                IconName = "something",
                Height = "1em",
                Width = "1em"
            };

            // Act
            sut.ProcessAsync(tagHelperContext, tagHelperOutput);

            // Assert
            Assert.Null(sut);

        }
    }

}
