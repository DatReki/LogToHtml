using System;

namespace LogToHtml
{
	public class Errors
	{
		/// <summary>
		/// Throw exception if configuration doesn't contain any projects.
		/// </summary>
		[Serializable]
		public class NoProjectsInConfig : Exception
		{
			public NoProjectsInConfig() { }

			public NoProjectsInConfig(string message) : base(message) { }
		}

		/// <summary>
		/// Throw exception if provided color isn't HEX or RGB.
		/// </summary>
		[Serializable]
		public class InvalidColorException : Exception
		{
			public InvalidColorException() { }

			public InvalidColorException(string message) : base(message) { }

			public InvalidColorException(string message, Exception innerException) : base(message, innerException) { }

			public override string HelpLink
			{
				get
				{
					return "Read about what colors we support here: https://spectreconsole.net/appendix/colors";
				}
			}
		}

		/// <summary>
		/// Didn't provide any project in the options.
		/// </summary>
		[Serializable]
		public class NoProjectInOptions : Exception
		{
			public NoProjectInOptions() { }

			public NoProjectInOptions(string message)
				: base(message) { }
		}

		/// <summary>
		/// The project provided in the options doesn't contain any project listed in the configuration.
		/// </summary>
		[Serializable]
		public class ProjectsDoesNotContainProject : Exception
		{
			public ProjectsDoesNotContainProject() { }

			public ProjectsDoesNotContainProject(string message)
				: base(message) { }
		}
	}
}
