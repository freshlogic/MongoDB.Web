using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Web.Providers;

namespace MongoDB.Web.Tests
{
    static internal class MembershipUtilities
    {
        public static void SetDefaultMembershipProvider(MongoDBMembershipProvider membershipProvider, NameValueCollection initializationSettings = null)
        {
            var testName = new StackTrace(StackTrace.METHODS_TO_SKIP + 1).GetFrames()
                .Select(f => f.GetMethod())
                .First(m => m.IsDefined(typeof(TestMethodAttribute), true))
                .Name;
            const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
            membershipProvider.Initialize("MongoDBMembershipProvider_" + testName, initializationSettings ?? new NameValueCollection());
            typeof(Membership).GetField("s_Providers", flags).SetValue(null, new MembershipProviderCollection { membershipProvider });
            typeof(Membership).GetField("s_Provider", flags).SetValue(null, membershipProvider);
            typeof(Membership).GetField("s_Initialized", flags).SetValue(null, true);
            typeof(Membership).GetField("s_InitializedDefaultProvider", flags).SetValue(null, true);
        }
    }
}