using System;

namespace LogToHtml.Errors
{
	internal class Logging
	{
		[Serializable]
		public class NoProjectsInOptions : Exception
		{
			public NoProjectsInOptions() { }

			public NoProjectsInOptions(string message)
				: base(message) { }
		}

		[Serializable]
		public class NoProjectInOptions : Exception
		{
			public NoProjectInOptions() { }

			public NoProjectInOptions(string message)
				: base(message) { }
		}

		[Serializable]
		public class ProjectsDoesNotContainProject : Exception
		{
			public ProjectsDoesNotContainProject() { }

			public ProjectsDoesNotContainProject(string message)
				: base(message) { }
		}
	}
}
