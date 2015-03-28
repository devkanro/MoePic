using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

using MoePic.Models;

namespace MoePic
{
    public partial class SearchPage : Models.MoePicPage
    {
        public SearchPage()
        {
            InitializeComponent();
            textInput.InitSuggestionsProvider(this.provider);
            provider.InputChanged += provider_InputChanged;
        }

        private WebServiceAutoCompleteProvider provider = new WebServiceAutoCompleteProvider();
        private bool requestMade = false;
        private string lastRequestString = null;

        async void provider_InputChanged(object sender, EventArgs e)
        {
            if (this.requestMade)
            {
                return;
            }

            this.lastRequestString = this.provider.InputString;

            string inputString = this.provider.InputString;
            if (!string.IsNullOrEmpty(inputString))
            {
                List<MoePic.Models.MoeTag> TagList;
                TagList = await MoePic.Models.MoebooruAPI.GetTagsFormName(lastRequestString);
                this.requestMade = false;

                this.provider.LoadSuggestions(TagList);

                if (this.lastRequestString != this.provider.InputString)
                {
                    this.provider_InputChanged(this.textInput, EventArgs.Empty);
                }
            }
            else
            {
                this.provider.LoadSuggestions(new List<string>());
            }

            if (textInput.Text == "")
            {
                this.provider.LoadSuggestions(Models.HistoryHelp.SearchHistory.Select((s) => { return new Models.MoeTag() { name = s }; }));
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            MoePic.Controls.StatusBarView view = new MoePic.Controls.StatusBarView(Controls.Logos.Search, MoePic.Resources.AppResources.Search, false, true);
            MoePic.Models.StatusBarService.Init(view);
            MoePic.Models.StatusBarService.Change(view);
            if(e.NavigationMode == NavigationMode.New)
            {
                if (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) != null)
                {
                    textInput.Text = (MoePic.Models.NavigationService.GetNavigateArgs(NavigationContext) as String);
                    Models.HistoryHelp.AddSearch(textInput.Text);
                    postViewer.LoadStart();
                    List<MoePic.Models.MoePost> PostList = await MoePic.Models.MoebooruAPI.GetPostsFromTags(textInput.Text, page++, MoePic.Models.Settings.Current.Limit, MoePic.Models.Settings.Current.Rating, MoePic.Models.MoebooruAPI.partWebsite);
                    foreach (var item in PostList)
                    {
                        postViewer.AddPost(item);
                    }
                    postViewer.LoadOver();
                }
            }
            
            base.OnNavigatedTo(e);
        }


        public int page = 1;

        public void Clear()
        {
            postViewer.Clear();
            page = 1;
        }

        private async void textInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Clear();
                postViewer.Focus();
                postViewer.LoadStart();
                Models.HistoryHelp.AddSearch(textInput.Text);
                List<MoePic.Models.MoePost> PostList = await MoePic.Models.MoebooruAPI.GetPostsFromTags(textInput.Text, page++, MoePic.Models.Settings.Current.Limit, MoePic.Models.Settings.Current.Rating);
                foreach (var item in PostList)
                {
                    postViewer.AddPost(item);
                }
                postViewer.LoadOver();
            }
        }

        private async void textInput_SuggestionSelected(object sender, SuggestionSelectedEventArgs e)
        {
            Clear();
            postViewer.Focus();
            postViewer.LoadStart();
            Models.HistoryHelp.AddSearch(textInput.Text);
            List<MoePic.Models.MoePost> PostList = await MoePic.Models.MoebooruAPI.GetPostsFromTags(textInput.Text, page++, MoePic.Models.Settings.Current.Limit, MoePic.Models.Settings.Current.Rating);
            foreach (var item in PostList)
            {
                postViewer.AddPost(item);
            }
            postViewer.LoadOver();
        }

        private async System.Threading.Tasks.Task<bool> postViewer_RequestLoadData(object sender, EventArgs e)
        {
            postViewer.LoadStart();
            List<MoePic.Models.MoePost> PostList = await MoePic.Models.MoebooruAPI.GetPostsFromTags(textInput.Text, page++, MoePic.Models.Settings.Current.Limit, MoePic.Models.Settings.Current.Rating);
            foreach (var item in PostList)
            {
                postViewer.AddPost(item);
            }
            postViewer.LoadOver();
            return true;
        }

        private void textInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if(textInput.Text == "")
            {
                this.provider.LoadSuggestions(Models.HistoryHelp.SearchHistory.Select((s) => { return new Models.MoeTag() { name = s }; }));
            }
        }

        
    }
    public class TagStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Regex.Replace((value as String).Replace('_', ' '), "\\w+", match => match.Value[0].ToString().ToUpper() + match.Value.Substring(1));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as String).ToLower().Replace(' ', '_');
        }
    }
}