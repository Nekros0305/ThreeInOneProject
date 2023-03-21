using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

//Original Source: https://github.com/aemarco/aemarcoCommons/blob/4e38cac668da20619ba45b3206a4327551e86707/WpfTools/BaseModels/LazyProperty.cs

namespace ThreeInOne.WPFStuff
{
    public partial class LazyProperty<T> : ObservableObject, INotifyPropertyChanged
    {
        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private readonly Func<CancellationToken, Task<T>> _retrievalFunc;
        private readonly T _defaultValue;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8601 // Possible null reference assignment.
        public LazyProperty(Func<CancellationToken, Task<T>> retrievalFunc, T defaultValue = default)
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _retrievalFunc = retrievalFunc ?? throw new ArgumentNullException(nameof(retrievalFunc));
            _defaultValue = defaultValue;
        }

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _errorOnLoading;

        private bool _isLoaded;
        private T _value;
        public T Value
        {
            get
            {
                if (_isLoaded)
                    return _value;

                if (IsLoading)
                    return _defaultValue;

                IsLoading = true;
                LoadValueAsync()
                    .ContinueWith((t) =>
                    {
                        if (t.IsCanceled)
                            return;

                        if (t.IsFaulted)
                        {
                            _value = _defaultValue;
                            ErrorOnLoading = true;
                            _isLoaded = true;
                            IsLoading = false;
                            OnPropertyChanged(nameof(Value));
                        }
                        else
                        {
                            Value = t.Result;
                        }
                    });
                return _defaultValue;
            }
            // if you want a ReadOnly-property just set this setter to private
            set
            {
                if (IsLoading)
                    // since we set the value now, there is no need
                    // to retrieve the "old" value asynchronously
                    CancelLoading();

                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                _value = value;
                _isLoaded = true;
                IsLoading = false;
                ErrorOnLoading = false;

                OnPropertyChanged();
            }
        }

        private async Task<T> LoadValueAsync()
            => await _retrievalFunc(_cancelTokenSource.Token);

        public void CancelLoading()
            => _cancelTokenSource.Cancel();

        /// <summary>
        /// This allows you to assign the value of this lazy property directly
        /// to a variable of type T
        /// </summary>        
        public static implicit operator T(LazyProperty<T> p)
            => p.Value;
    }
}