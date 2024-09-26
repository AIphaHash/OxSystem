using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace OxSystem
{
    internal class ButtonStyles
    {
        public static Style CreateAnimatedButtonStyle()
        {
            var style = new Style(typeof(Button));

            // Define the storyboard for the click animation
            var clickAnimationStoryboard = new Storyboard();

            var scaleTransform = new ScaleTransform();
            var scaleTransformSetter = new Setter
            {
                Property = UIElement.RenderTransformProperty,
                Value = scaleTransform
            };

            style.Setters.Add(scaleTransformSetter);

            var scaleTransformXAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.9,
                Duration = new Duration(TimeSpan.FromSeconds(0.1)),
                AutoReverse = true
            };
            Storyboard.SetTarget(scaleTransformXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleTransformXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
            clickAnimationStoryboard.Children.Add(scaleTransformXAnimation);

            var scaleTransformYAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.9,
                Duration = new Duration(TimeSpan.FromSeconds(0.1)),
                AutoReverse = true
            };
            Storyboard.SetTarget(scaleTransformYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleTransformYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));
            clickAnimationStoryboard.Children.Add(scaleTransformYAnimation);

            // Create the ControlTemplate for the button
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.Name = "buttonGrid";
            gridFactory.SetValue(Grid.RenderTransformOriginProperty, new Point(0.5, 0.5));

            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "buttonBorder";
            borderFactory.SetValue(Border.BorderBrushProperty, Brushes.Transparent);
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(2));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(15));
            borderFactory.SetValue(Border.BackgroundProperty, new LinearGradientBrush(
                Color.FromArgb(255, 19, 143, 158),
                Color.FromArgb(255, 134, 202, 210),
                0.5
            ));

            var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenterFactory);
            gridFactory.AppendChild(borderFactory);

            var scaleTransformFactory = new FrameworkElementFactory(typeof(ScaleTransform));
            scaleTransformFactory.Name = "buttonScaleTransform";
            scaleTransformFactory.SetValue(ScaleTransform.ScaleXProperty, 1.0);
            scaleTransformFactory.SetValue(ScaleTransform.ScaleYProperty, 1.0);

            gridFactory.SetValue(Grid.RenderTransformProperty, scaleTransformFactory);

            var template = new ControlTemplate(typeof(Button))
            {
                VisualTree = gridFactory
            };

            style.Setters.Add(new Setter(Control.TemplateProperty, template));

            // Add event triggers for mouse events
            var mouseEnterTrigger = new EventTrigger(Button.MouseEnterEvent)
            {
                Actions = { new BeginStoryboard { Storyboard = clickAnimationStoryboard } }
            };
            var mouseLeaveTrigger = new EventTrigger(Button.MouseLeaveEvent)
            {
                Actions = { new BeginStoryboard { Storyboard = clickAnimationStoryboard } }
            };

            style.Triggers.Add(mouseEnterTrigger);
            style.Triggers.Add(mouseLeaveTrigger);

            return style;
        }
    }
}
