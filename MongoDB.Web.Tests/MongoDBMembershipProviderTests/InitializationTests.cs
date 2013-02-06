using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Web.Internal;
using MongoDB.Web.Providers;
using Moq;

namespace MongoDB.Web.Tests.MongoDBMembershipProviderTests
{
    [TestClass]
    public class InitializationTests
    {
        [TestMethod]
        public void When_I_create_with_no_configuration_options()
        {
            var collection = new Mock<IMongoCollection>(MockBehavior.Strict);
            var provider = new Mock<IMongoConnectionProvider>(MockBehavior.Strict);

            provider.Setup(m => m.GetCollection("mongodb://localhost", "ASPNETDB", "Users")).Returns(() => collection.Object);

            collection.Setup(c => c.EnsureIndex(It.IsAny<string[]>())).Callback((string[] keys) => { Assert.Fail("Tried to create unknown key: {0}", string.Join(", ", keys)); });
            collection.Setup(c => c.EnsureIndex("ApplicationName"));
            collection.Setup(c => c.EnsureIndex("ApplicationName", "Email"));
            collection.Setup(c => c.EnsureIndex("ApplicationName", "Username"));

            var membershipProvider = new MongoDBMembershipProvider(provider.Object);
            var nvc = new NameValueCollection();
            MembershipUtilities.SetDefaultMembershipProvider(membershipProvider, nvc);

            // It should not modify the configuration options
            CollectionAssert.AreEquivalent(new object[0], nvc);

            // It should only open the collection once
            provider.Verify(m => m.GetCollection("mongodb://localhost", "ASPNETDB", "Users"), Times.Once());

            // It should create all the needed indexes
            collection.Verify(c => c.EnsureIndex("ApplicationName"), Times.Once());
            collection.Verify(c => c.EnsureIndex("ApplicationName", "Email"), Times.Once());
            collection.Verify(c => c.EnsureIndex("ApplicationName", "Username"), Times.Once());
        }

        [TestMethod]
        public void When_I_create_with_a_bad_set_of_password_options_it_throws()
        {
            var provider = new Mock<IMongoConnectionProvider>();

            var membershipProvider = new MongoDBMembershipProvider(provider.Object);
            var nvc = new NameValueCollection
                          {
                              {"enablePasswordRetrieval", "true"},
                              {"passwordFormat", "Hashed"},
                          };
            try {
                MembershipUtilities.SetDefaultMembershipProvider(membershipProvider, nvc);
                Assert.Fail("Did not throw the expected exception.");
            } catch (ProviderException exception) {
                Assert.AreEqual("Configured settings are invalid: Hashed passwords cannot be retrieved. Either set the password format to different type, or set enablePasswordRetrieval to false.", exception.Message);
            }
        }

        [TestMethod]
        public void When_I_provide_alternate_connection_options()
        {
            var provider = new Mock<IMongoConnectionProvider>(MockBehavior.Strict);
            provider.Setup(m => m.GetCollection("mongodb://alternate/?slaveOk=true", "TESTDB", "MyUsers"))
                .Returns(() => new Mock<IMongoCollection>().Object);

            var membershipProvider = new MongoDBMembershipProvider(provider.Object);
            var nvc = new NameValueCollection
                          {
                              {"connectionString", "mongodb://alternate/?slaveOk=true"},
                              {"database", "TESTDB"},
                              {"collection", "MyUsers"}
                          };
            MembershipUtilities.SetDefaultMembershipProvider(membershipProvider, nvc);

            provider.Verify(m => m.GetCollection("mongodb://alternate/?slaveOk=true", "TESTDB", "MyUsers"), Times.Once());
        }
    }
}
