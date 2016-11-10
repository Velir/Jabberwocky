namespace Jabberwocky.Extras.Polly.Sc.Constants
{
	public static class FieldConstants
	{
		public const string UseCircuitBreaker = "Use Circuit Breaker";
		public const string OpenCircuitDurationInSeconds = "Open Circuit Duration In Seconds";
		public const string HideOnError = "Hide On Error";
		public const string BreakAfterExceptionCount = "Break After Exception Count";

		public const string PolicyVariesByDatasource = "Circuit Breaker Varies By Datasource";
		public const string PolicyVariesByLanguage = "Circuit Breaker Varies By Language";
		public const string PolicyVariesBySite = "Circuit Breaker Varies By Site";
	}
}
