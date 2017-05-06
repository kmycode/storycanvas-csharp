﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewModels;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class SlotManagementPage : ContentPage
	{
		public SlotManagementPage()
		{
			InitializeComponent();
			this.BindingContext = StoryViewModel.Default;
		}
	}
}
