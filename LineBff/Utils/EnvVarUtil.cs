namespace LineBff.Utils
{
	public static class EnvVarUtil
	{
        public static string GetEnvVarByKeyStr(string keyStr)
        {
            try
            {
                if (Environment.GetEnvironmentVariable(keyStr, EnvironmentVariableTarget.Process) == null) {
                    throw new NullEnvironmentVariableException(keyStr);
                }
                return Environment.GetEnvironmentVariable(keyStr, EnvironmentVariableTarget.Process)!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class NullEnvironmentVariableException : Exception
    {
        public string VariableName { get; }

        public NullEnvironmentVariableException(string variableName)
            : base($"The environment variable '{variableName}' is null.")
        {
            VariableName = variableName;
        }
    }
}

