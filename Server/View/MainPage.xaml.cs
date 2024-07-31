﻿using Server.ViewModel;

namespace Server.View;
public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}
