using System.Windows.Controls;
using System.Windows.Input;

namespace HearthStoneSim.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CardView : UserControl
    {
        public Card CardSource { get; set; }
        public ucCard()
        {
            InitializeComponent();
            CardSource = new card {name="beast", cost=3, attack=5, health=2};
            tbName.Text = CardSource.name;
            tbCost.Text = CardSource.cost.ToString();
            tbAttack.Text = CardSource.attack.ToString();
            tbHealth.Text = CardSource.health.ToString();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            //MessageBox.Show("mouse down");
        }
    }
}
