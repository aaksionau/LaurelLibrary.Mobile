using System.ComponentModel;

namespace LaurelLibrary.Views.Behaviors;

/// <summary>
/// Behavior that applies fade-in animation when IsVisible changes to true
/// </summary>
public class FadeInAnimationBehavior : Behavior<VisualElement>
{
    public static readonly BindableProperty DurationProperty =
        BindableProperty.Create(nameof(Duration), typeof(uint), typeof(FadeInAnimationBehavior), 300u);

    public uint Duration
    {
        get => (uint)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    protected override void OnAttachedTo(VisualElement visualElement)
    {
        visualElement.PropertyChanged += OnVisualElementPropertyChanged;
        base.OnAttachedTo(visualElement);
    }

    protected override void OnDetachingFrom(VisualElement visualElement)
    {
        visualElement.PropertyChanged -= OnVisualElementPropertyChanged;
        base.OnDetachingFrom(visualElement);
    }

    private void OnVisualElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not VisualElement visualElement)
            return;

        if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && visualElement.IsVisible)
        {
            AnimateFadeIn(visualElement);
        }
    }

    private void AnimateFadeIn(VisualElement visualElement)
    {
        visualElement.Opacity = 0;
        visualElement.Animate("FadeIn", v => visualElement.Opacity = v, 0, 1, easing: Easing.CubicOut, length: Duration);
    }
}
