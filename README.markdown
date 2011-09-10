# MongoDB.Web
## A collection of ASP.NET providers (caching, membership, profiles, roles, session state, web events) for MongoDB.

NuGet Package: http://nuget.org/List/Packages/MongoDB.Web

## Configuration
To use MongoDB.Web, add the providers you're interested in to your Web.config file:

    <system.web>
        <caching>
            <outputCache defaultProvider="MongoDBOutputCache">
                <providers>
                    <add name="MongoDBOutputCache" type="MongoDB.Web.Providers.MongoDBOutputCacheProvider"
                        connectionString="mongodb://localhost" database="ASPNETDB" collection="OutputCache" />
                </providers>
            </outputCache>
        </caching>
        <healthMonitoring enabled="true">
            <providers>
                <add name="MongoDBWebEventProvider" type="MongoDB.Web.Providers.MongoDBWebEventProvider" bufferMode="Notification"
                    connectionString="mongodb://localhost" database="ASPNETDB" collection="WebEvents" />
            </providers>
            <rules>
                <add name="LogAllEvents" eventName="All Events" provider="MongoDBWebEventProvider" />
            </rules>
        </healthMonitoring>
        <membership defaultProvider="MongoDBMembershipProvider">
            <providers>
                <clear />
                <add name="MongoDBMembershipProvider" type="MongoDB.Web.Providers.MongoDBMembershipProvider" applicationName="/"
                    connectionString="mongodb://localhost" database="ASPNETDB" collection="Users"
                    enablePasswordRetrieval="false" enablePasswordReset="true" requiresQuestionAndAnswer="false" requiresUniqueEmail="false"
                    maxInvalidPasswordAttempts="5" minRequiredPasswordLength="6" minRequiredNonalphanumericCharacters="0" passwordAttemptWindow="10" />
            </providers>
        </membership>
        <profile defaultProvider="MongoDBProfileProvider" enabled="true">
            <properties>
                <add name="Age" type="Int32"/>
                <add name="Country" type="string"/>
                <add name="Gender" type="string"/>
            </properties>
            <providers>
                <clear/>
                <add name="MongoDBProfileProvider" type="MongoDB.Web.Providers.MongoDBProfileProvider" applicationName="/"
                    connectionString="mongodb://localhost" database="ASPNETDB" collection="Profiles" />
            </providers>
        </profile>
        <roleManager enabled="false">
            <providers>
                <clear/>
                <add name="MongoDBRoleProvider" type="MongoDB.Web.Providers.MongoDBRoleProvider" applicationName="/"
                    connectionString="mongodb://localhost" database="ASPNETDB" collection="Roles" />
            </providers>
        </roleManager>
        <sessionState mode="Custom" customProvider="MongoDBSessionStateProvider">
            <providers>
                <add name="MongoDBSessionStateProvider" type="MongoDB.Web.Providers.MongoDBSessionStateProvider"
                    connectionString="mongodb://localhost" database="ASPNETDB" collection="ASPState" />
            </providers>
        </sessionState>
    </system.web>