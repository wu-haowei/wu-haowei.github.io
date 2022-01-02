using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Common_jPager : System.Web.UI.UserControl
{
    public bool IsPaging = false;
    private GridView _oGridView;
    public GridView oGridView {
        set
        {
            _oGridView = value;
            oGridView.DataBound += new EventHandler(gvIndex_DataBound);
            oGridView.PagerSettings.Visible = false;
        }
        get
        {
            return _oGridView;
        }
    }
    public ObjectDataSource _oObjectDataSource;
    public ObjectDataSource oObjectDataSource
    {
        set
        {
            _oObjectDataSource = value;
            _oObjectDataSource.Selected += new ObjectDataSourceStatusEventHandler(odsIndex_Selected);
        }
        get
        {
            return _oObjectDataSource;
        }
    }
    private int pageview = 3; 

    public int _TotalCount = 0;
    public int TotalCount
    {
        set
        {
            _TotalCount = value;
        }
        get
        {
            return _TotalCount;
        }
    }
    
    public int PageCount
    {
        get
        {
            int _PageCount = 0;
            if (!IsPaging)
            {
                _PageCount = oGridView.PageCount;
            }
            else
            {
                _PageCount = Convert.ToInt16(Math.Ceiling((double)TotalCount / (double)oGridView.PageSize));
            }
            return _PageCount;
        }
    }

    public int _PageSize = 0;
    public int PageSize
    {
        set
        {
            _PageSize = value;
            oGridView.PageSize = _PageSize;
            if (_oObjectDataSource.SelectParameters["PageSize"] != null)
                _oObjectDataSource.SelectParameters["PageSize"].DefaultValue = _PageSize.ToString();
        }
        get
        {
            return _PageSize;
        }
    }

    public int _PageIndex = 0;
    public int PageIndex
    {
        set
        {
            _PageIndex = value;
            oGridView.PageIndex = _PageIndex;
            if (IsPaging)
            {
                _oObjectDataSource.SelectParameters["PageIndex"].DefaultValue = (_PageIndex + 1).ToString();
            }
        }
        get
        {
            return _PageIndex;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (IsPaging)
        {
            string oldPageIndex = _oObjectDataSource.SelectParameters["PageIndex"].DefaultValue;
            _oObjectDataSource.SelectParameters["PageIndex"].DefaultValue = "0";
            DataView dv = (DataView)_oObjectDataSource.Select();
            DataRowView rowView = dv[0];
            TotalCount = int.Parse(rowView[0].ToString());            
            _oObjectDataSource.SelectParameters["PageIndex"].DefaultValue = oldPageIndex;
           
        }
        
    }

    protected void odsIndex_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if( !IsPaging)
        {
            //int i = ((List<object>)e.ReturnValue).Count();
            //TotalCount = -9999;
            if (e.ReturnValue is ICollection)
            {
                TotalCount = ((ICollection)e.ReturnValue).Count;
            }
            if (e.ReturnValue is DataTable)
            {
                TotalCount = ((DataTable)e.ReturnValue).DefaultView.Count;
            }
            ddlPageIndex.SelectedValue = oGridView.PageSize.ToString();
        }
    }

    protected void gvIndex_DataBound(object sender, EventArgs e)
    {
        
        if (PageCount > 1)
        {
            PagerUI.Visible = true;
            GenControl();
        }
        else
        {
            PagerUI.Visible = false;
        }

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        PageIndex = int.Parse(hidPage.Value);
        
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
      
        PageSize= int.Parse(hidPageSize.Value);
        PageIndex = 0;
        oGridView.DataBind();
    }
    public void GenControl()
    {
        litContent.Text = "";
        int MinPage = oGridView.PageIndex - pageview;
        if (MinPage <= 0) MinPage = 0;
        int MaxPage = oGridView.PageIndex + pageview;
        if (MaxPage > PageCount-1) MaxPage = PageCount-1;

        if (MinPage > 0) litContent.Text += " <li "  + " data-index=\"" + 0 + "\"><span ><a>" + 1 + "</a></span></li>";
        if (MinPage > 1) litContent.Text += " <li "  + " data-index=\"" + 1 + "\"><span class=\"page_empty\">" + " ... " + "</span></li>";

        for (int i = MinPage; i <= MaxPage; i++)
        {
            string sClass = "";
            if (i == oGridView.PageIndex) sClass = "class=\"is-active\"";
            litContent.Text +=" <li " + sClass + " data-index=\"" + i + "\"><span ><a>" + (i + 1) + "</a></span></li>";
        }

        if(MaxPage<PageCount-2) litContent.Text += " <li "  + " data-index=\"" + (PageCount - 2) + "\"><span class=\"page_empty\">" + " ... " + "</span></li>";
        if(MaxPage< PageCount - 1) litContent.Text += " <li "  + " data-index=\"" + (PageCount - 1) + "\"><span ><a>" + PageCount + "</a></span></li>";
    }


    
}