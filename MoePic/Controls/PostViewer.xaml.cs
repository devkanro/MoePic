using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text.RegularExpressions;

using MoePic.Models;

namespace MoePic.Controls
{
    public partial class PostViewer : UserControl
    {
        public PostViewer()
        {
            InitializeComponent();
            selectedType = ImageType.Sample;
        }

        bool IsFav = false;

        public void SetLoad(int id)
        {
            sizeText.Text = String.Format("载入Post {0}中", id);
            typeText.Text = "Loading";
        }

        public void SetPost(MoePost post)
        {
            Post = post;
            IsPostSet = true;
            HistoryHelp.AddPost(Post);
            ImageViewer.Width = 470;
            ImageViewer.Height = 1.0 * 470 / Post.sample_width * Post.sample_height;
            ImageViewer.Background = new ImageBrush() { ImageSource = new ImageSourceConverter().ConvertFromString(CDNHelper.GetCDNUri(Post.preview_url).OriginalString) as ImageSource };
            image.Source = new BitmapImage(CDNHelper.GetCDNUri(Post.sample_url));
            (image.Source as BitmapImage).DownloadProgress += PostViewPage_DownloadProgress;
            (image.Source as BitmapImage).ImageFailed += PostViewPage_ImageFailed;
            (image.Source as BitmapImage).ImageOpened += PostViewer_ImageOpened;
            SetType(GetDefultType());
            IsFav = FavoriteHelp.ContainsPost(post);
            if(IsFav)
            {
                DelFav.Visibility = System.Windows.Visibility.Visible;
                AddFav.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                DelFav.Visibility = System.Windows.Visibility.Collapsed;
                AddFav.Visibility = System.Windows.Visibility.Visible;
            }

            if(Post.source != "" && Post.source != null)
            {
                pixiv.Visibility = System.Windows.Visibility.Visible;
                Uri uri = null;
                if(Uri.IsWellFormedUriString(Post.source, UriKind.Absolute))
                {
                    uri = new Uri(Post.source);
                }
                else
                {
                    pixiv.Visibility = System.Windows.Visibility.Collapsed;
                    goto CONTINUE;
                }
                bool idGet = false;
                //http://www.pixiv.net/member_illust.php?mode=medium&illust_id=47305056

                if (uri.Host.Contains("pixiv"))
                {
                    pLink.Visibility = System.Windows.Visibility.Visible;
                    link.Visibility = System.Windows.Visibility.Collapsed;

                    if (System.IO.Path.GetExtension(Post.source) != "")
                    {
                        Regex regex = new Regex("([0-9]*)");
                        Match match = regex.Match(System.IO.Path.GetFileName(Post.source));
                        if (match.Success)
                        {
                            idGet = true;
                            (App.Current.Resources["RuntimeResources"] as RuntimeResources).PixivID = int.Parse(match.Groups[1].Value);
                        }
                    }
                    else
                    {
                        Regex regex = new Regex("illust_id=([0-9]*)");
                        Match match = regex.Match(uri.Query);
                        if (match.Success)
                        {
                            idGet = true;
                            (App.Current.Resources["RuntimeResources"] as RuntimeResources).PixivID = int.Parse(match.Groups[1].Value);
                        }
                    }
                }
                else
                {
                    pLink.Visibility = System.Windows.Visibility.Collapsed;
                    link.Visibility = System.Windows.Visibility.Visible;
                }
            }

            CONTINUE:
            
            LoadComments();
        }

        public void ReleaseImage()
        {
            if (image != null && image.Source != null)
            {
                (image.Source as System.Windows.Media.Imaging.BitmapImage).UriSource = null;
                image.Source = null;
            }
        }

        public String GetFileSizeText(long @byte)
        {
            if (1.0 * @byte / 1024 > 1024)
            {
                return String.Format("{0:F2}{1}", 1.0 * @byte / 1024 / 1024, "MB");
            }
            else
            {
                return String.Format("{0:F2}{1}", 1.0 * @byte / 1024, "KB");
            }
        }

        public void SetType(ImageType type)
        {
            selectedType = type;
            typeText.Text = selectedType.ToString();
            switch (selectedType)
            {
                case ImageType.Sample:
                    sizeText.Text = String.Format("{0}x{1}", Post.sample_width, Post.sample_height);
                    fileSizeText.Text = GetFileSizeText(Post.sample_file_size == 0 ? (Post.jpeg_file_size == 0 ? Post.file_size : Post.jpeg_file_size) : Post.sample_file_size);
                    break;
                case ImageType.JPG:
                    sizeText.Text = String.Format("{0}x{1}", Post.jpeg_width, Post.jpeg_height);
                    fileSizeText.Text = GetFileSizeText(Post.jpeg_file_size == 0 ? Post.file_size : Post.jpeg_file_size);
                    break;
                case ImageType.PNG:
                    sizeText.Text = String.Format("{0}x{1}", Post.width, Post.height);
                    fileSizeText.Text = GetFileSizeText(Post.file_size);
                    break;
            }
        }

