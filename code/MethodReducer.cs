using System.Collections.Generic;

public class Helpers
{
	private class CallQuery
	{
		public System.Action Method;
		public double Debounce;
	}

	private static Dictionary<string, CallQuery> Calls = new Dictionary<string, CallQuery>();

	public static void Debounce(System.Action methodToDebounce, double frequencyOfCall, double delta, string uniqueIdOfCaller = null)
	{
		var id = $"{uniqueIdOfCaller}::{methodToDebounce.Method.Name}";
		Calls.TryGetValue(id, out CallQuery item);

		if (item == null)
		{
			Calls.Add(id, new CallQuery()
			{
				Debounce = frequencyOfCall,
				Method = methodToDebounce
			});
		}
		else
		{
			item.Debounce -= delta;
			if (item.Debounce < 0)
			{
				item.Debounce = frequencyOfCall;
				item.Method();
			}
		}
	}
}