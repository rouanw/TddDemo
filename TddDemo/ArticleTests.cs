using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TddDemo
{
    [TestClass]
    public class ArticleTests
    {
        [TestMethod]
        public void ShouldLowerCaseAllCharactersInItsSlug()
        {
            var article = new Article("SMELlY", "");
            article.Slug.Should().Be("smelly");
        }

        [TestMethod]
        public void ShouldConvertAllSpacesToDashesInItsSlug()
        {
            var article = new Article("Deodorant saves world", "");
            article.Slug.Should().Be("deodorant-saves-world");
        }

        [TestMethod]
        public void ShouldConvertSpecialCharactersInItsSlugToDashes()
        {
            var article = new Article("$melly", "");
            article.Slug.Should().Be("-melly");
        }

        [TestMethod]
        public void ShouldConvertGreaterThanToHtmlInTheBody()
        {
            var article = new Article("", "Sbu >");
            article.EscapedBody.Should().Be("Sbu &gt;");
        }

        [TestMethod]
        public void ShouldConvertLessThanToHtmlInTheBody()
        {
            var article = new Article("", "Clinton <");
            article.EscapedBody.Should().Be("Clinton &lt;");
        }

        [TestMethod]
        public void ShouldSaveACommentToTheDatabaseWhenItIsAdded()
        {
            var article = new Article("", "");
            var comment = new Comment("TDD is cool");
            var database = new Mock<DatabaseService>();

            article.AddComment(database.Object, comment);

            database.Verify(db => db.Save(comment));
        }

        [TestMethod]
        public void ShouldNotSaveACommentWithShitInIt()
        {
            var article = new Article("", "");
            var comment = new Comment("this is shit....");
            var database = new Mock<DatabaseService>();

            article.AddComment(database.Object, comment);

            database.Verify(db => db.Save(comment), Times.Never());

        }
    }

    public class DatabaseService
    {
        public virtual Article Save(Comment comment)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Comment
    {
        public readonly string Text;

        public Comment(string text)
        {
            Text = text;
        }
    }

    public class Article
    {
        private readonly string _title;
        private readonly string _body;

        public Article(string title, string body)
        {
            _title = title;
            _body = body;
        }

        public string Slug
        {
            get
            {
                return Regex.Replace(_title.ToLower(), "[^0-9a-zA-Z]", "-");
            }
        }

        public string EscapedBody
        {
            get
            {
                return _body.Replace(">", "&gt;").Replace("<", "&lt;");
            }
        }

        public void AddComment(DatabaseService db, Comment comment)
        {

            if (!comment.Text.Contains("shit"))
            {
                db.Save(comment);
            }
        }
    }
}
