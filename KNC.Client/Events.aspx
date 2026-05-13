<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="Events.aspx.cs" Inherits="KNC.Client.Events" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>KMC Event Manager</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f0f2f5; margin: 0; color: #333; }
        .header-banner { background-color: #0d2b4d; color: white; text-align: center; padding: 30px 20px; font-size: 28px; font-weight: bold; border-bottom: 5px solid #2d2d2d; }
        .container { max-width: 1200px; margin: 30px auto; background: #fff; padding: 30px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }
        .controls-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; padding-bottom: 15px; border-bottom: 1px solid #eee; }
        .header-title { font-size: 24px; font-weight: bold; margin: 0; }
        .header-subtitle { color: #777; font-size: 14px; margin-top: 5px; }
        .action-group { display: flex; gap: 10px; align-items: center;}
        
        .form-control { padding: 10px 15px; border: 1px solid #ccc; border-radius: 4px; font-size: 14px; outline: none; box-sizing: border-box; }
        .form-control:focus { border-color: #2196F3; }
        
        .btn { padding: 10px 20px; border: none; border-radius: 4px; cursor: pointer; font-weight: bold; color: white; font-size: 14px; text-decoration: none;}
        .btn-add { background-color: #28a745; }
        .btn-search { background-color: #6c757d; }
        .btn-blue { background-color: #007bff; }
        .btn-update { background-color: #007bff; padding: 6px 12px; font-size: 12px; border-radius: 4px; }
        .btn-delete { background-color: #ff9800; padding: 6px 12px; font-size: 12px; border-radius: 4px; margin-left: 5px; }
        .btn-back { background-color: #e0e0e0; color: #333; margin-bottom: 15px; display: inline-block;}

        .event-card { display: flex; border: 1px solid #ddd; border-radius: 8px; margin-bottom: 15px; overflow: hidden; background: #fff; }
        .event-image-container { width: 250px; background: #e9ecef; display: flex; align-items: center; justify-content: center; }
        .event-image { width: 100%; height: 100%; object-fit: cover; }
        .event-placeholder { font-size: 24px; color: #aaa; font-weight: bold; }
        .event-details { padding: 20px; flex-grow: 1; }
        .event-title { font-size: 18px; font-weight: bold; margin: 0 0 10px 0; }
        .event-info { font-size: 13px; color: #555; margin-bottom: 5px; display: flex; align-items: center; gap: 8px;}
        .event-desc { font-size: 14px; color: #444; margin: 10px 0; line-height: 1.5; background: #f9f9f9; padding: 10px; border-left: 3px solid #007bff; }
        .event-actions { margin-top: 15px; }

        .form-section { background: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px; border: 1px solid #ddd; position: relative; }
        .form-wrapper { max-width: 600px; margin: 0 auto; background: #fff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.05); }
        .form-group { margin-bottom: 15px; }
        .form-group label { display: block; font-weight: bold; margin-bottom: 5px; font-size: 14px; color: #333; }
        .form-group input[type="text"], .form-group input[type="date"], .form-group select, .form-group textarea, .form-group input[type="file"] { width: 100%; }
        
        .no-results { text-align: center; padding: 40px; color: #dc3545; font-size: 18px; font-weight: bold; display: none; }

        .modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 1000; }
        .modal-content { background: #fff; padding: 30px; border-radius: 8px; width: 400px; box-shadow: 0 4px 20px rgba(0,0,0,0.2); }
        .modal-header { font-size: 20px; font-weight: bold; margin-bottom: 15px; color: #0d2b4d; border-bottom: 2px solid #eee; padding-bottom: 10px; }
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div class="header-banner">KMC Event Management</div>

        <div class="container">
            
            <asp:Panel ID="pnlForm" runat="server" CssClass="form-section" Visible="false">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-back" OnClick="btnCancel_Click" CausesValidation="false">← Back to Events</asp:LinkButton>
                <div class="form-wrapper">
                    <h3 id="formTitle" runat="server" style="margin-top:0; text-align: center; color: #0d2b4d; border-bottom: 2px solid #eee; padding-bottom: 15px; margin-bottom: 20px;">Add New Event</h3>
                    <asp:HiddenField ID="hdnCurrentImage" runat="server" />
                    <div class="form-group"><label>Event ID (e.g. E001):</label><asp:TextBox ID="txtEventId" runat="server" CssClass="form-control"></asp:TextBox></div>
                    <div class="form-group"><label>Title:</label><asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox></div>
                    <div class="form-group"><label>Description:</label><asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox></div>
                    <div class="form-group">
                        <label>Category:</label>
                        <asp:DropDownList ID="ddlFormCategory" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Meeting" Value="Meeting"></asp:ListItem>
                            <asp:ListItem Text="Workshop" Value="Workshop"></asp:ListItem>
                            <asp:ListItem Text="Seminar" Value="Seminar"></asp:ListItem>
                            <asp:ListItem Text="Extracurricular" Value="Extracurricular"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div style="display: flex; gap: 15px;">
                        <div class="form-group" style="flex: 1;"><label>Date:</label><asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
                        <div class="form-group" style="flex: 1;"><label>Location:</label><asp:TextBox ID="txtLocation" runat="server" CssClass="form-control"></asp:TextBox></div>
                    </div>
                    <div class="form-group">
                        <label>Event Image:</label>
                        <asp:FileUpload ID="fileUploadImage" runat="server" CssClass="form-control" style="padding: 7px 15px;" />
                        <div style="text-align: center;"><asp:Image ID="imgPreview" runat="server" style="max-height: 120px; margin-top: 15px; border-radius: 4px;" Visible="false" /></div>
                    </div>
                    <div style="display: flex; gap: 10px; margin-top: 25px;">
                        <asp:Button ID="btnSave" runat="server" Text="Save Event" CssClass="btn btn-add" OnClick="btnSave_Click" style="flex: 1; padding: 12px; font-size: 16px;" />
                        <asp:Button ID="btnCancelForm" runat="server" Text="Cancel" CssClass="btn btn-search" OnClick="btnCancel_Click" CausesValidation="false" style="flex: 1; padding: 12px; background-color: #dc3545;" />
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlRegisterModal" runat="server" CssClass="modal-overlay" Visible="false">
                <div class="modal-content">
                    <div class="modal-header">Event Registration</div>
                    <asp:HiddenField ID="hdnRegisterEventId" runat="server" />
                    <div class="form-group"><label>Full Name:</label><asp:TextBox ID="txtRegName" runat="server" CssClass="form-control"></asp:TextBox></div>
                    <div class="form-group"><label>Age:</label><asp:TextBox ID="txtRegAge" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox></div>
                    <div class="form-group"><label>Phone Number:</label><asp:TextBox ID="txtRegPhone" runat="server" CssClass="form-control"></asp:TextBox></div>
                    <div style="display: flex; gap: 10px; margin-top: 20px;">
                        <asp:Button ID="btnSubmitRegistration" runat="server" Text="Confirm" CssClass="btn btn-blue" style="flex: 1;" OnClick="btnSubmitRegistration_Click" />
                        <asp:Button ID="btnCloseRegistration" runat="server" Text="Cancel" CssClass="btn btn-search" style="flex: 1; background-color: #dc3545;" OnClick="btnCloseRegistration_Click" CausesValidation="false" />
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlList" runat="server">
                <div class="controls-header">
                    <div>
                        <h1 class="header-title">List of Events</h1>
                        <p class="header-subtitle" id="lblTotalCount" runat="server">0 total events</p>
                    </div>
                    <div class="action-group">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" Placeholder="Search..."></asp:TextBox>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-blue" OnClick="btnSearch_Click" />
                        <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Filter Category" Value="All"></asp:ListItem>
                            <asp:ListItem Text="Meeting" Value="Meeting"></asp:ListItem>
                            <asp:ListItem Text="Workshop" Value="Workshop"></asp:ListItem>
                            <asp:ListItem Text="Seminar" Value="Seminar"></asp:ListItem>
                            <asp:ListItem Text="Extracurricular" Value="Extracurricular"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-search" OnClick="btnSearch_Click" />
                        
                        <asp:Button ID="btnViewRegistrations" runat="server" Text="📋 View Registrations" CssClass="btn btn-search" OnClick="btnViewRegistrations_Click" />
                        <asp:Button ID="btnShowAddForm" runat="server" Text="+ Add Event" CssClass="btn btn-add" OnClick="btnShowAddForm_Click" />
                    </div>
                </div>

                <div id="divNoResults" runat="server" class="no-results">No events found.</div>

                <asp:Repeater ID="rptEvents" runat="server" OnItemCommand="rptEvents_ItemCommand">
                    <ItemTemplate>
                        <div class="event-card">
                            <div class="event-image-container">
                                <%# string.IsNullOrEmpty(Eval("ImageUrl")?.ToString()) ? "<div class='event-placeholder'>Event</div>" : $"<img src='{ResolveUrl(Eval("ImageUrl").ToString())}' class='event-image' />" %>
                            </div>
                            <div class="event-details">
                                <h3 class="event-title"><%# Eval("Title") %></h3>
                                <div class="event-info">📍 <%# Eval("Location") %> | 🏷️ <%# Eval("Category") %> | 📅 <%# Convert.ToDateTime(Eval("EventDate")).ToString("yyyy-MM-dd") %></div>
                                <p class="event-desc"><%# Eval("Description") %></p>
                                <div class="event-actions">
                                    <asp:Button ID="btnRegister" runat="server" Text="REGISTER" CommandName="Register" CommandArgument='<%# Eval("EventId") %>' CssClass="btn btn-add" style="margin-right: 5px;" />
                                    <asp:Button ID="btnEdit" runat="server" Text="Update" CommandName="Edit" CommandArgument='<%# Eval("EventId") %>' CssClass="btn btn-update" />
                                    <asp:Button ID="btnDelete" runat="server" Text="DELETE" CommandName="Delete" CommandArgument='<%# Eval("EventId") %>' CssClass="btn btn-delete" OnClientClick="return confirm('Delete this event?');" />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>

            <asp:Panel ID="pnlRegistrations" runat="server" Visible="false" CssClass="form-section">
                <div class="controls-header">
                    <h2 style="margin:0; color:#0d2b4d;">Registered Participants</h2>
                    <asp:Button ID="btnBackFromReg" runat="server" Text="← Back to Events" CssClass="btn btn-search" OnClick="btnBackFromReg_Click" CausesValidation="false" />
                </div>
                
                <table style="width: 100%; border-collapse: collapse; margin-top: 20px; background: #fff; box-shadow: 0 2px 5px rgba(0,0,0,0.05);">
                    <thead>
                        <tr style="background-color: #0d2b4d; color: white; text-align: left;">
                            <th style="padding: 12px; border: 1px solid #ddd;">Reg ID</th>
                            <th style="padding: 12px; border: 1px solid #ddd;">Event Name</th>
                            <th style="padding: 12px; border: 1px solid #ddd;">Participant Name</th>
                            <th style="padding: 12px; border: 1px solid #ddd;">Age</th>
                            <th style="padding: 12px; border: 1px solid #ddd;">Phone Number</th>
                            <th style="padding: 12px; border: 1px solid #ddd;">Date Registered</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptRegistrations" runat="server">
                            <ItemTemplate>
                                <tr style="border-bottom: 1px solid #ddd;">
                                    <td style="padding: 12px; border: 1px solid #ddd;"><%# Eval("RegistrationId") %></td>
                                    <td style="padding: 12px; border: 1px solid #ddd; font-weight: bold; color: #007bff;"><%# Eval("EventTitle") %></td>
                                    <td style="padding: 12px; border: 1px solid #ddd;"><%# Eval("Name") %></td>
                                    <td style="padding: 12px; border: 1px solid #ddd;"><%# Eval("Age") %></td>
                                    <td style="padding: 12px; border: 1px solid #ddd;"><%# Eval("PhoneNumber") %></td>
                                    <td style="padding: 12px; border: 1px solid #ddd;"><%# Convert.ToDateTime(Eval("RegistrationDate")).ToString("yyyy-MM-dd HH:mm") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <div id="divNoRegistrations" runat="server" class="no-results" style="display:none; padding: 20px;">
                    No one has registered for any events yet.
                </div>
            </asp:Panel>

        </div>
    </form>
</body>
</html>