﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AirMonitor.Views.Controls
{

    public partial class ContentWidthHeader : StackLayout
    {
        
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(ContentWidthHeader),
            defaultValue: null
            );


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty ControlContentProperty = BindableProperty.Create(
           propertyName: nameof(ControlContent),
           returnType: typeof(View),
           declaringType: typeof(ContentWidthHeader),
           defaultValue: null
           );


        public View ControlContent
        {
            get { return (View)GetValue(ControlContentProperty); }
            set { SetValue(ControlContentProperty, value); }
        }

        public ContentWidthHeader()
        {
            InitializeComponent();
        }
    }
}