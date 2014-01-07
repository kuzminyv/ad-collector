using System;
using System.ComponentModel;
using System.Diagnostics;

namespace UI.Desktop.Views
{
    public class ViewModel<TModel, TParent> : ViewModel
        where TParent : ViewModel 
        where TModel : class
    {
        public TModel Model
        {
            get;
            protected set;
        }

        public TParent Parent
        {
            get;
            protected set;
        }

        public ViewModel(TParent parent, TModel model)
        {
            Parent = parent;
            Model = model;
        }

        public ViewModel(TParent parent)
            : this(parent, null)
        {
        }

        public ViewModel()
           : this(null, null)
        { 
        }
    }

	public abstract class ViewModel : INotifyPropertyChanged
	{
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Properties
		private bool _hasChanges;
		public bool HasChanges
		{
			get
			{
				return _hasChanges;
			}
			set
			{
				if (_hasChanges != value)
				{					
					_hasChanges = value;
					OnPropertyChanged("HasChanges", false);
				}
			}
		}

		private void OnHasChangesChanged()
		{
			throw new NotImplementedException();
		}
		#endregion

		protected virtual void OnPropertyChanged(string propertyName, bool setHasChanges)
		{
			OnPropertyChanged(propertyName);
			if (setHasChanges)
			{
				HasChanges = true;
			}
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			this.VerifyPropertyName(propertyName);

			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#region Debugging Aides

		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This 
		/// method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// Verify that the property name matches a real,  
			// public, instance property on this object.
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				string msg = "Invalid property name: " + propertyName;

				if (this.ThrowOnInvalidPropertyName)
					throw new ArgumentException(msg);
				else
					Debug.Fail(msg);
			}
		}

		/// <summary>
		/// Returns whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might 
		/// override this property's getter to return true.
		/// </summary>
		protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

		#endregion
	}
}
