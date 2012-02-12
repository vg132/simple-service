<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SimpleService.SampleWebForms._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7/jquery.min.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>Welcome to Simple Service</h2>
    
    <hr />

    <div id="loading"></div>
    <div id="result"><b>Results:</b> <span></span></div>
    <hr />
    <div id="show"></div>
    <hr />

    <script type="text/javascript" src="/services/helloworld/proxy.min.js"></script>
    <script type="text/javascript">
        simpleServiceProxy.beginLoading = function () { $('#loading').text('Loading...'); };
        simpleServiceProxy.finishedLoading = function () { $('#loading').text('Loaded.'); };
        
        var service = new HelloWorldService();

        var responseHandler = function (response) {
            $('#result span').text(JSON.stringify(response));
            $('#show').html(response.value);
        };

        $(function () {
            service.controlTest('Simple Service!', responseHandler);
        });
        
    </script>
</asp:Content>
