using RegDiff.Core;
using RegDiff.Core.Models;
using RegDiff.Core.Parsing;
using RegDiff.UWP.Models;
using RegDiff.UWP.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RegDiff.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        #region Keys

        public IList<RegistryKey> Keys
        {
            get => (IList<RegistryKey>)GetValue(KeysProperty);
            set => SetValue(KeysProperty, value);
        }

        public static readonly DependencyProperty KeysProperty =
            DependencyProperty.Register("Keys", typeof(IList<RegistryKey>), typeof(MainPage), new PropertyMetadata(null));

        #endregion Keys

        #region DiffingProgressModel

        public ProcessingProgressViewModel DiffingProgressViewModel
        {
            get => (ProcessingProgressViewModel)GetValue(DiffingProgressViewModelProperty);
            set => SetValue(DiffingProgressViewModelProperty, value);
        }

        public static readonly DependencyProperty DiffingProgressViewModelProperty =
            DependencyProperty.Register("DiffingProgressViewModel", typeof(ProcessingProgressViewModel), typeof(MainPage), new PropertyMetadata(null));

        #endregion DiffingProgressModel

        #region SelectedValue

        public BaseRegistryValue SelectedValue
        {
            get => (BaseRegistryValue)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(BaseRegistryValue), typeof(MainPage), new PropertyMetadata(null));

        #endregion SelectedValue

        #region ViewModelA

        public RegistryTreeViewModel ViewModelA
        {
            get => (RegistryTreeViewModel)GetValue(ViewModelAProperty);
            set => SetValue(ViewModelAProperty, value);
        }

        public static readonly DependencyProperty ViewModelAProperty =
            DependencyProperty.Register("ViewModelA", typeof(RegistryTreeViewModel), typeof(MainPage), new PropertyMetadata(null));

        #endregion ViewModelA

        #region ViewModelB

        public RegistryTreeViewModel ViewModelB
        {
            get => (RegistryTreeViewModel)GetValue(ViewModelBProperty);
            set => SetValue(ViewModelBProperty, value);
        }

        public static readonly DependencyProperty ViewModelBProperty =
            DependencyProperty.Register("ViewModelB", typeof(RegistryTreeViewModel), typeof(MainPage), new PropertyMetadata(null));

        #endregion ViewModelB

        private int _filesProcessing = 0;

        public MainPage()
        {
            InitializeComponent();

            ViewModelA = new RegistryTreeViewModel();
            ViewModelB = new RegistryTreeViewModel();
        }

        private async void FilePicker_FileProcessingEnded(object _,
                                                          RoutedEventArgs __)
        {
            _filesProcessing--;

            if (_filesProcessing < 1)
            {
                StartFileProcessingDoneAnimations();

                if (ViewModelA.Data != null && ViewModelB.Data != null)
                {
                    var progress = new Progress<ProgressIncrement>();
                    DiffingProgressViewModel = new ProcessingProgressViewModel(progress);
                    DiffingProgressPanel.Visibility = Visibility.Visible;
                    Keys = await new Parser().ParseAsync(ViewModelA.Data, ViewModelB.Data, progress);
                    DiffingProgressPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void FilePicker_FileProcessingStarted(object _,
                                                      RoutedEventArgs __)
        {
            _filesProcessing++;

            if (_filesProcessing == 1)
            {
                StartFileProcessingAnimations();
            }

            Keys = null;
        }

        private void HideItemMenuItem_Click(object sender,
                                            RoutedEventArgs _)
        {
            var item = (sender as FrameworkElement).DataContext as BaseRegistryModel;
            item.IsVisible = false;
            BottomSnack.Show(new UndoHideItemModel
            {
                Item = item
            });
        }

        private void HideNoDifferencesButton_Click(object _, 
                                                   RoutedEventArgs __)
        {
            void HideKeysWithNoDifferences(RegistryKey key)
            {
                if (!key.HasDifference)
                {
                    key.IsVisible = false;
                }

                foreach (var k in key.Keys)
                {
                    HideKeysWithNoDifferences(k);
                }

                foreach (var value in key.Values)
                {
                    if (!value.HasDifference)
                    {
                        value.IsVisible = false;
                    }
                }
            }

            foreach (var key in Keys)
            {
                HideKeysWithNoDifferences(key);
            }
        }

        private void ResultTreeView_ItemInvoked(Microsoft.UI.Xaml.Controls.TreeView _,
                                                Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is BaseRegistryValue regValue)
            {
                SelectedValue = regValue;
            }
        }

        private void StartFileProcessingAnimations()
        {
            FileProcessingPickerOffset.StartAnimation();
            FileProcessingHeaderOffset.StartAnimation();
            FileProcessingPanelAOffset.StartAnimation();
            FileProcessingPanelBOffset.StartAnimation();
        }

        private void StartFileProcessingDoneAnimations()
        {
            FileProcessingDonePickerOffset.StartAnimation();
            FileProcessingDoneHeaderOffset.StartAnimation();
            FileProcessingDonePanelAOffset.StartAnimation();
            FileProcessingDonePanelBOffset.StartAnimation();
        }

        private void UndoButton_Click(object _, 
                                      RoutedEventArgs __)
        {
            BottomSnack.Hide();
        }
    }
}
