using System;

namespace Jabberwocky.Glass.Factory.Exceptions
{
	public class ImplementationMismatchException : Exception
	{
		private const string DefaultPrescriptiveMessageFormat = 
			"The provided Glass Model type '{0}' does not match the type required by the target implementation type '{1}'.\n" + 
			"This may be due to a template change that needs to be published, or Glass Mapper not correctly inferring types.\n" +
			"Make sure that all templates are published, and your solution has Glass definitions for all templates, and that Glass Mapper's inferType is set to true.";

		private readonly string _message;

		public ImplementationMismatchException(Type expectedModelType, object glassModel)
		{
			_message = string.Format(DefaultPrescriptiveMessageFormat, glassModel.GetType().FullName, expectedModelType.FullName);
		}

		public override string Message
		{
			get { return _message; }
		}
	}
}
