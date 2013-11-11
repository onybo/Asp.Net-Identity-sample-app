using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoUser.Identity.MongoDb;
using MongoUser.Models;
using MongoDB.Driver;
using System.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver.Builders;

namespace MongoUser.Tests
{
    [TestClass]
    public class StoreTests
    {
        private MongoDatabase CreateDatabase()
        {
            var client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString);
            var server = client.GetServer();
            return server.GetDatabase("IdentityDatabase");
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            var database = CreateDatabase();
            
            var store = new UserStore<ApplicationUser>(database);
            var user = new ApplicationUser() { UserName = "TestUser" };
            await store.CreateAsync(user);
            Assert.IsTrue(user.Id.Length > 0);
            var userFromDb = await store.FindByIdAsync(user.Id);
            
            Assert.IsNotNull(userFromDb);
            Assert.AreEqual("TestUser", userFromDb.UserName);

            database.GetCollection<ApplicationUser>("users").Remove(Query<ApplicationUser>.EQ(u => u.Id, user.Id));
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            var database = CreateDatabase();

            var store = new UserStore<ApplicationUser>(database);
            var user = new ApplicationUser() { UserName = "TestUser" };
            await store.CreateAsync(user);
            await store.DeleteAsync(user);
            Assert.IsTrue(user.Id.Length > 0);
            var userFromDb = await store.FindByIdAsync(user.Id);

            Assert.IsNull(userFromDb);
        }

        [TestMethod]
        public async Task FindByIdAsyncTest()
        {
            var database = CreateDatabase();

            var store = new UserStore<ApplicationUser>(database);
            var user = new ApplicationUser() { UserName = "TestUser" };
            await store.CreateAsync(user);
            var userFromDb = await store.FindByIdAsync(user.Id);
            Assert.IsTrue(userFromDb.Id.Length > 0);
            Assert.AreEqual("TestUser", userFromDb.UserName);
            await store.DeleteAsync(user);
        }

        [TestMethod]
        public async Task FindByNameAsyncAsyncTest()
        {
            var database = CreateDatabase();

            var store = new UserStore<ApplicationUser>(database);
            var user = new ApplicationUser() { UserName = "TestUser" };
            await store.CreateAsync(user);
            var userFromDb = await store.FindByNameAsync("testuser");
            Assert.IsTrue(userFromDb.Id.Length > 0);
            Assert.AreEqual(user.Id, userFromDb.Id);
            await store.DeleteAsync(user);
        }

        [TestMethod]
        public async Task UpdateAsyncAsyncTest()
        {
            var database = CreateDatabase();

            var store = new UserStore<ApplicationUser>(database);
            var user = new ApplicationUser() { UserName = "TestUser" };
            await store.CreateAsync(user);
            var id = user.Id;
            Assert.IsTrue(id.Length > 0);

            user.PasswordHash = "testhash";
            user.UserName = "modified user name";

            await store.UpdateAsync(user);
            Assert.AreEqual(id, user.Id);

            var updatedUser = await store.FindByIdAsync(id);
            Assert.AreEqual("testhash", updatedUser.PasswordHash);
            Assert.AreEqual("modified user name", updatedUser.UserName);
            await store.DeleteAsync(user);
        }
    }
}
