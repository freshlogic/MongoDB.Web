using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Web.Internal;
using MongoDB.Web.Providers;
using Moq;

namespace MongoDB.Web.Tests.MongoDBMembershipProviderTests
{
    [TestClass]
    public class GetUserXTests
    {
        [TestMethod]
        public void When_I_get_a_user_by_name_but_they_are_not_online()
        {
            var collection = new Mock<IMongoCollection>(MockBehavior.Strict);
            var provider = new Mock<IMongoConnectionProvider>(MockBehavior.Strict);

            provider.Setup(m => m.GetCollection("mongodb://localhost", "ASPNETDB", "Users")).Returns(() => collection.Object);
            collection.Setup(c => c.EnsureIndex(It.IsAny<string[]>()));

            var membershipProvider = new MongoDBMembershipProvider(provider.Object);
            MembershipUtilities.SetDefaultMembershipProvider(membershipProvider);

            string query = null;

            var providerKey = Guid.NewGuid();

            var userDocument = new BsonDocument((IDictionary<string, object>)
                new Dictionary<string, object>
                    {
                        { "Username", "bob" },
                        { "_id", providerKey },
                        { "Email", "bob@example.com" },
                        { "PasswordQuestion", "MyQuestion" },
                        { "Comment", "MyComment" },
                        { "IsApproved", true },
                        { "IsLockedOut", false },
                        { "CreationDate", new DateTime(2000, 1, 1) },
                        { "LastLoginDate", new DateTime(2000, 1, 2) },
                        { "LastActivityDate", new DateTime(2000, 1, 3) },
                        { "LastPasswordChangedDate", new DateTime(2000, 1, 4) },
                        { "LastLockoutDate", new DateTime(2000, 1, 5) },
                    });

            collection.Setup(c => c.FindOneAs<BsonDocument>(It.IsAny<IMongoQuery>()))
                .Callback((IMongoQuery q) => query = q.ToJson())
                .Returns(userDocument);

            var user = membershipProvider.GetUser("bob", false);

            Assert.AreEqual("{ \"Username\" : \"bob\" }", query);

            Assert.AreEqual("bob", user.UserName);
            Assert.AreEqual(providerKey, (Guid)user.ProviderUserKey);
            Assert.AreEqual("bob@example.com", user.Email);
            Assert.AreEqual("MyQuestion", user.PasswordQuestion);
            Assert.AreEqual("MyComment", user.Comment);
            Assert.AreEqual(true, user.IsApproved);
            Assert.AreEqual(false, user.IsLockedOut);
            Assert.AreEqual(new DateTime(2000, 1, 1), user.CreationDate);
            Assert.AreEqual(new DateTime(2000, 1, 2), user.LastLoginDate);
            Assert.AreEqual(new DateTime(2000, 1, 3), user.LastActivityDate);
            Assert.AreEqual(new DateTime(2000, 1, 4), user.LastPasswordChangedDate);
            Assert.AreEqual(new DateTime(2000, 1, 5), user.LastLockoutDate);
        }

