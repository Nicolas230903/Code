<%@ Application Language="C#" %>


<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {

    }

    void Application_BeginRequest(Object sender, EventArgs e)
    {
        //Para que esto funcione tiene que estar el archivo PrecompiledApp.config en el server
        /*if (HttpContext.Current.Request.Url.ToString().ToLower().StartsWith("http://app.axanweb.com/login"))
        {
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", "https://app.axanweb.com/login.aspx");
        }
        else if (HttpContext.Current.Request.Url.ToString().ToLower().StartsWith("http://app.axanweb.com/registro"))
        {
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", "https://app.axanweb.com/registro.aspx");
        }
        else if (HttpContext.Current.Request.Url.ToString().ToLower().StartsWith("http://app.axanweb.com"))
        {
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", "https://app.axanweb.com");
        }*/

        //if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("http:") && HttpContext.Current.Request.IsLocal.Equals(false))
        //{
        //    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.ToString().ToLower().Replace("http:", "https:"));
        //}
    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    protected void Application_Error(object sender, EventArgs e)
    {
        Exception TheError = Server.GetLastError();

        if (HttpContext.Current.Request.IsLocal.Equals(false) && HttpContext.Current.Request.Url.ToString().ToLower().Substring(0, 18).Contains("app"))
        {
            if (TheError is HttpException && ((HttpException)TheError).GetHttpCode() == 400)
                Response.Redirect("/errorGenerico.aspx");
            else if (TheError is HttpException && ((HttpException)TheError).GetHttpCode() == 401)
                Response.Redirect("/errorGenerico.aspx");
            else if (TheError is HttpException && ((HttpException)TheError).GetHttpCode() == 403)
                Response.Redirect("/errorGenerico.aspx");
            else if (TheError is HttpException && ((HttpException)TheError).GetHttpCode() == 404)
                Response.Redirect("/errorGenerico.aspx");
            else if (TheError is HttpException && ((HttpException)TheError).GetHttpCode() == 500)
                Response.Redirect("/errorGenerico.aspx");
            else
                Response.Redirect("/errorGenerico.aspx");
        }
    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

</script>
