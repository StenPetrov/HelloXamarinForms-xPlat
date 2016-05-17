using Xamarin.Forms;
using Xamarin.Forms.Pages;

namespace HelloForms
{
    public partial class EasyTablesPage : ListDataPage
    {
        public EasyTablesPage ()
        {
            InitializeComponent (); 
        }
    }

    public class EasyTablesDetailsPage : DataPage
    {
        public EasyTablesDetailsPage ()
        {
            var dataSourceProvider = (IDataSourceProvider)this;
            dataSourceProvider.MaskKey ("_id");
        }
    }
}

