using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Spectrum;
using NGravatar;
using Color = System.Windows.Media.Color;

namespace Q3Client
{
    /// <summary>
    /// Interaction logic for Avatar.xaml
    /// </summary>
    public partial class Avatar : UserControl
    {
        private readonly User user;

        public Avatar(User user)
        {
            this.user = user;
            InitializeComponent();

            var initials = GetInitials();

            InitialsLabel.Content = initials;

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(user.EmailAddress));
            double hue = hash[0];
            hue = hue / 256d * 360d;

            var hsl = new Spectrum.Color.HSL(hue, 0.8d, 0.3d);
            var rgb = hsl.ToRGB();
            InitialsLabel.Background = new SolidColorBrush(Color.FromRgb(rgb.R, rgb.G, rgb.B));
            InitialsLabel.Foreground = new SolidColorBrush(Colors.White);            
            
            var image = new BitmapImage(new Uri(new Gravatar().GetUrl(user.EmailAddress, 24, GravatarRating.G, "blank" )), new RequestCachePolicy(RequestCacheLevel.Default));
            AvatarImage.Source = image;
            AvatarImage.ToolTip = user.FullName;

        }

        private string GetInitials()
        {
            var initials = user.EmailAddress.Substring(0,1);
            var dot = user.EmailAddress.IndexOf(".");
            if (dot > 0 && dot < user.EmailAddress.IndexOf("@"))
            {
               initials += user.EmailAddress[dot + 1];
            }

            return initials.ToUpper();
        }
    }
}
