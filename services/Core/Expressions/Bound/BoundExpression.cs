using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;

namespace Core.Expressions
{
	public class BoundExpression : BoundExpressionToken
	{
		private List<BoundExpressionToken> _tokens;

		public List<BoundMatch> Matches(string input)
		{
			var matches = new List<BoundMatch>();

			int startIndex  = 0;
			bool next = true;

			while (next)
			{
				var matchValues = new Dictionary<string, string>(_tokens.Count);
                var tokens = _tokens;

                for (int i = 0; i < tokens.Count; i++)
                {
                    BoundExpressionToken token = tokens[i];
                    if (token is BoundExpressionCondition)
                    {
                        tokens = EvaluateCondition((BoundExpressionCondition)token, input, startIndex).ToList();
                        if (tokens.Count == 0)
                        {
                            next = false;
                            break;
                        }
                        else                        
                        {
                            i = -1;
                            continue;
                        }
                    }
                    else
                    {
                        BoundExpressionGroup groupToken = (BoundExpressionGroup)token;
                        BoundSelectorResult selectorResult = groupToken.GetResult(input, startIndex);
                        if (groupToken.IsOptional)
                        {
                            if (selectorResult == null)
                            {
                                matchValues.Add(groupToken.Name, null);
                                continue;
                            }

                            BoundExpressionGroup lastRequiredGroup = (BoundExpressionGroup)tokens.Last(t => t is BoundExpressionGroup && !((BoundExpressionGroup)t).IsOptional);
                            BoundSelectorResult reqResult = lastRequiredGroup.GetResult(input, startIndex);
                            if (reqResult == null || reqResult.SelectorRange.Start < selectorResult.SelectorRange.Start)
                            {
                                matchValues.Add(groupToken.Name, null);
                                continue;
                            }
                        }

                        if (selectorResult == null && !groupToken.IsOptional)
                        {
                            next = false;
                            break;
                        }
                        if (!selectorResult.Selector.HoldPosition)
                        {
                            startIndex = selectorResult.Selector.Superposition ? selectorResult.ValueRange.End : selectorResult.SelectorRange.End;
                        }

                        matchValues.Add(groupToken.Name, selectorResult.GetValue(input).Trim());
                    }
                }
				if (next)
				{
					matches.Add(new BoundMatch(matchValues));
				}
			}

			return matches;
		}

        private BoundExpressionToken[] EvaluateCondition(BoundExpressionCondition expressionCondition, string input, int startIndex)
        {
            BoundSelectorResult leftResult = expressionCondition.LeftCondition.GetResult(input, startIndex);
            BoundSelectorResult rightResult = expressionCondition.RightCondition.GetResult(input, startIndex);
            if (leftResult == null && rightResult == null)
            {
                return new BoundExpressionToken[0];
            }
            else if (leftResult == null && rightResult != null)
            {
                return expressionCondition.RightTokens;
            }
            else if (leftResult != null && rightResult == null)
            {
                return expressionCondition.LeftTokens;
            }
            else 
            {
                return leftResult.SelectorRange.Start <= rightResult.SelectorRange.Start ? 
                    expressionCondition.LeftTokens : expressionCondition.RightTokens;
            }
        }

        public void AddToken(BoundExpressionToken token)
        {
            _tokens.Add(token);
        }

        public BoundExpression(IEnumerable<BoundExpressionToken> tokens)
        {
            _tokens = tokens.ToList();
        }

        public BoundExpression(params BoundExpressionToken[] tokens)
        {
            _tokens = tokens.ToList();
        }
	}
}