        [TestMethod]
        public void When_I_get_a_user_by_name_and_they_are_online()
        {
            var collection = new Mock<IMongoCollection>(MockBehavior.Strict);
            var provider = new Mock<IMongoConnectionProvider>(MockBehavior.Strict);

            provider.Setup(m => m.GetCollection("mongodb://localhost", "ASPNETDB", "Users")).Returns(() => collection.Object);
            collection.Setup(c => c.EnsureIndex(It.IsAny<string[]>()));

            var membershipProvider = new MongoDBMembershipProvider(provider.Object);
            MembershipUtilities.SetDefaultMembershipProvider(membershipProvider);

            BsonDocument query = null;
            BsonDocument update = null;

            var providerKey = Guid.NewGuid();

            var userDocument = new BsonDocument((IDictionary<string, object>)
                new Dictionary<string, object>
                    {
                        { "Username", "bob" },
                        { "_id", providerKey },
                        { "Email", "bob@example.com" },
                        { "PasswordQuestion", "MyQuestion" },
                        { "Comment", "MyComment" },
                        { "IsApproved", true },
                        { "IsLockedOut", false },
                        { "CreationDate", new DateTime(2000, 1, 1) },
                        { "LastLoginDate", new DateTime(2000, 1, 2) },
                        { "LastActivityDate", new DateTime(2000, 1, 3) },
                        { "LastPasswordChangedDate", new DateTime(2000, 1, 4) },
                        { "LastLockoutDate", new DateTime(2000, 1, 5) },
                    });

            collection.Setup(c => c.FindOneAs<BsonDocument>(It.IsAny<IMongoQuery>())).Returns(userDocument);

            collection.Setup(c => c.Update(It.IsAny<IMongoQuery>(), It.IsAny<IMongoUpdate>()))
                .Callback((IMongoQuery q, IMongoUpdate u) =>
                {
                    query = q.ToBsonDocument();
                    update = u.ToBsonDocument();
                })
                .Returns(new SafeModeResult());

            var user = membershipProvider.GetUser("bob", true);

            Assert.AreEqual("{ \"Username\" : \"bob\" }", query.ToString());
            var lastActivityDateOffset = DateTime.UtcNow - update["$set"].AsBsonDocument["LastActivityDate"].AsDateTime;
            Assert.AreEqual(0, lastActivityDateOffset.TotalSeconds, 1);
        }

