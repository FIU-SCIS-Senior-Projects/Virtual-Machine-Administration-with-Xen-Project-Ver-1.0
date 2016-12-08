<%@ Page Title="About UI" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
  

    <div class="back">
        	<div class="welcome">Welcome, admin</div>
            <div class="font36">Virtual machine administration- Xen project<br />
                <br />
  

              <div class="form-group">
                <label for="inputEmail3" class="col-sm-2 control-label font36 control-label-one">Ip Address:</label>
                    <asp:TextBox ID="Textbox_IPAddress" CssClass="form-control" placeholder="IP address" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter IP address" ControlToValidate="Textbox_IPAddress" Display="Dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Please enter valid IP address" ControlToValidate="Textbox_IPAddress" ValidationExpression="^(([01]?\d\d?|2[0-4]\d|25[0-5])\.){3}([01]?\d\d?|25[0-5]|2[0-4]\d)$" Display="Dynamic"></asp:RegularExpressionValidator><asp:LinkButton ID="LinkButton_Connect" CssClass="btn btn-success btn-5cca4d slow-click" runat="server" OnClick="LinkButton_Connect_Click">Connect</asp:LinkButton>
                </div>
                
              </div>
              <div class="slow" runat="server" id="Panel_slow">
                  <div class="form-group">
                    <label for="inputPassword3" class="col-sm-2 control-label">Host:</label>
                    &nbsp;<div class="col-sm-4">
                        <asp:TextBox ID="Textbox_Host" CssClass="form-control" placeholder="Host" runat="server"></asp:TextBox>
                    </div>
                  </div>
                  <div class="form-group">
                    <label for="inputPassword3" class="col-sm-2 control-label">CPU:</label>
                    &nbsp; <div class="col-sm-4">
                        <asp:TextBox ID="Textbox_CPU" CssClass="form-control" placeholder="CPU" runat="server"></asp:TextBox>
                    </div>
                  </div>
                  <div class="form-group">
                    <label for="inputPassword3" class="col-sm-2 control-label">MHZ:</label>
                     &nbsp;<div class="col-sm-4">
                     <asp:TextBox ID="Textbox_MHZ" CssClass="form-control" placeholder="MHZ" runat="server"></asp:TextBox>
                    </div>
                  </div>
                  <div class="form-group">
                    <label for="inputPassword3" class="col-sm-2 control-label">Memory:</label>
                    &nbsp; <div class="col-sm-4">
                      <asp:TextBox ID="Textbox_Memory" CssClass="form-control" placeholder="Memory" runat="server"></asp:TextBox>
                    </div>
                  </div>
              </div>
              <div class="form-group padding-top-50">
              	<div class="clearfix">
                	<button class="btn btn-black">create</button>
                	<button class="btn btn-black">delete</button>
                	<button class="btn btn-black">start</button>
               		<button class="btn btn-black">stop</button>
               		<button class="btn btn-black">snapshot</button>
                 	<button class="btn btn-black">resume</button>
                 </div>
              </div>
                <asp:Label ID="Label_IPAddress" runat="server" Text="IPAddress"></asp:Label>

            <asp:Label ID="Label_VMList" runat="server" Text="Label"></asp:Label>

            <asp:GridView ID="GridView1" runat="server">
            </asp:GridView>

            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CTAMConnectionString %>" SelectCommand="VMStatus_GetVMStatus" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" CssClass="table table-striped">
                <Columns>
                    <asp:BoundField DataField="HostID" HeaderText="HostID" SortExpression="HostID" />
                    <asp:BoundField DataField="HostName" HeaderText="HostName" SortExpression="HostName" />
                    <asp:BoundField DataField="VMUUID" HeaderText="VMUUID" SortExpression="VMUUID" />
                    <asp:BoundField DataField="VMName" HeaderText="VMName" SortExpression="VMName" />
                    <asp:BoundField DataField="OperatingSystem" HeaderText="OperatingSystem" SortExpression="OperatingSystem" />
                    <asp:BoundField DataField="CPUTime" HeaderText="CPUTime" SortExpression="CPUTime" />
                    <asp:BoundField DataField="VMState" HeaderText="VMState" SortExpression="VMState" />
                    <asp:BoundField DataField="Memory" HeaderText="Memory" SortExpression="Memory" />
                    <asp:BoundField DataField="MaxMemory" HeaderText="MaxMemory" SortExpression="MaxMemory" />
                    <asp:BoundField DataField="VirtualCPUS" HeaderText="VirtualCPUS" SortExpression="VirtualCPUS" />
                    <asp:BoundField DataField="StatusCode" HeaderText="StatusCode" SortExpression="StatusCode" />
                    <asp:BoundField DataField="Comments" HeaderText="Comments" SortExpression="Comments" />
                </Columns>
            </asp:GridView>

        </div>
</asp:Content>
