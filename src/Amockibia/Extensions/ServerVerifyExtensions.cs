namespace Amockibia.Extensions
{
    public static class ServerVerifyExtensions
    {
        public static RuleExecutionResult Retrieve(this AmockibiaServer server, string ruleId)
        {
            return new RuleExecutionResult(server.GetHandler(ruleId));
        }
    }
}