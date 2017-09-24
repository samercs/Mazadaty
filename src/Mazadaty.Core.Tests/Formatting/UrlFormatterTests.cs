using Mazadaty.Core.Formatting;
using NUnit.Framework;
using System;

namespace Mazadaty.Core.Tests.Formatting
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable PossibleNullReferenceException

    [TestFixture]
    public class UrlFormatterTests
    {
        [Test]
        public void SetCdnHost_WithUri_AddsCdnHost()
        {
            var uri = new Uri("https://Mazadaty.blob.core.windows.net/avatars/image.jpg");
            var cdnUrl = UrlFormatter.GetCdnUrl(uri);

            Assert.AreEqual("https://az723232.vo.msecnd.net/avatars/image.jpg", cdnUrl);
        }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}
