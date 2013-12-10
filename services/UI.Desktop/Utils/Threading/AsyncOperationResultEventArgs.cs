using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace UI.Desktop.Utils
{
	public class AsyncOperationResultEventArgs<TOperationResult> : EventArgs
	{
		private readonly TOperationResult _result;
		public TOperationResult Result
		{
			get
			{
				return _result;
			}
		}

		public AsyncOperationResultEventArgs(TOperationResult result)
		{
			_result = result;
		}
	}
}
