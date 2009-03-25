using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation
{
	delegate Number CallBack(Object state);

	delegate Node NodeParse(ParseState state);

	delegate Node NodeBuild(List<Token> tokens, ref Int32 tokenindex);
}