        [TestMethod]
        public void When_I_get_a_nonexistant_user_by_name_and_they_are_online()
        {
            var collection = new Mock<IMongoCollection>(MockBehavior.Strict);
            var provider = new Mock<IMongoConnectionProvider>(MockBehavior.Strict);

            provider.Setup(m => m.GetCollection("mongodb://localhost", "ASPNETDB", "Users")).Returns(() => collection.Object);
            collection.Setup(c => c.EnsureIndex(It.IsAny<string[]>()));

            var membershipProvider = new MongoDBMembershipProvider(provider.Object);
            MembershipUtilities.SetDefaultMembershipProvider(membershipProvider);

            collection.Setup(c => c.FindOneAs<BsonDocument>(It.IsAny<IMongoQuery>())).Returns(() => null);

            var user = membershipProvider.GetUser("bob", true);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void When_I_get_a_user_by_id_but_they_are_not_online()
        {
            var collection = new Mock<IMongoCollection>(MockBehavior.Strict);
            var provider = new Mock<IMongoConnectionProvider>(MockBehavior.Strict);

            provider.Setup(m => m.GetCollection("mongodb://localhost", "ASPNETDB", "Users")).Returns(() => collection.Object);
            collection.Setup(c => c.EnsureIndex(It.IsAny<string[]>()));

            var membershipProvider = new MongoDBMembershipProvider(provider.Object);
            MembershipUtilities.SetDefaultMembershipProvider(membershipProvider);

            string query = null;

            var providerKey = Guid.NewGuid();

            var userDocument = new BsonDocument((IDictionary<string, object>)
                new Dictionary<string, object>
                    {
                        { "Username", "bob" },
                        { "_id", providerKey },
                        { "Email", "bob@example.com" },
                        { "PasswordQuestion", "MyQuestion" },
                        { "Comment", "MyComment" },
                        { "IsApproved", true },
                        { "IsLockedOut", false },
                        { "CreationDate", new DateTime(2000, 1, 1) },
                        { "LastLoginDate", new DateTime(2000, 1, 2) },
                        { "LastActivityDate", new DateTime(2000, 1, 3) },
                        { "LastPasswordChangedDate", new DateTime(2000, 1, 4) },
                        { "LastLockoutDate", new DateTime(2000, 1, 5) },
                    });

            collection.Setup(c => c.FindOneAs<BsonDocument>(It.IsAny<IMongoQuery>()))
                .Callback((IMongoQuery q) => query = q.ToJson())
                .Returns(userDocument);

            var user = membershipProvider.GetUser(providerKey, false);

            Assert.AreEqual(string.Format("{{ \"_id\" : CSUUID(\"{0}\") }}", providerKey), query.ToString());

            Assert.AreEqual("bob", user.UserName);
            Assert.AreEqual(providerKey, (Guid)user.ProviderUserKey);
            Assert.AreEqual("bob@example.com", user.Email);
            Assert.AreEqual("MyQuestion", user.PasswordQuestion);
            Assert.AreEqual("MyComment", user.Comment);
            Assert.AreEqual(true, user.IsApproved);
            Assert.AreEqual(false, user.IsLockedOut);
            Assert.AreEqual(new DateTime(2000, 1, 1), user.CreationDate);
            Assert.AreEqual(new DateTime(2000, 1, 2), user.LastLoginDate);
            Assert.AreEqual(new DateTime(2000, 1, 3), user.LastActivityDate);
            Assert.AreEqual(new DateTime(2000, 1, 4), user.LastPasswordChangedDate);
            Assert.AreEqual(new DateTime(2000, 1, 5), user.LastLockoutDate);
        }

        [TestMethod]
        public void When_I_get_a_user_by_id_and_they_are_online()
        {
            var collection = new Mock<IMongoCollection>(MockBehavior.Strict);
            var provider = new Mock<IMongoConnectionProvider>(MockBehavior.Strict);

            provider.Setup(m => m.GetCollection("mongodb://localhost", "ASPNETDB", "Users")).Returns(() => collection.Object);
            collection.Setup(c => c.EnsureIndex(It.IsAny<string[]>()));

            var membershipProvider = new MongoDBMembershipProvider(provider.Object);
            MembershipUtilities.SetDefaultMembershipProvider(membershipProvider);

            BsonDocument query = null;
            BsonDocument update = null;

            var providerKey = Guid.NewGuid();

            var userDocument = new BsonDocument((IDictionary<string, object>)
                new Dictionary<string, object>
                    {
                        { "Username", "bob" },
                        { "_id", providerKey },
                        { "Email", "bob@example.com" },
                        { "PasswordQuestion", "MyQuestion" },
                        { "Comment", "MyComment" },
                        { "IsApproved", true },
                        { "IsLockedOut", false },
                        { "CreationDate", new DateTime(2000, 1, 1) },
                        { "LastLoginDate", new DateTime(2000, 1, 2) },
                        { "LastActivityDate", new DateTime(2000, 1, 3) },
                        { "LastPasswordChangedDate", new DateTime(2000, 1, 4) },
                        { "LastLockoutDate", new DateTime(2000, 1, 5) },
                    });

            collection.Setup(c => c.FindOneAs<BsonDocument>(It.IsAny<IMongoQuery>())).Returns(userDocument);

            collection.Setup(c => c.Update(It.IsAny<IMongoQuery>(), It.IsAny<IMongoUpdate>()))
                .Callback((IMongoQuery q, IMongoUpdate u) =>
                {
                    query = q.ToBsonDocument();
                    update = u.ToBsonDocument();
                })
                .Returns(new SafeModeResult());

            var user = membershipProvider.GetUser(providerKey, true);

            Assert.AreEqual(string.Format("{{ \"_id\" : CSUUID(\"{0}\") }}", providerKey), query.ToString());
            var lastActivityDateOffset = DateTime.UtcNow - update["$set"].AsBsonDocument["LastActivityDate"].AsDateTime;
            Assert.AreEqual(0, lastActivityDateOffset.TotalSeconds, 1);
        }

        [TestMethod]
        public void When_I_get_a_nonexistant_user_by_id_and_they_are_online()
        {
            var collection = new Mock<IMongoCollection>(MockBehavior.Strict);
            var provider = new Mock<IMongoConnectionProvider>(MockBehavior.Strict);

            provider.Setup(m => m.GetCollection("mongodb://localhost", "ASPNETDB", "Users")).Returns(() => collection.Object);
            collection.Setup(c => c.EnsureIndex(It.IsAny<string[]>()));

            var membershipProvider = new MongoDBMembershipProvider(provider.Object);
            MembershipUtilities.SetDefaultMembershipProvider(membershipProvider);

            var providerKey = Guid.NewGuid();

            collection.Setup(c => c.FindOneAs<BsonDocument>(It.IsAny<IMongoQuery>())).Returns(() => null);

            var user = membershipProvider.GetUser(providerKey, true);

            Assert.IsNull(user);
        }
    }
}
