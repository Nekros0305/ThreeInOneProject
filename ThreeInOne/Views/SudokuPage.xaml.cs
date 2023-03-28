using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using ThreeInOne.Models.Sudoku;
using ThreeInOne.ViewModels.Sudoku;

namespace ThreeInOne.Views;

public partial class SudokuPage : ContentPage,
    IRecipient<UnableToSolveMessage>,
    IRecipient<SudokuBoardErrorMessage>
{
    private readonly SudokuPageViewModel _viewModel;
    private readonly IValueConverter _valueConverter;
    public SudokuPage(SudokuPageViewModel vm,
        IValueConverter valueConverter,
        IMessenger messenger)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
        _valueConverter = valueConverter;

        messenger.RegisterAll(this);
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int x = 0; x < 9; x++)
            {
                var entry = new Entry()
                {
                    Placeholder = "X",
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    MaxLength = 1,
                    Keyboard = Keyboard.Numeric,
                }.Behaviors(new NumericValidationBehavior
                {
                    InvalidStyle = new Style<Entry>(Entry.TextColorProperty, Colors.Red),
                    ValidStyle = new Style<Entry>(Entry.TextColorProperty, Colors.Green),
                    Flags = ValidationFlags.ValidateOnValueChanged,
                    MinimumValue = 1.0,
                    MaximumValue = 9.0,
                });
                entry.SetBinding(Entry.TextProperty, new Binding(nameof(_viewModel.Board))
                {
                    Converter = _valueConverter,
                    ConverterParameter = $"{x},{i}",
                    Mode = BindingMode.TwoWay,
                });
                entry.SetBinding(IsEnabledProperty, new Binding(nameof(_viewModel.SolveBoardCommand.IsRunning)));
                entry.SetBinding(InputView.IsReadOnlyProperty, new Binding(nameof(_viewModel.IsSolved)));
                entry.BindingContext = _viewModel;

                Border border = new()
                {
                    StrokeThickness = 0.5,
                    Stroke = Colors.Gray,
                    Content = entry,
                };

                border.SetValue(Grid.RowProperty, i);
                border.SetValue(Grid.ColumnProperty, x);
                Playground.Children.Add(border);
            }
        }
    }

    async void IRecipient<UnableToSolveMessage>.Receive(UnableToSolveMessage message)
        => await DisplayAlert("Error", message.Value, "OK");

    async void IRecipient<SudokuBoardErrorMessage>.Receive(SudokuBoardErrorMessage message)
        => await DisplayAlert("Rules Break", message.Value, "OK");
}