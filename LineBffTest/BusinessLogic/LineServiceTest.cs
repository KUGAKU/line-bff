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
			//given
			Environment.SetEnvironmentVariable("LINE_CLIENT_ID", "test_line_client_id",EnvironmentVariableTarget.Process);
			Environment.SetEnvironmentVariable("LINE_CALLBACK_URL", "test_line_callback_url", EnvironmentVariableTarget.Process);

			var lineRepositoryMock = new Mock<ILineRepository>();
			lineRepositoryMock.Setup(x => x.AddLineState(It.IsAny<string>())).Returns(true);

			//and
			var service = new LineService(lineRepositoryMock.Object);

			//when
			var actual = service.GenerateAuthURL();

			//then
			Assert.IsTrue(actual.AuthURL.Contains("response_type"));
			Assert.IsTrue(actual.AuthURL.Contains("client_id"));
			Assert.IsTrue(actual.AuthURL.Contains("redirect_uri"));
			Assert.IsTrue(actual.AuthURL.Contains("state"));
			Assert.IsTrue(actual.AuthURL.Contains("scope"));
		}

		[TestMethod]
		public void NegativeGenerateAuthURLTest()
		{
			//given
			Environment.SetEnvironmentVariable("LINE_CLIENT_ID", "test_line_client_id",EnvironmentVariableTarget.Process);
			Environment.SetEnvironmentVariable("LINE_CALLBACK_URL", "test_line_callback_url", EnvironmentVariableTarget.Process);

			var lineRepositoryMock = new Mock<ILineRepository>();
			lineRepositoryMock.Setup(x => x.AddLineState(It.IsAny<string>())).Returns(false);

			//and
			var service = new LineService(lineRepositoryMock.Object);

			//when
			var exception = Assert.ThrowsException<SystemException>(() => service.GenerateAuthURL());

			//then
			Assert.AreEqual("System error.", exception.Message);
		}

		[TestMethod]
		public async Task NegativeGenerateAccesstoken()
		{
			//given
			var lineRepositoryMock = new Mock<ILineRepository>();
			lineRepositoryMock.Setup(x => x.GetLineState()).Returns("test_valid_line_state");

			//and
			var generateAccesstokenRequest = new GenerateAccesstokenRequest() { AuthorizationCode = "test_authorization_code", State = "test_invalid_line_state" };
			var service = new LineService(lineRepositoryMock.Object);

			//when
			var exception = await Assert.ThrowsExceptionAsync<SystemException>(async () => {
				await service.GenerateAccesstoken(generateAccesstokenRequest);
			});

			//then
			Assert.AreEqual("System error.", exception.Message);	
		}

		[TestMethod]
		public async Task PositiveGenerateAccesstokenTest()
		{
			//given
			var lineRepositoryMock = new Mock<ILineRepository>();
			lineRepositoryMock.Setup(x => x.GetLineState()).Returns("test_valid_line_state");
			lineRepositoryMock.Setup(x => x.GenerateAccesstoken(It.IsAny<GenerateAccesstokenRequest>())).Returns(Task.FromResult(new GenerateAccesstokenResponse() {
				AccessToken = "test_access_token",
				ExpiresIn = 123345,
				IdToken = "test_id_token",
				RefreshToken = "test_refresh_token",
				Scope = "test_scope",
				TokenType = "test_token_type"
			}));

			//and
			var expected = new GenerateAccesstokenResponse() {
				AccessToken = "test_access_token",
				ExpiresIn = 123345,
				IdToken = "test_id_token",
				RefreshToken = "test_refresh_token",
				Scope = "test_scope",
				TokenType = "test_token_type"
				};
			var generateAccesstokenRequest = new GenerateAccesstokenRequest() { AuthorizationCode = "test_authorization_code", State = "test_valid_line_state" };
			var service = new LineService(lineRepositoryMock.Object);

			//when
			var actual = await service.GenerateAccesstoken(generateAccesstokenRequest);

			//then
			Assert.AreEqual(expected.AccessToken, actual.AccessToken);
			Assert.AreEqual(expected.ExpiresIn, actual.ExpiresIn);
			Assert.AreEqual(expected.IdToken, actual.IdToken);
			Assert.AreEqual(expected.RefreshToken, actual.RefreshToken);
			Assert.AreEqual(expected.Scope, actual.Scope);
			Assert.AreEqual(expected.TokenType, actual.TokenType);
		}
    }
}