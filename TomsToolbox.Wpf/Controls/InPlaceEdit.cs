namespace TomsToolbox.Wpf.Controls
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;

    /// <inheritdoc />
    /// <summary>
    /// Text control supporting in place editing.
    /// <para />
    /// Editing starts
    /// <list type="bullet">
    /// <item>by deferred mouse double click</item>
    /// <item>by pressing F2</item>
    /// <item>setting IsEditing to true</item>
    /// </list>
    /// <para />
    /// Editing terminates
    /// <list type="bullet">
    /// <item>when the focus gets lost (changes accepted)</item>
    /// <item>when setting IsEditing to false (changes accepted)</item>
    /// <item>when the user clicks outside the text box or moves the mouse wheel (changes accepted)</item>
    /// <item>by pressing RETURN (changes accepted)</item>
    /// <item>by pressing ESC (changes rejected)</item>
    /// </list>
    /// </summary>
    [TemplatePart(Name = TextboxPartName, Type = typeof(TextBox))]
    public class InPlaceEdit : Control
    {
        private const string TextboxPartName = "PART_TextBox";
        private static readonly TimeSpan _doubleClickTime = TimeSpan.FromMilliseconds(NativeMethods.GetDoubleClickTime());

        [NotNull]
        private readonly Throttle _mouseDoubleClickThrottle;
        [CanBeNull]
        private TextBox _textBox;
        // The first focusable ancestor to test for focus and to provide F2 key.
        [CanBeNull]
        private FrameworkElement _focusableParent;

        // The time when the parent item got the focus.
        private DateTime _parentGotFocusTime = DateTime.MinValue;
        // State of mouse event handling to prevent editing when double clicking the item.
        private bool _processingMouseLeftButtonDown;
        private bool _mouseDoubleClicked;


        static InPlaceEdit()
        {
            // ReSharper disable once PossibleNullReferenceException
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InPlaceEdit), new FrameworkPropertyMetadata(typeof(InPlaceEdit)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.Wpf.Controls.InPlaceEdit" /> class.
        /// </summary>
        public InPlaceEdit()
        {
            _mouseDoubleClickThrottle = new Throttle(_doubleClickTime, OnMouseDoubleClickThrottle);
            Loaded += Self_Loaded;
            Unloaded += Self_Unloaded;
        }

        /// <summary>
        /// Occurs when the edited text needs to be validated.
        /// </summary>
        public event EventHandler<TextValidationEventArgs> Validate;

        /// <summary>
        /// Gets or sets the text to be edited.
        /// </summary>
        [CanBeNull]
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InPlaceEdit), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus });


        /// <summary>
        /// Gets or sets a value indicating whether editing is active.
        /// </summary>
        public bool IsEditing
        {
            get => this.GetValue<bool>(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="IsEditing"/> dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty IsEditingProperty =
            // ReSharper disable once PossibleNullReferenceException
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(InPlaceEdit), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) => ((InPlaceEdit)sender)?.IsEditing_Changed((bool)e.NewValue), (sender, baseValue) => ((InPlaceEdit)sender)?.IsEditing_CoerceValue(baseValue.SafeCast<bool>())));


        /// <summary>
        /// Gets or sets a value indicating whether editing is currently disabled.
        /// </summary>
        public bool IsEditingDisabled
        {
            get => this.GetValue<bool>(IsEditingDisabledProperty);
            set => SetValue(IsEditingDisabledProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="IsEditingDisabled"/> dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty IsEditingDisabledProperty =
            DependencyProperty.Register("IsEditingDisabled", typeof(bool), typeof(InPlaceEdit), new FrameworkPropertyMetadata(false));


        /// <summary>
        /// Gets or sets a value indicating whether the edited text has errors; if there are errors the text can't be committed.
        /// </summary>
        public bool HasErrors
        {
            get => this.GetValue<bool>(HasErrorsProperty);
            set => SetValue(HasErrorsProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="HasErrors"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register("HasErrors", typeof(bool), typeof(InPlaceEdit));


        /// <summary>
        /// Gets or sets the text trimming.
        /// </summary>
        /// <value>
        /// The text trimming.
        /// </value>
        public TextTrimming TextTrimming
        {
            get => this.GetValue<TextTrimming>(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="TextTrimming"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(InPlaceEdit), new FrameworkPropertyMetadata(TextTrimming.CharacterEllipsis));


        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get => this.GetValue<TextAlignment>(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="TextAlignment"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(InPlaceEdit), new FrameworkPropertyMetadata(TextAlignment.Left));


        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var template = Template;
            if (template == null)
                return;

            var textBox = _textBox = template.FindName(TextboxPartName, this) as TextBox;

            if (textBox == null)
                return;

            textBox.KeyDown += TextBox_KeyDown;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.TextChanged += TextBox_TextChanged;
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Focusable)
            {
                IsEditing = true;
            }
            else
            {
                // Edit on deferred double click => Only handle mouse clicks if parent is already focused.
                // Since the parent gets the focus before we receive this event do not handle if within double click time.
                if ((_focusableParent != null) && _focusableParent.IsFocused
                    && ((_parentGotFocusTime + _doubleClickTime) < DateTime.Now))
                {
                    // Defer processing so we don't start editing on double clicks.
                    if (!_processingMouseLeftButtonDown)
                    {
                        _processingMouseLeftButtonDown = true;
                        _mouseDoubleClicked = false;
                        _mouseDoubleClickThrottle.Tick();
                    }
                }
            }

            base.OnMouseLeftButtonDown(e);
        }

        private void OnMouseDoubleClickThrottle()
        {
            if (!_mouseDoubleClicked)
            {
                Dispatcher.Invoke(() => IsEditing = true);
            }
            _processingMouseLeftButtonDown = false;
        }

        /// <inheritdoc />
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            _mouseDoubleClicked = true;
            base.OnMouseDoubleClick(e);
        }

        private void Self_Loaded([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            var focusableParent = _focusableParent = this.TryFindAncestorOrSelf<FrameworkElement>(item => item?.Focusable == true);
            if (focusableParent == null)
                return;

            // This is needed to handle the F2 key.
            focusableParent.KeyDown += Parent_KeyDown;
            focusableParent.GotFocus += FocusableParent_GotFocus;
        }

        private void FocusableParent_GotFocus([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            _parentGotFocusTime = DateTime.Now;
        }

        private void Self_Unloaded([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            var focusableParent = _focusableParent;
            if (focusableParent == null)
                return;

            focusableParent.KeyDown -= Parent_KeyDown;
            focusableParent.GotFocus -= FocusableParent_GotFocus;
            _focusableParent = null;
        }

        private void TextBox_KeyDown([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            if (_textBox == null)
                return;

            switch (e.Key)
            {
                case Key.Escape:
                    _textBox.Text = Text ?? string.Empty;
                    IsEditing = false;
                    e.Handled = true;
                    break;

                case Key.Enter:
                    IsEditing = false;
                    e.Handled = true;
                    break;
            }
        }

        private void TextBox_LostFocus([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            IsEditing = false;
        }

        private void TextBox_TextChanged([CanBeNull] object sender, [CanBeNull] TextChangedEventArgs e)
        {
            OnValidate();
        }

        private void OnValidate()
        {
            var eventHandler = Validate;
            if (eventHandler == null)
                return;

            var textBox = _textBox;

            if (textBox == null)
                return;

            var args = new TextValidationEventArgs(textBox.Text);
            eventHandler(this, args);

            HasErrors = args.Action != TextValidationAction.None;

            if (args.Action == TextValidationAction.Undo)
            {
                this.BeginInvoke(() => textBox.Undo());
            }
        }

        private void Parent_KeyDown([CanBeNull] object sender, [NotNull] KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    break;

                case Key.Enter:
                    if (ReferenceEquals(sender, this))
                        break;

                    return;

                default:
                    return;
            }

            IsEditing = true;
            e.Handled = true;
        }

        private bool IsEditing_CoerceValue(bool baseValue)
        {
            if (baseValue)
            {
                return IsEnabled && !IsEditingDisabled;
            }

            return false;
        }

        private void IsEditing_Changed(bool newValue)
        {
            var textBox = _textBox;

            if (textBox == null)
                return;

            if (newValue)
            {
                // Start editing: Set text of text box manually since we need to keep the original text any how.
                textBox.Text = Text ?? string.Empty;
                textBox.SelectAll();
                textBox.Visibility = Visibility.Visible;

                // Subscribe to any mouse action in the hosting window to properly exit editing state
                if (this.Ancestors().LastOrDefault() is FrameworkElement window)
                {
                    window.IsKeyboardFocusWithinChanged += Window_IsKeyboardFocusWithinChanged;
                    window.PreviewMouseDown += Window_PreviewMouseDown;
                    window.PreviewMouseWheel += Window_PreviewMouseWheel;
                }

                PreviewMouseDoubleClick += Self_PreviewMouseDoubleClick;

                // Delay setting the focus to ensure no one else in the call chain grabs it - this would stop editing immediately.
                this.BeginInvoke(() => textBox.Focus());
            }
            else
            {
                // Stop editing: Manually update source if it's valid.
                if (!HasErrors)
                {
                    Text = textBox.Text;
                }

                // Set focus to parent before it will get lost when we hide the TextBox
                if (IsFocused)
                {
                    _focusableParent?.Focus();
                }

                textBox.Visibility = Visibility.Hidden;

                if (this.Ancestors().LastOrDefault() is FrameworkElement window)
                {
                    window.IsKeyboardFocusWithinChanged -= Window_IsKeyboardFocusWithinChanged;
                    window.PreviewMouseDown -= Window_PreviewMouseDown;
                    window.PreviewMouseWheel -= Window_PreviewMouseWheel;
                }

                PreviewMouseDoubleClick -= Self_PreviewMouseDoubleClick;
            }
        }

        private void Self_PreviewMouseDoubleClick([CanBeNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            if (_textBox == null)
                return;

            if (!IsEditing)
                return;

            _textBox.SelectAll();

            e.Handled = true;
        }

        private void Window_IsKeyboardFocusWithinChanged([CanBeNull] object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Equals(e.NewValue, false))
            {
                IsEditing = false;
            }
        }

        private void Window_PreviewMouseDown([CanBeNull] object sender, [CanBeNull] MouseButtonEventArgs e)
        {
            if (_textBox == null)
                return;

            // Stop editing if user clicks outside of control - if the click is not on a focusable element, we don't loose the focus!
            if (!_textBox.IsMouseOver)
            {
                IsEditing = false;
            }
        }

        private void Window_PreviewMouseWheel([CanBeNull] object sender, [CanBeNull] MouseWheelEventArgs e)
        {
            IsEditing = false;
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern int GetDoubleClickTime();
        }
    }
}
