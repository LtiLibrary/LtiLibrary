using System;
using System.Web.UI.WebControls;

namespace SimpleLti12.WebForms
{
    public partial class Tool : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OutcomesPanel.Visible = !string.IsNullOrEmpty(Request.Form["lis_outcome_service_url"]);
            ProfilePanel.Visible = !string.IsNullOrEmpty(Request.Form["custom_tc_profile_url"]);
            postParameters.DataSource = Request.Form;
            postParameters.DataBind();
        }

        protected void postParameters_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string key = e.Item.DataItem as string;
                var value = e.Item.FindControl("Value") as Literal;
                if (value != null && key != null) value.Text = Request.Form[key];
            }
        }
    }
}