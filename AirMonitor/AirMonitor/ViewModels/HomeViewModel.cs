using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AirMonitor.Views;
using Xamarin.Forms;


namespace AirMonitor.ViewModels
{
    public class HomeViewModel
    {
        private readonly INavigation _navigation;

        public HomeViewModel(INavigation navigation)
        {
            _navigation = navigation;
        }

        private ICommand _moveToDetailsCommand;
        public ICommand MoveToDetailsCommand => _moveToDetailsCommand ?? (_moveToDetailsCommand = new Command(GoToDetailsPage_Clicked));

        private void GoToDetailsPage_Clicked()
        {
            _navigation.PushAsync(new DetailsPage());
        }
    }
}

