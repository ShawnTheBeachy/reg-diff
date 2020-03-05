using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace RegDiff.UWP.Controls
{
    public sealed class SnackNotification : Control
    {
        private Compositor _compositor;
        private bool _isShown;
        private Border _rootElement;
        private float _targetOffsetY;
        private Vector3KeyFrameAnimation _translateVisibleAnimation;

        #region Dependency properties

        #region DataTemplateSelector

        public DataTemplateSelector DataTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(DataTemplateSelectorProperty);
            set => SetValue(DataTemplateSelectorProperty, value);
        }

        public static readonly DependencyProperty DataTemplateSelectorProperty =
            DependencyProperty.Register("DataTemplateSelector", typeof(DataTemplateSelector), typeof(SnackNotification), new PropertyMetadata(null));

        #endregion DataTemplateSelector

        #endregion Dependency properties

        public SnackNotification()
        {
            DefaultStyleKey = typeof(SnackNotification);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _rootElement = GetTemplateChild("Root") as Border;
            _rootElement.SizeChanged += RootElement_SizeChanged;

            ElementCompositionPreview.SetIsTranslationEnabled(_rootElement, true);

            _translateVisibleAnimation = _compositor.CreateVector3KeyFrameAnimation();
            _translateVisibleAnimation.Duration = TimeSpan.FromMilliseconds(200);
            _translateVisibleAnimation.InsertKeyFrame(1, new Vector3(0));
            _translateVisibleAnimation.Target = nameof(Translation);
        }

        private void RootElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _targetOffsetY = ActualSize.Y;

            if (!_isShown)
            {
                _rootElement.Translation = new Vector3(0, _targetOffsetY, 0);
            }
        }

        public async void Hide()
        {
            _isShown = false;

            var translateInvisibleAnimation = _compositor.CreateVector3KeyFrameAnimation();
            translateInvisibleAnimation.Duration = TimeSpan.FromMilliseconds(200);
            translateInvisibleAnimation.InsertKeyFrame(1, new Vector3(0, _targetOffsetY, 0));
            translateInvisibleAnimation.Target = nameof(Translation);


            _rootElement.StartAnimation(translateInvisibleAnimation);
            await Task.Delay(200);
            _rootElement.Child = null;
        }

        public void Show(object item)
        {
            var template = DataTemplateSelector.SelectTemplate(item);

            if (template != null)
            {
                var content = template.LoadContent() as FrameworkElement;
                content.DataContext = item;
                _rootElement.Child = content;
                UpdateLayout();
                _isShown = true;
                _rootElement.StartAnimation(_translateVisibleAnimation);
            }
        }
    }
}
