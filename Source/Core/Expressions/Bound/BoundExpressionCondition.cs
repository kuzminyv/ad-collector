using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Expressions
{
    public class BoundExpressionCondition : BoundExpressionToken
    {
        private readonly BoundExpressionToken[] _leftTokens;
        public BoundExpressionToken[] LeftTokens
        {
            get
            {
                return _leftTokens;
            }
        }

        private readonly BoundExpressionToken[] _rightTokens;
        public BoundExpressionToken[] RightTokens
        {
            get
            {
                return _rightTokens;
            }
        }

        private readonly BoundExpressionGroup _leftCondition;
        public BoundExpressionGroup LeftCondition
        {
            get
            {
                return _leftCondition;
            }
        }

        private readonly BoundExpressionGroup _rightCondition;
        public BoundExpressionGroup RightCondition
        {
            get
            {
                return _rightCondition;
            }
        }

        public BoundExpressionCondition(BoundExpressionGroup leftCondition, BoundExpressionToken[] leftTokens,
            BoundExpressionGroup rightCondition, BoundExpressionToken[] rightTokens)
        {
            _leftCondition = leftCondition;
            _leftTokens = leftTokens;
            _rightCondition = rightCondition;
            _rightTokens = rightTokens;

        }
    }
}
