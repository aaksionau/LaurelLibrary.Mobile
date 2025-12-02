namespace LaurelLibrary.Views.Behaviors;

/// <summary>
/// Behavior that applies a pulsing animation to a view to draw attention
/// </summary>
public class PulseAnimationBehavior : Behavior<VisualElement>
{
    protected override void OnAttachedTo(VisualElement visualElement)
    {
        StartPulseAnimation(visualElement);
        base.OnAttachedTo(visualElement);
    }

    protected override void OnDetachingFrom(VisualElement visualElement)
    {
        visualElement.AbortAnimation("PulseAnimation");
        base.OnDetachingFrom(visualElement);
    }

    private static void StartPulseAnimation(VisualElement visualElement)
    {
        var animation = new Animation();
        
        // Scale up
        animation.Add(0, 0.5, new Animation(v => visualElement.Scale = 1 + (v * 0.15), 1, 1.15));
        // Scale down
        animation.Add(0.5, 1, new Animation(v => visualElement.Scale = 1.15 - ((v - 0.5) * 0.3), 1.15, 1));

        // Opacity pulse
        var opacityAnimation = new Animation();
        opacityAnimation.Add(0, 0.5, new Animation(v => visualElement.Opacity = 1 - (v * 0.3), 1, 0.7));
        opacityAnimation.Add(0.5, 1, new Animation(v => visualElement.Opacity = 0.7 + ((v - 0.5) * 0.6), 0.7, 1));

        animation.Add(0, 1, opacityAnimation);

        animation.Commit(visualElement, "PulseAnimation", 16, 1500, Easing.Linear, (v, c) =>
        {
            if (visualElement.IsEnabled)
            {
                StartPulseAnimation(visualElement);
            }
        });
    }
}
