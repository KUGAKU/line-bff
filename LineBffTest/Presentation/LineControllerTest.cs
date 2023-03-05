using System.Net;
using LineBff;
using LineBff.BusinessLogic;
using LineBff.RequestDTO;
using LineBff.ResponseDTO;
using LineBffTest.Extension;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;

namespace LineBffTest;

[TestClass]
public class LineControllerTest
{

    [TestMethod]
    public void PositiveRunGenerateAuthURLTest()
    {
        //Arrange:
        var requestMock = new MockHttpRequestData("");

        requestMock.CreateResponse(HttpStatusCode.OK);

        var serviceMock = new Mock<ILineService>();
        serviceMock.Setup(x => x.GenerateAuthURL()).Returns(new GenerateAuthURLResponse() { AuthURL = "https://access.line.me/oauth2/v2.1/authorize/xxxx" });

        //and
        var controller = new LineController(serviceMock.Object);
        var expected = HttpStatusCode.OK;

        // Act:
        var responseData = controller.RunGenerateAuthURL(requestMock);
        var actual = responseData.StatusCode;

        // Assert:
        Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public async Task PositiveRunGenerateAccesstokenTest()
    {

        //Arrange:
        var requestMock = new MockHttpRequestData("{\"AuthorizationCode\":\"pqYNb7TvtrPKq7B1N9Ax\",\"State\": \"ajOupwzbKsspguG\"}");
        requestMock.CreateResponse(HttpStatusCode.OK);

        var serviceMock = new Mock<ILineService>();
        serviceMock.Setup(x => x.GenerateAccesstoken(It.IsAny<GenerateAccesstokenRequest>()))
            .Returns(Task.FromResult(new GenerateAccesstokenResponse()
            {
                AccessToken = "TestAccessToken",
                ExpiresIn = 5000,
                IdToken = "TestIdToken",
                RefreshToken = "TestRefreshToken",
                Scope = "TestScope",
                TokenType = "TestTokenType"
            }));

        //and
        var controller = new LineController(serviceMock.Object);
        var expected = HttpStatusCode.OK;

        // Act:
        var responseData = await controller.RunGenerateAccesstoken(requestMock);
        var actual = responseData.StatusCode;

        // Assert:
        Assert.AreEqual(expected, actual);
    }
}
