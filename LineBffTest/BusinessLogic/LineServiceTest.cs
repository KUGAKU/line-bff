using LineBff.BusinessLogic;
using LineBff.DataAccess;
using LineBff.RequestDTO;
using LineBff.ResponseDTO;
using Moq;

namespace LineBffTest.BusinessLogic
{
    [TestClass]
    public class LineServiceTest
    {
        [TestMethod]
        public void PositiveGenerateAuthURLTest()
        {
            //Arrange:
            Environment.SetEnvironmentVariable("LINE_CLIENT_ID", "test_line_client_id", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("LINE_CALLBACK_URL", "test_line_callback_url", EnvironmentVariableTarget.Process);

            var lineRepositoryMock = new Mock<ILineRepository>();
            lineRepositoryMock.Setup(x => x.AddLineState(It.IsAny<string>())).Returns(true);

            //and
            var service = new LineService(lineRepositoryMock.Object);

            // Act:
            var actual = service.GenerateAuthURL();

            // Assert:
            Assert.IsTrue(actual.AuthURL.Contains("response_type"));
            Assert.IsTrue(actual.AuthURL.Contains("client_id"));
            Assert.IsTrue(actual.AuthURL.Contains("redirect_uri"));
            Assert.IsTrue(actual.AuthURL.Contains("state"));
            Assert.IsTrue(actual.AuthURL.Contains("scope"));
        }

        [TestMethod]
        public void NegativeGenerateAuthURLTest()
        {
            //Arrange:
            Environment.SetEnvironmentVariable("LINE_CLIENT_ID", "test_line_client_id", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("LINE_CALLBACK_URL", "test_line_callback_url", EnvironmentVariableTarget.Process);

            var lineRepositoryMock = new Mock<ILineRepository>();
            lineRepositoryMock.Setup(x => x.AddLineState(It.IsAny<string>())).Returns(false);

            //and
            var service = new LineService(lineRepositoryMock.Object);

            // Act:
            var exception = Assert.ThrowsException<SystemException>(() => service.GenerateAuthURL());

            // Assert:
            Assert.AreEqual("System error.", exception.Message);
        }

        [TestMethod]
        public async Task NegativeGenerateAccesstoken()
        {
            //Arrange:
            var lineRepositoryMock = new Mock<ILineRepository>();
            lineRepositoryMock.Setup(x => x.GetLineState()).Returns("test_valid_line_state");

            //and
            var generateAccesstokenRequest = new GenerateAccesstokenRequest() { AuthorizationCode = "test_authorization_code", State = "test_invalid_line_state" };
            var service = new LineService(lineRepositoryMock.Object);

            // Act:
            var exception = await Assert.ThrowsExceptionAsync<SystemException>(async () =>
            {
                await service.GenerateAccesstoken(generateAccesstokenRequest);
            });

            // Assert:
            Assert.AreEqual("System error.", exception.Message);
        }

        [TestMethod]
        public async Task PositiveGenerateAccesstokenTest()
        {
            //Arrange:
            var lineRepositoryMock = new Mock<ILineRepository>();
            lineRepositoryMock.Setup(x => x.GetLineState()).Returns("test_valid_line_state");
            lineRepositoryMock.Setup(x => x.GenerateAccesstoken(It.IsAny<GenerateAccesstokenRequest>())).Returns(Task.FromResult(new GenerateAccesstokenResponse()
            {
                AccessToken = "test_access_token",
                ExpiresIn = 123345,
                IdToken = "test_id_token",
                RefreshToken = "test_refresh_token",
                Scope = "test_scope",
                TokenType = "test_token_type"
            }));

            //and
            var expected = new GenerateAccesstokenResponse()
            {
                AccessToken = "test_access_token",
                ExpiresIn = 123345,
                IdToken = "test_id_token",
                RefreshToken = "test_refresh_token",
                Scope = "test_scope",
                TokenType = "test_token_type"
            };
            var generateAccesstokenRequest = new GenerateAccesstokenRequest() { AuthorizationCode = "test_authorization_code", State = "test_valid_line_state" };
            var service = new LineService(lineRepositoryMock.Object);

            // Act:
            var actual = await service.GenerateAccesstoken(generateAccesstokenRequest);

            // Assert:
            Assert.AreEqual(expected.AccessToken, actual.AccessToken);
            Assert.AreEqual(expected.ExpiresIn, actual.ExpiresIn);
            Assert.AreEqual(expected.IdToken, actual.IdToken);
            Assert.AreEqual(expected.RefreshToken, actual.RefreshToken);
            Assert.AreEqual(expected.Scope, actual.Scope);
            Assert.AreEqual(expected.TokenType, actual.TokenType);
        }
    }
}