using Avalonia.Rendering.Composition;
using ImplicitAnimationCollection = Avalonia.Rendering.Composition.Animations.ImplicitAnimationCollection;
using Visual = Avalonia.Visual;

namespace BD.WTTS.UI.Styling;

public static class Animations
{
    private static ImplicitAnimationCollection? _implicitAnimations;

    private static void EnsureImplicitAnimations(Visual visual, bool isRotate = true)
    {
        if (_implicitAnimations == null)
        {
            var compositor = ElementComposition.GetElementVisual(visual)!.Compositor;

            var animationGroup = compositor.CreateAnimationGroup();

            var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.Target = "Offset";
            offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(400);
            animationGroup.Add(offsetAnimation);

            if (isRotate)
            {
                var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
                rotationAnimation.Target = "RotationAngle";
                rotationAnimation.InsertKeyFrame(.5f, 0.160f);
                rotationAnimation.InsertKeyFrame(1f, 0f);
                rotationAnimation.Duration = TimeSpan.FromMilliseconds(400);
                animationGroup.Add(rotationAnimation);
            }

            _implicitAnimations = compositor.CreateImplicitAnimationCollection();
            _implicitAnimations["Offset"] = animationGroup;
        }
    }

    public static void SetEnableOffsetAnimations(Border border, bool value)
    {
        var page = border.GetVisualParent();
        if (page == null)
        {
            border.AttachedToVisualTree += (sender, e) => { SetEnableOffsetAnimations(border, value); };
            return;
        }

        if (ElementComposition.GetElementVisual(page) == null)
            return;

        EnsureImplicitAnimations(page, false);

        if (border.GetVisualParent() is Visual visualParent
            && ElementComposition.GetElementVisual(visualParent) is CompositionVisual compositionVisual)
        {
            compositionVisual.ImplicitAnimations = _implicitAnimations;
        }
    }

    public static void SetEnableAnimations(Border border, bool value)
    {
        var page = border.GetVisualParent();
        if (page == null)
        {
            border.AttachedToVisualTree += (sender, e) => { SetEnableAnimations(border, value); };
            return;
        }

        if (ElementComposition.GetElementVisual(page) == null)
            return;

        EnsureImplicitAnimations(page);

        if (border.GetVisualParent() is Visual visualParent
            && ElementComposition.GetElementVisual(visualParent) is CompositionVisual compositionVisual)
        {
            compositionVisual.ImplicitAnimations = _implicitAnimations;
        }
    }
}
