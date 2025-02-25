﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Client.Internal.Broker;
using Microsoft.Identity.Client.OAuth2;
using Microsoft.Identity.Client.PlatformsCommon.Shared;
using Microsoft.Identity.Client.TelemetryCore;
using Microsoft.Identity.Client.Utils;
using Microsoft.Identity.Json.Linq;
using Microsoft.Identity.Test.Common.Core.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Test.Unit.CoreTests.OAuth2Tests
{
    [TestClass]
    public class TokenResponseTests : TestBase
    {
        [TestMethod]
        public void JsonDeserializationTest()
        {
            using (var harness = CreateTestHarness())
            {
                harness.HttpManager.AddSuccessTokenResponseMockHandlerForPost(TestConstants.AuthorityCommonTenant);

                OAuth2Client client = new OAuth2Client(harness.ServiceBundle.ApplicationLogger, harness.HttpManager);

                Task<MsalTokenResponse> task = client.GetTokenAsync(
                    new Uri(TestConstants.AuthorityCommonTenant + "oauth2/v2.0/token"),
                    new RequestContext(harness.ServiceBundle, Guid.NewGuid()), addCommonHeaders: true, onBeforePostRequestHandler: null);
                MsalTokenResponse response = task.Result;
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public void AndroidBrokerTokenResponseParseTest()
        {
            string unixTimestamp = DateTimeHelpers.DateTimeToUnixTimestamp(DateTimeOffset.UtcNow + TimeSpan.FromMinutes(40));
            string androidBrokerResponse = TestConstants.AndroidBrokerResponse.Replace("1591196764", unixTimestamp); 
            string correlationId = Guid.NewGuid().ToString();
            // Act
            var msalTokenResponse = MsalTokenResponse.CreateFromAndroidBrokerResponse(androidBrokerResponse, correlationId);

            // Assert
            Assert.AreEqual("secretAt", msalTokenResponse.AccessToken);
            Assert.AreEqual(correlationId, msalTokenResponse.CorrelationId);
            Assert.AreEqual("clientInfo", msalTokenResponse.ClientInfo);
            Assert.AreEqual("idT", msalTokenResponse.IdToken);
            Assert.AreEqual("User.Read openid offline_access profile", msalTokenResponse.Scope);
            Assert.AreEqual("Bearer", msalTokenResponse.TokenType);

            Assert.IsNull(msalTokenResponse.RefreshToken);
        }       
    }
}
