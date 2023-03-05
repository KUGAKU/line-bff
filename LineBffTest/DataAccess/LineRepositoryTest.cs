using Flurl.Http.Testing;
using LineBff.DataAccess;
using LineBff.DataAccess.Datasource;
using LineBff.RequestDTO;
using LineBff.ResponseDTO;
using Moq;

namespace LineBffTest.DataAccess
{
    [TestClass]
    public class LineRepositoryTest
    {
        [TestMethod]
        public void PositiveAddLineStateTrueTest()
        {
            //Arrange:
            var cacheDataSourceMock = new Mock<ICacheDataSource>();
            var expected = true;
            cacheDataSourceMock.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            //and
            var lineRepository = new LineRepository(cacheDataSourceMock.Object);

            // Act:
            var actual = lineRepository.AddLineState("test_state");

            // Assert:
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PositiveAddLineStateFalseTest()
        {
            //Arrange:
            var cacheDataSourceMock = new Mock<ICacheDataSource>();
            var expected = false;
            cacheDataSourceMock.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            //and
            var lineRepository = new LineRepository(cacheDataSourceMock.Object);

            // Act:
            var actual = lineRepository.AddLineState("test_state");

            // Assert:
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PositiveAddLineAccessTokenFalseTest()
        {
            //Arrange:
            var cacheDataSourceMock = new Mock<ICacheDataSource>();
            var expected = false;
            cacheDataSourceMock.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(false);

            //and
            var lineRepository = new LineRepository(cacheDataSourceMock.Object);

            // Act:
            var actual = lineRepository.AddLineAccessToken(
                new GenerateAccesstokenResponse()
                {
                    AccessToken = "test_access_token",
                    ExpiresIn = 12345,
                    IdToken = "test_id_token",
                    RefreshToken = "test_refresh_token",
                    Scope = "test_scope",
                    TokenType = "test_token_type"
                });

            // Assert:
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PositiveAddLineAccessTokenTrueTest()
        {
            //Arrange:
            var cacheDataSourceMock = new Mock<ICacheDataSource>();
            var expected = true;
            cacheDataSourceMock.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(true);

            //and
            var lineRepository = new LineRepository(cacheDataSourceMock.Object);

            // Act:
            var actual = lineRepository.AddLineAccessToken(
                new GenerateAccesstokenResponse()
                {
                    AccessToken = "test_access_token",
                    ExpiresIn = 12345,
                    IdToken = "test_id_token",
                    RefreshToken = "test_refresh_token",
                    Scope = "test_scope",
                    TokenType = "test_token_type"
                });

            // Assert:
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PositiveGetLineStateTest()
        {
            //Arrange:
            var cacheDataSourceMock = new Mock<ICacheDataSource>();
            var expected = "test_state";
            cacheDataSourceMock.Setup(x => x.GetValue(It.IsAny<string>())).Returns("test_state");

            //and
            var lineRepository = new LineRepository(cacheDataSourceMock.Object);

            // Act:
            var actual = lineRepository.GetLineState();

            // Assert:
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PositiveGetLineAccessToken()
        {
            //Arrange:
            var cacheDataSourceMock = new Mock<ICacheDataSource>();
            var expected = new GenerateAccesstokenResponse()
            {
                AccessToken = "eyJhbGciOiJIUzI1NiJ9.j7oM-FeF395HmQORxcHJU2J2OsNFlUj7P1fa0QS0i1cgsroITVRyiTC6Pg3Uwk_CSjJsPBqCbOlvIKTcmTQFmxqK58NiF94JgUCYsCS6RlK9Z7dnxjAOWnFQ746NMdoVqBTNgL9v8L_AcYGIg7ginG2CEItf06YZbv2amzfVyLM.ydz74FRzZCxlgrTV4L470FByt_5BuuZcnjTpHND9N8o",
                ExpiresIn = 2592000,
                IdToken = null,
                RefreshToken = "8QU722RJ8y0lEsqacdpm",
                Scope = "profile",
                TokenType = "Bearer"
            };
            cacheDataSourceMock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(
                "{\"access_token\": \"eyJhbGciOiJIUzI1NiJ9.j7oM-FeF395HmQORxcHJU2J2OsNFlUj7P1fa0QS0i1cgsroITVRyiTC6Pg3Uwk_CSjJsPBqCbOlvIKTcmTQFmxqK58NiF94JgUCYsCS6RlK9Z7dnxjAOWnFQ746NMdoVqBTNgL9v8L_AcYGIg7ginG2CEItf06YZbv2amzfVyLM.ydz74FRzZCxlgrTV4L470FByt_5BuuZcnjTpHND9N8o\",\"expires_in\": 2592000, \"id_token\": null,\"refresh_token\": \"8QU722RJ8y0lEsqacdpm\",\"scope\": \"profile\",\"token_type\": \"Bearer\"}");
            //and
            var lineRepository = new LineRepository(cacheDataSourceMock.Object);

            // Act:
            var actual = lineRepository.GetLineAccessToken();

            // Assert:
            Assert.AreEqual(expected.AccessToken, actual.AccessToken);
            Assert.AreEqual(expected.ExpiresIn, actual.ExpiresIn);
            Assert.AreEqual(expected.IdToken, actual.IdToken);
            Assert.AreEqual(expected.RefreshToken, actual.RefreshToken);
            Assert.AreEqual(expected.Scope, actual.Scope);
            Assert.AreEqual(expected.TokenType, actual.TokenType);
        }

        [TestMethod]
        public async Task PositiveGenerateAccesstokenTest()
        {
            //Arrange:
            Environment.SetEnvironmentVariable("LINE_CALLBACK_URL", "test_line_callback_url", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("LINE_CLIENT_ID", "test_line_client_id", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("LINE_CLIENT_SECRET", "test_line_client_secret", EnvironmentVariableTarget.Process);
            var cacheDataSourceMock = new Mock<ICacheDataSource>();
            var expected = new GenerateAccesstokenResponse()
            {
                AccessToken = "eyJhbGciOiJIUzI1NiJ9.j7oM-FeF395HmQORxcHJU2J2OsNFlUj7P1fa0QS0i1cgsroITVRyiTC6Pg3Uwk_CSjJsPBqCbOlvIKTcmTQFmxqK58NiF94JgUCYsCS6RlK9Z7dnxjAOWnFQ746NMdoVqBTNgL9v8L_AcYGIg7ginG2CEItf06YZbv2amzfVyLM.ydz74FRzZCxlgrTV4L470FByt_5BuuZcnjTpHND9N8o",
                ExpiresIn = 2592000,
                IdToken = null,
                RefreshToken = "8QU722RJ8y0lEsqacdpm",
                Scope = "profile",
                TokenType = "Bearer"
            };

            //and
            var lineRepository = new LineRepository(cacheDataSourceMock.Object);
            var generateAccesstokenRequest = new GenerateAccesstokenRequest()
            {
                AuthorizationCode = "test_authorization_code",
                State = "test_state"
            };

            GenerateAccesstokenResponse? actual;
            //FlurlHttp Fake Act:
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith("{\"access_token\": \"eyJhbGciOiJIUzI1NiJ9.j7oM-FeF395HmQORxcHJU2J2OsNFlUj7P1fa0QS0i1cgsroITVRyiTC6Pg3Uwk_CSjJsPBqCbOlvIKTcmTQFmxqK58NiF94JgUCYsCS6RlK9Z7dnxjAOWnFQ746NMdoVqBTNgL9v8L_AcYGIg7ginG2CEItf06YZbv2amzfVyLM.ydz74FRzZCxlgrTV4L470FByt_5BuuZcnjTpHND9N8o\",\"expires_in\": 2592000, \"id_token\": null,\"refresh_token\": \"8QU722RJ8y0lEsqacdpm\",\"scope\": \"profile\",\"token_type\": \"Bearer\"}", 200);
                actual = await lineRepository.GenerateAccesstoken(generateAccesstokenRequest);
            }

            // Assert:
            Assert.AreEqual(expected.AccessToken, actual.AccessToken);
            Assert.AreEqual(expected.ExpiresIn, actual.ExpiresIn);
            Assert.AreEqual(expected.IdToken, actual.IdToken);
            Assert.AreEqual(expected.RefreshToken, actual.RefreshToken);
            Assert.AreEqual(expected.Scope, actual.Scope);
            Assert.AreEqual(expected.TokenType, actual.TokenType);
        }

        [TestMethod]
        public async Task NegativeGenerateAccesstokenTest()
        {
            //Arrange:
            Environment.SetEnvironmentVariable("LINE_CALLBACK_URL", "test_line_callback_url", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("LINE_CLIENT_ID", "test_line_client_id", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("LINE_CLIENT_SECRET", "test_line_client_secret", EnvironmentVariableTarget.Process);
            var cacheDataSourceMock = new Mock<ICacheDataSource>();
            var expected = new GenerateAccesstokenResponse()
            {
                AccessToken = "eyJhbGciOiJIUzI1NiJ9.j7oM-FeF395HmQORxcHJU2J2OsNFlUj7P1fa0QS0i1cgsroITVRyiTC6Pg3Uwk_CSjJsPBqCbOlvIKTcmTQFmxqK58NiF94JgUCYsCS6RlK9Z7dnxjAOWnFQ746NMdoVqBTNgL9v8L_AcYGIg7ginG2CEItf06YZbv2amzfVyLM.ydz74FRzZCxlgrTV4L470FByt_5BuuZcnjTpHND9N8o",
                ExpiresIn = 2592000,
                IdToken = null,
                RefreshToken = "8QU722RJ8y0lEsqacdpm",
                Scope = "profile",
                TokenType = "Bearer"
            };

            //and
            var lineRepository = new LineRepository(cacheDataSourceMock.Object);
            var generateAccesstokenRequest = new GenerateAccesstokenRequest()
            {
                AuthorizationCode = "test_authorization_code",
                State = "test_state"
            };

            SystemException exception;
            //FlurlHttp Fake Act:
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith("{\"access_token\": \"eyJhbGciOiJIUzI1NiJ9.j7oM-FeF395HmQORxcHJU2J2OsNFlUj7P1fa0QS0i1cgsroITVRyiTC6Pg3Uwk_CSjJsPBqCbOlvIKTcmTQFmxqK58NiF94JgUCYsCS6RlK9Z7dnxjAOWnFQ746NMdoVqBTNgL9v8L_AcYGIg7ginG2CEItf06YZbv2amzfVyLM.ydz74FRzZCxlgrTV4L470FByt_5BuuZcnjTpHND9N8o\",\"expires_in\": 2592000, \"id_token\": null,\"refresh_token\": \"8QU722RJ8y0lEsqacdpm\",\"scope\": \"profile\",\"token_type\": \"Bearer\"}", 201);
                exception = await Assert.ThrowsExceptionAsync<SystemException>(async () =>
                {
                    await lineRepository.GenerateAccesstoken(generateAccesstokenRequest);
                });
            }

            // Assert:
            Assert.AreEqual("System error.", exception.Message);
        }

    }
}