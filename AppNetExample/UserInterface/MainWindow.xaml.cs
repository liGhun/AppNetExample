using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AppNetDotNet.ApiCalls;
using AppNetDotNet.Model;

namespace RundApp.UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            Tuple<List<Post>, ApiCallResponse> posts_response = SimpleStreams.getUnifiedStream(Properties.Settings.Default.access_token);
            // the response is a general comcept in the API: A tuple containing the data (in this case a list of posts) and an "ApiCallResponse" with the metadata
            if (posts_response.Item2.success)
            {
                listview_items.ItemsSource = posts_response.Item1;
                // of course this is very basic as we just show the ToString() method
            }
        }
    }
}
