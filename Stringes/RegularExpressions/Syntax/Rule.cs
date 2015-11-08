using System.Collections;
using System.Collections.Generic;

namespace Stringes.RegularExpressions.Syntax
{
	internal abstract class Rule
	{
		public abstract IEnumerator<Rule> Test();
	}
}