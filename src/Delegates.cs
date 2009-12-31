using System;
using System.Collections.Generic;

namespace xnaMugen
{
	delegate Object Constructor(params Object[] args);
}

namespace xnaMugen.Evaluation
{
	delegate Node NodeParse(ParseState state);

	delegate Node NodeBuild(List<Token> tokens, ref Int32 tokenindex);

	delegate Number EvaluationCallback(Object state);
}