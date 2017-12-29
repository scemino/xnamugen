using System.Collections.Generic;

namespace xnaMugen
{
	internal delegate object Constructor(params object[] args);
}

namespace xnaMugen.Evaluation
{
	internal delegate Node NodeParse(ParseState state);

	internal delegate Node NodeBuild(List<Token> tokens, ref int tokenindex);

	internal delegate Number EvaluationCallback(object state);
}