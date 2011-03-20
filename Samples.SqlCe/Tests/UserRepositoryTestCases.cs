using NHibernate;
using NUnit.Framework;
using Samples.SqlCe.DomainObjects;
using Samples.SqlCe.Repositories;

namespace Samples.SqlCe.Tests {

    [TestFixture]
    public class UserRepositoryTestCases : EmbeddedTestBase {
        private ISession session;
        private UserRepository userRepository;
        private User user;

        private string firstname = "TestFN";
        private string lastname = "TestLN";
        private string email = "TestEmail";
        private string username = "testusername";

        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            FixtureInitalize(typeof(User).Assembly);

            session = CreateSession();
            userRepository = new UserRepository();
            userRepository.session = session;
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown() {
            session.Dispose();
        }

        [SetUp]
        public void SetUp() {
            user = new User();
            user.Firstname = firstname;
            user.Lastname = lastname;
            user.Email = email;
            user.Username = username;

            // Persist user to DB for testing
            session.Save(user);
            session.Flush();
            session.Evict(user);
        }

        [TearDown]
        public void TearDown() {
            user = null;
        }

        [Test]
        public void CanSaveAndLoadUser() {
            User loaded = session.Load<User>(user.Id);
            Assert.AreEqual(user.Firstname, loaded.Firstname, "Firstname does not match");
            Assert.AreEqual(user.Lastname, loaded.Lastname, "Lastname does not match");
            Assert.AreEqual(user.Email, loaded.Email, "Email does not match");
        }

        [Test]
        public void MakeSureUsernameIsAvailable() {
            Assert.IsTrue(userRepository.CheckUsernameIsAvailable(username));
        }

    }
}