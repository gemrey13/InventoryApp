using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InventoryApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnVoterReg_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //registerVoter.Activate();
            //registerVoter.Show();
        }

        private void btnHouseKeeping_Click(object sender, RoutedEventArgs e)
        {
            if (txtTemp.Text.Trim() == "1")
            {
                //RegisterUser userWindow = new RegisterUser();
                //userWindow.Show();
                this.Hide();
            }
            else
            {
                txtUserManagement.Text = "Action not authorized";
                imgHouseKeeping.Source = new BitmapImage(new Uri(@"/images/notallowed.png", UriKind.Relative));
            }
        }

        private void btnElec_Click(object sender, RoutedEventArgs e)
        {
            if (txtTemp.Text.Trim() == "1")
            {
                // RegisterUser userWindow = new RegisterUser();
                // userWindow.Show();
                // this.Hide();
            }
            else
            {
                txtVotingProper.Text = "Action not authorized";
                imgVotingProper.Source = new BitmapImage(new Uri(@"/images/notallowed.png", UriKind.Relative));
            }
        }

        private void btnCandReg_Click(object sender, RoutedEventArgs e)
        {
            //this.hide();
            //registercandidate candidate = new registercandidate();
            //candidate.show();
        }
    }
}