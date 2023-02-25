using System.Text.Json;
using LineBff.DataAccess.Datasource;
using LineBff.ResponseDTO;

namespace LineBff.DataAccess
{
	public interface ILineRepository
	{
		bool AddLineState(string state);
		bool AddLineAccessToken(GenerateAccesstokenResponse dto);
		string GetLineState();
		GenerateAccesstokenResponse GetLineAccessToken();
    }

	public class LineRepository: ILineRepository
	{
		private readonly ICacheDataSource _cacheDataSource;
		private readonly string _lineStateKey = "line_state";
		private readonly string _lineAccessTokenKey = "line_access_token";


        public LineRepository(ICacheDataSource cacheDataSource)
		{
			_cacheDataSource = cacheDataSource;
		}

        public bool AddLineState(string state)
        {
			return _cacheDataSource.SetStringValue(_lineStateKey, state, 30); //30分でstateの検証はできなくなる
        }

        public bool AddLineAccessToken(GenerateAccesstokenResponse dto)
        {
			var json = JsonSerializer.Serialize(dto);
            return _cacheDataSource.SetStringValue(_lineAccessTokenKey, json, 10080); //1週間でアクセストークンがなくなる
        }

        public string GetLineState()
        {
			var value = _cacheDataSource.GetValue(_lineStateKey);
			return value.ToString();
        }

        public GenerateAccesstokenResponse GetLineAccessToken()
        {
			var value = _cacheDataSource.GetValue(_lineAccessTokenKey);
			var response = JsonSerializer.Deserialize<GenerateAccesstokenResponse>(value.ToString()) ?? throw new SystemException();
            return response;
        }
    }
}

