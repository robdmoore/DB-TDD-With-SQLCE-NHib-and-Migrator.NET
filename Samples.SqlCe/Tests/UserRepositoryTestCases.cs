using NHibernate;
using NUnit.Framework;
using Samples.SqlCe.DomainObjects;
using Samples.SqlCe.Migrations;
using Samples.SqlCe.Repositories;

namespace Samples.SqlCe.Tests {

    [TestFixture]
    public class UserRepositoryTestCases : EmbeddedTestBase
    {
        private ISession _session;
        private UserRepository _userRepository;
        private User _user;

        private const string Firstname = "TestFN";
        private const string Lastname = "TestLN";
        private const string Email = "TestEmail";
        private const string Username = "TestUserName";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Initialize("DomainObjects", typeof(CreateUserTable).Assembly, typeof(User).Assembly);

            _session = CreateSession();
            _userRepository = new UserRepository {session = _session};
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _session.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            _user = new User {FirstName = Firstname, LastName = Lastname, EmailAddress = Email, Username = Username};

            // Persist user to DB for testing
            _session.Save(_user);
            _session.Flush();
            _session.Evict(_user);
        }

        [TearDown]
        public void TearDown()
        {
            _user = null;
        }

        [Test]
        public void Can_save_and_load_user()
        {
            var loaded = _session.Load<User>(_user.Id);
            Assert.AreEqual(_user.Username, loaded.Username, "Username does not match");
            Assert.AreEqual(_user.FirstName, loaded.FirstName, "FirstName does not match");
            Assert.AreEqual(_user.LastName, loaded.LastName, "LastName does not match");
            Assert.AreEqual(_user.EmailAddress, loaded.EmailAddress, "EmailAddress does not match");
        }

        [Test]
        public void Make_sure_username_is_available()
        {
            Assert.IsTrue(_userRepository.CheckUsernameIsAvailable(Username));
        }

    }
}