        public MoePost Post { get; set; }

        void PostViewPage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            LoadProgress.Text = MoePic.Resources.AppResources.LoadFail;
        }

        void PostViewPage_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            LoadProgressBar.Value = e.Progress;
            LoadProgress.Text = String.Format(MoePic.Resources.AppResources.LoadPro, e.Progress);
        }

        

        void PostViewer_ImageOpened(object sender, RoutedEventArgs e)
        {
            if (firstLoad)
            {
                firstLoad = false;
                Load.Visibility = System.Windows.Visibility.Collapsed;
                ImageLoad.Begin();
                if(Settings.Current.HideTags)
                {
                    tagsPanel.Opacity = 0;
                    TagMoede = 'S';
                    delete.Visibility = System.Windows.Visibility.Collapsed;
                    hide.Visibility = System.Windows.Visibility.Collapsed;
                    show.Visibility = System.Windows.Visibility.Visible;
                }
                if(!Settings.Current.AutoHideTags)
                {
                    AddTags(Post.tags);
                }
            }
        }

        
        bool firstLoad = true;


        public async void LoadComments()
        {
            List<MoeComment> commentsList = await MoebooruAPI.GetCommentsFormPost(Post.id, MoebooruAPI.partWebsite);
            if(commentsList.Count == 0)
            {
                comments.Visibility = System.Windows.Visibility.Collapsed;
                commentsText.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                commentsList.Reverse();
                foreach (var item in commentsList)
                {
                    comments.Children.Add(new CommentBox() { Comment = item });
                }
                comments.Visibility = System.Windows.Visibility.Visible;
                commentsText.Visibility = System.Windows.Visibility.Visible;
            }
            if (Settings.Current.AutoHideTags)
            {
                AddTags(Post.tags);
            }
            LoadParentsAndChildren();
        }

        int count = 0;

        public async void LoadParentsAndChildren()
        {
            List<MoePost> postList = new List<MoePost>();
            if(Post.parent_id != null)
            {
                var p = await MoebooruAPI.GetPostFormID(int.Parse(Post.parent_id), MoebooruAPI.partWebsite);
                if(p!=null)
                {
                    postList.Add(p);
                }
            }
            if(Post.has_children)
            {
                postList.AddRange(await MoebooruAPI.GetPostsFormParents((int)Post.id, 1, 100, Settings.Current.Rating, MoebooruAPI.partWebsite));
            }
            if (postList.Count == 0)
            {
                parents.Visibility = System.Windows.Visibility.Collapsed;
                parentsText.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                parents.Visibility = System.Windows.Visibility.Visible;
                parentsText.Visibility = System.Windows.Visibility.Visible;

                foreach (var item in postList)
                {
                    if(PostComparer.GetHashCodeStatic(item) != PostComparer.GetHashCodeStatic(Post))
                    {
                        parents.Children.Add(new PostItem(false) { Post = item, Name = String.Format("Item{0}", count++) });
                    }
                }
            }
            LoadNote();
        }

        public async void LoadNote()
        {
            List<MoeNote> noteList = await MoebooruAPI.GetNotesFormPost(Post.id, MoebooruAPI.partWebsite);
            if(noteList.Count != 0)
            {
                noteViewer.Visibility = System.Windows.Visibility.Visible;
                foreach (var item in noteList)
                {
                    NoteItem note = new NoteItem() { Note = item, Height = 1.0 * 470 / Post.width * item.height, Width = 1.0 * 470 / Post.width * item.width };
                    note.Click += note_Click;
                    Canvas.SetLeft(note, 1.0 * 470 / Post.width  * item.x);
                    Canvas.SetTop(note, 1.0 * ImageViewer.Height / Post.height  * item.y);
                    notePanel.Children.Add(note);
                }
                changeTagAndNote_Click(changeTagAndNote, new RoutedEventArgs());
            }
            else
            {
                noteViewer.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        bool noteTextShowing = false;

        void note_Click(object sender, RoutedEventArgs e)
        {
            if(noteTextShowing)
            {
                NoteTextHide.Begin();
                foreach (var item in notePanel.Children)
                {
                    item.Visibility = System.Windows.Visibility.Visible;
                }
                noteTextShowing = false;
            }
            else
            {
                noteText.Text = (sender as NoteItem).Note.body;
                noteText.Margin = new Thickness(Canvas.GetLeft((sender as NoteItem)), Canvas.GetTop((sender as NoteItem)) + (sender as NoteItem).Height, 0, 0);
                noteText.MaxWidth = 470 - Canvas.GetLeft((sender as NoteItem));
                NoteTextShow.Begin();
                foreach (var item in notePanel.Children)
                {
                    if(item != sender)
                    {
                        item.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                noteTextShowing = true;
            }
        }

        Dictionary<String, List<MoeTag>> TagDownloadList = new Dictionary<string, List<MoeTag>>();

        public int zindex = 0;

        public async System.Threading.Tasks.Task<MoeTag> GetTag (String tagName)
        {
            var tagsList = await MoebooruAPI.GetTagsFormName(tagName, 0, 1, TagOrder.name, MoebooruAPI.partWebsite);
            return tagsList.Find(t => t.name == tagName);
        }

        public async void AddTags(String tagsString)
        {
            Random r = new Random();
            string[] tags = tagsString.Split(' ');
            int i = 0;

            if(Settings.Current.AutoHideTags)
            {
                int max = (int)(1.0 * tagsPanel.ActualHeight / 48);
                IEnumerable<System.Threading.Tasks.Task<MoeTag>> tasks;
                List<MoeTag> tagQueue = new List<MoeTag>();


                tasks = from tag in tags select GetTag(tag);

                var tagsResult = await System.Threading.Tasks.Task.WhenAll(tasks);

                foreach (var item in tagsResult)
                {
                    if (item.type != (int)TagType.General)
                    {
                        TagItem tag = new TagItem(item, this)
                        {
                            Margin = new Thickness(0, 0, r.Next((int)ImageViewer.Width), r.Next((int)ImageViewer.Height)),
                            Name = String.Format("Tag{0}", i++),
                        };
                        Canvas.SetZIndex(tag, zindex++);
                        tagsPanel.Children.Add(tag);
                        if (i == max)
                        {
                            break;
                        }
                    }
                    else
                    {
                        tagQueue.Add(item);
                    }
                }

                if(i < max)
                {
                    var tagsList = tagQueue.OrderByDescending(t => t.count).ToList();
                    foreach (var item in tagsList)
                    {
                        TagItem tag = new TagItem(item, this)
                        {
                            Margin = new Thickness(0, 0, r.Next((int)ImageViewer.Width), r.Next((int)ImageViewer.Height)),
                            Name = String.Format("Tag{0}", i++),
                        };
                        Canvas.SetZIndex(tag, zindex++);
                        tagsPanel.Children.Add(tag);

                        if (i == max)
                        {
                            break;
                        }
                    }
                }

                if(TagMoede == 'S')
                {
                    tagsPanel.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                foreach (var item in tags)
                {
                    TagItem tag = new TagItem(item, this)
                    {
                        Margin = new Thickness(0, 0, r.Next((int)ImageViewer.Width), r.Next((int)ImageViewer.Height)),
                        Name = String.Format("Tag{0}", i++),
                    };
                    Canvas.SetZIndex(tag, zindex++);
                    tagsPanel.Children.Add(tag);
                }
            }
        }

        public char TagMoede = 'H';
        bool noClick = false;


        public Color ButtonColor
        {
            get { return (Color)GetValue(ButtonColorProperty); }
            set { SetValue(ButtonColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonColorProperty =
            DependencyProperty.Register("ButtonColor", typeof(Color), typeof(PostViewer), new PropertyMetadata(Color.FromArgb(0xff,0x58,0xac,0xed)));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(IsTagShow)
            {
                if (!noClick)
                {
                    if (TagMoede == 'H')
                    {
                        HideTags.Begin();
                        TagMoede = 'S';
                        delete.Visibility = System.Windows.Visibility.Collapsed;
                        hide.Visibility = System.Windows.Visibility.Collapsed;
                        show.Visibility = System.Windows.Visibility.Visible;
                    }
                    else if (TagMoede == 'S')
                    {
                        tagsPanel.Visibility = System.Windows.Visibility.Visible;
                        ShownTags.Begin();
                        TagMoede = 'H';
                        delete.Visibility = System.Windows.Visibility.Collapsed;
                        hide.Visibility = System.Windows.Visibility.Visible;
                        show.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        TagMoede = 'H';
                        delete.Visibility = System.Windows.Visibility.Collapsed;
                        hide.Visibility = System.Windows.Visibility.Visible;
                        show.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                else
                {
                    noClick = false;

                }
            }
            else
            {
                if (TagMoede == 'H')
                {
                    HideNotes.Begin();
                    TagMoede = 'S';
                    delete.Visibility = System.Windows.Visibility.Collapsed;
                    hide.Visibility = System.Windows.Visibility.Collapsed;
                    show.Visibility = System.Windows.Visibility.Visible;
                }
                else if (TagMoede == 'S')
                {
                    noteViewer.Visibility = System.Windows.Visibility.Visible;
                    ShownNotes.Begin();
                    TagMoede = 'H';
                    delete.Visibility = System.Windows.Visibility.Collapsed;
                    hide.Visibility = System.Windows.Visibility.Visible;
                    show.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    TagMoede = 'H';
                    delete.Visibility = System.Windows.Visibility.Collapsed;
                    hide.Visibility = System.Windows.Visibility.Visible;
                    show.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            
        }

        private void Button_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(IsTagShow)
            {
                if (TagMoede == 'H')
                {
                    noClick = true;
                    TagMoede = 'D';
                    delete.Visibility = System.Windows.Visibility.Visible;
                    hide.Visibility = System.Windows.Visibility.Collapsed;
                    show.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
        DownloadTask task;
        bool IsDownloading = false;

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            if(!IsDownloading && IsPostSet)
            {
                IsDownloading = true;
                task = new DownloadTask(Post, selectedType);
                task.DownloadProgressChanged += task_DownloadProgressChanged;
                task.DownloadCompleted += task_DownloadComplete;
                DownChange.Begin();
                DownloadTaskManger.AddDownload(task);
                ToastService.Show(new Uri(task.Post.preview_url), "已加入下载队列.", (s, a) => { NavigationService.Navigate("DownListPage.xaml", 0); }, null, null);
            }
        }


        public Boolean IsPostSet = false;

        void task_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            if (DownPro != null)
            {
                DownPro.Text = String.Format("{0}%", e.ProgressPercentage);
            }
        }

        void task_DownloadComplete(object sender, EventArgs e)
        {
            if (DownPro != null)
            {
                DownPro.Text = "完成";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (IsPostSet)
            {

                NavigationService.Navigate("ImageViewPage.xaml", new KeyValuePair<MoePic.Models.MoePost, MoePic.Models.ImageType>(Post, selectedType));
            }
        }

        ImageType selectedType;
        List<ImageType> ableType;

        ImageType GetDefultType()
        {
            if(Post.sample_url == Post.file_url)
            {
                ableType = new List<ImageType> { ImageType.JPG };
                return ImageType.JPG;
            }
            else if(Post.jpeg_url == Post.file_url)
            {
                ableType = new List<ImageType> { ImageType.Sample, ImageType.JPG };
                switch (Models.Settings.Current.DefaultType)
                {
                    case ImageType.Sample:
                        return ImageType.Sample;
                    case ImageType.PNG:
                        return ImageType.JPG;
                    case ImageType.JPG:
                        return ImageType.JPG;
                    default:
                        break;
                }
                return ImageType.JPG;
            }
            else
            {
                ableType = new List<ImageType> { ImageType.Sample, ImageType.JPG, ImageType.PNG };
                return Models.Settings.Current.DefaultType;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (IsPostSet)
            {
                if (ableType.Count == 1)
                {
                    TypeCanChange.Begin();

                }
                else
                {
                    TypeChange1.Begin();
                }
            }
        }

        private void TypeChange1_Completed(object sender, EventArgs e)
        {
            int index = ableType.IndexOf(selectedType) + 1;
            if(index > ableType.Count - 1)
            {
                index = 0;
            }
            SetType(ableType[index]);
            TypeChange2.Begin();
        }

        private void DownChange_Completed(object sender, EventArgs e)
        {
            DownIcon.Visibility = System.Windows.Visibility.Collapsed;
            DownPro.Visibility = System.Windows.Visibility.Visible;
            DownChange2.Begin();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if(IsPostSet)
            {
                if (IsFav)
                {
                    IsFav = false;
                    FavoriteHelp.DelFavorite(Post);
                    ToastService.Show(new Uri(Post.preview_url), MoePic.Resources.AppResources.DelFav);
                }
                else
                {
                    IsFav = true;
                    FavoriteHelp.AddFavorite(Post);
                    ToastService.Show(new Uri(Post.preview_url), MoePic.Resources.AppResources.AddFav, (s, a) => { NavigationService.Navigate("FavoritePage.xaml"); }, null, null);
                }
                FavChange.Begin();
            }
        }

        private void FavChange_Completed(object sender, EventArgs e)
        {
            if (IsFav)
            {
                DelFav.Visibility = System.Windows.Visibility.Visible;
                AddFav.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                DelFav.Visibility = System.Windows.Visibility.Collapsed;
                AddFav.Visibility = System.Windows.Visibility.Visible;
            }
            FavChange2.Begin();
        }

        bool IsTagShow = true;

        private void TagChange_Completed(object sender, EventArgs e)
        {
            if(IsTagShow)
            {
                IsTagShow = false;
                tagIcon.Visibility = System.Windows.Visibility.Collapsed;
                NoteIcon.Visibility = System.Windows.Visibility.Visible;
                HideTags.Begin();
                noteViewer.Visibility = System.Windows.Visibility.Visible;
                ShownNotes.Begin();
            }
            else
            {
                IsTagShow = true;
                tagIcon.Visibility = System.Windows.Visibility.Visible;
                NoteIcon.Visibility = System.Windows.Visibility.Collapsed;
                HideNotes.Begin();
                tagsPanel.Visibility = System.Windows.Visibility.Visible;
                ShownTags.Begin();
            }
            TagChange2.Begin();
        }

        private void HideTags_Completed(object sender, EventArgs e)
        {
            tagsPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void HideNotes_Completed(object sender, EventArgs e)
        {
            noteViewer.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void changeTagAndNote_Click(object sender, RoutedEventArgs e)
        {
            if(IsPostSet)
            {
                if (notePanel.Children.Count != 0)
                {
                    TagChange.Begin();
                }
                else
                {
                    TagCanChange.Begin();
                }
            }
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if(commentBody.Text == "")
            {
                ToastService.Show(MoePic.Resources.AppResources.CommentNoBody);
            }
            else
            {
                if (MoebooruAPI.partWebsite.Contains("yande"))
                {
                    if(Settings.Current.UserY == null || Settings.Current.UserYHash == null)
                    {
                        ToastService.Show(MoePic.Resources.AppResources.NotLoginY);
                        return;
                    }
                }
                else
                {
                    if (Settings.Current.UserK == null || Settings.Current.UserKHash == null)
                    {
                        ToastService.Show(MoePic.Resources.AppResources.NotLoginK);
                        return;
                    }
                }
                nowSending.Visibility = System.Windows.Visibility.Visible;
                bool ok = await MoebooruAPI.CreateComment(Post.id, commentBody.Text, false, MoebooruAPI.partWebsite);
                if(ok)
                {
                    MoeComment comment = new MoeComment()
                    {
                        body = commentBody.Text,
                        created_at = DateTime.Now,
                    };
                    if (MoebooruAPI.partWebsite.Contains("yande"))
                    {
                        comment.creator = Settings.Current.UserY.name;
                        comment.creator_id = Settings.Current.UserY.id;
                    }
                    else
                    {
                        comment.creator = Settings.Current.UserK.name;
                        comment.creator_id = Settings.Current.UserK.id;
                    }
                    commentsText.Visibility = System.Windows.Visibility.Visible;
                    comments.Visibility = System.Windows.Visibility.Visible;
                    comments.Children.Add(new CommentBox() { Comment = comment });
                    commentBody.Text = "";
                    nowSending.Visibility = System.Windows.Visibility.Collapsed;
                    ToastService.Show(MoePic.Resources.AppResources.SendCommentOk);
                }
                else
                {
                    nowSending.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBoxService.Show(MoePic.Resources.AppResources.SendCommentFailTile, MoePic.Resources.AppResources.SendCommentFailContent, true, new Command(MoePic.Resources.AppResources.Cancel, null));
                }
            }
        }

        private void ImageLoad_Completed(object sender, EventArgs e)
        {
            image.Opacity = 1;
        }

        private async void pixiv_Click(object sender, RoutedEventArgs e)
        {
            if(link.Visibility == System.Windows.Visibility.Collapsed)
            {
                Uri uri = new Uri(String.Format("pixiver://Illust/?id={0}", (App.Current.Resources["RuntimeResources"] as RuntimeResources).PixivID), UriKind.Absolute);
                var ok = await Windows.System.Launcher.LaunchUriAsync(uri);
                if(!ok)
                {
                    Uri uri2 = new Uri(Post.source);
                    Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask()
                    {
                        Uri = uri2
                    };
                    task.Show();
                }
            }
            else
            {
                Uri uri = new Uri(Post.source);
                Microsoft.Phone.Tasks.WebBrowserTask task = new Microsoft.Phone.Tasks.WebBrowserTask()
                {
                    Uri = uri
                };
                task.Show();
            }
        }
    }
}
