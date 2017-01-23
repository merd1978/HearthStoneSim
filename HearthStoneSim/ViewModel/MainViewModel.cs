using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight;
using GongSolutions.Wpf.DragDrop;
using HearthStoneSim.Model;

namespace HearthStoneSim.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IDropTarget
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Get the window title (name and assembly version)
        /// </summary>
        public string MainWindowTitle { get; private set; }

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }
            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.NotHandled = true;
        }

        public void Drop(IDropInfo dropInfo)
        {
            dropInfo.NotHandled = true;
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    WelcomeTitle = item.Title;
                });

            Version deploy = Assembly.GetExecutingAssembly().GetName().Version;
            MainWindowTitle = $"HearthStoneSim v{deploy.Major}.{deploy.Minor}.{deploy.Build}";
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}