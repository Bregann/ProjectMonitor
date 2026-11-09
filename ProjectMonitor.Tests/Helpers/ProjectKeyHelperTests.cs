using ProjectMonitor.Domain.Helpers;

namespace ProjectMonitor.Tests.Helpers
{
    public class ProjectKeyHelperTests
    {
        [TestCase("Orbit", "orbit")]
        [TestCase("Bregan Twitch Bot", "bregan-twitch-bot")]
        [TestCase("  Project Monitor!! ", "project-monitor")]
        [TestCase("C# / .NET thing", "c-net-thing")]
        [TestCase("!!!", "project")]
        public void ToSlug(string name, string expected)
        {
            Assert.That(ProjectKeyHelper.ToSlug(name), Is.EqualTo(expected));
        }

        [Test]
        public void GenerateApiKey_IsLongAndUnique()
        {
            string a = ProjectKeyHelper.GenerateApiKey();
            string b = ProjectKeyHelper.GenerateApiKey();

            Assert.That(a, Has.Length.EqualTo(64));
            Assert.That(a, Is.Not.EqualTo(b));
        }
    }
}
