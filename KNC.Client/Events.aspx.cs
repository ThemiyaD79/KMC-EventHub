using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace KNC.Client
{
    public partial class Events : System.Web.UI.Page
    {
        private readonly EventService _eventService = new EventService();
        private const string ImageFolder = "~/ProductImages/";

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) await LoadEvents();
        }

        private async Task LoadEvents()
        {
            try
            {
                string search = txtSearch.Text;
                string category = ddlFilterCategory.SelectedValue == "All" ? null : ddlFilterCategory.SelectedValue;

                var events = await _eventService.GetAllEvents(search, category);
                rptEvents.DataSource = events;
                rptEvents.DataBind();

                lblTotalCount.InnerText = $"{events.Count} total events";
                divNoResults.Style["display"] = events.Count == 0 ? "block" : "none";
            }
            catch (Exception ex) { ShowAlert("Error loading events: " + ex.Message); }
        }

        protected async void btnSearch_Click(object sender, EventArgs e) => await LoadEvents();

        protected void btnShowAddForm_Click(object sender, EventArgs e)
        {
            ClearForm();
            pnlList.Visible = false;
            pnlRegistrations.Visible = false;
            pnlForm.Visible = true;  
            formTitle.InnerText = "Add New Event";
            txtEventId.Enabled = true; 
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            pnlList.Visible = true; 
            ClearForm();
        }

        private string SaveImage()
        {
            if (!fileUploadImage.HasFile) return string.Empty;
            string ext = Path.GetExtension(fileUploadImage.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png") throw new Exception("Only JPG, JPEG, PNG allowed");
            
            string path = Server.MapPath(ImageFolder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            
            string fileName = Guid.NewGuid() + ext;
            string fullPath = Path.Combine(path, fileName);
            fileUploadImage.SaveAs(fullPath);
            return ImageFolder + fileName;
        }

        protected async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEventId.Text) || string.IsNullOrWhiteSpace(txtTitle.Text) || 
                    string.IsNullOrWhiteSpace(txtDescription.Text) || string.IsNullOrWhiteSpace(txtLocation.Text) || 
                    string.IsNullOrWhiteSpace(txtDate.Text)) 
                { ShowAlert("Please fill out all required fields."); return; }

                DateTime parsedDate;
                if (!DateTime.TryParse(txtDate.Text, out parsedDate)) { ShowAlert("Invalid Date format."); return; }
                if (parsedDate.Date < DateTime.Today) { ShowAlert("Cannot schedule in the past."); return; }

                string imagePath = SaveImage();

                var newEvent = new KMCEvent
                {
                    EventId = txtEventId.Text, Title = txtTitle.Text, Description = txtDescription.Text, 
                    Category = ddlFormCategory.SelectedValue, EventDate = parsedDate, Location = txtLocation.Text,
                    ImageUrl = string.IsNullOrEmpty(imagePath) ? hdnCurrentImage.Value : imagePath
                };

                if (!txtEventId.Enabled) { await _eventService.UpdateEvent(newEvent); ShowAlert("Event Updated successfully!"); }
                else { await _eventService.AddEvent(newEvent); ShowAlert("Event Saved successfully!"); ClearForm(); txtEventId.Enabled = true; }

                await LoadEvents();
            }
            catch (Exception ex) { ShowAlert(ex.Message); }
        }

        protected async void rptEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string eventId = e.CommandArgument.ToString();

            if (e.CommandName == "Edit")
            {
                var ev = await _eventService.GetEventById(eventId);
                txtEventId.Text = ev.EventId; txtEventId.Enabled = false; 
                txtTitle.Text = ev.Title; txtDescription.Text = ev.Description; 
                ddlFormCategory.SelectedValue = ev.Category; txtDate.Text = ev.EventDate.ToString("yyyy-MM-dd");
                txtLocation.Text = ev.Location;
                
                if (!string.IsNullOrEmpty(ev.ImageUrl))
                {
                    imgPreview.ImageUrl = ev.ImageUrl; imgPreview.Visible = true; hdnCurrentImage.Value = ev.ImageUrl;
                }
                pnlList.Visible = false; pnlForm.Visible = true; formTitle.InnerText = "Update Event";
            }
            else if (e.CommandName == "Delete") { await _eventService.DeleteEvent(eventId); await LoadEvents(); }
            else if (e.CommandName == "Register") { hdnRegisterEventId.Value = eventId; pnlRegisterModal.Visible = true; }
        }

        protected async void btnSubmitRegistration_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtRegName.Text) || string.IsNullOrWhiteSpace(txtRegAge.Text) || string.IsNullOrWhiteSpace(txtRegPhone.Text))
                { ShowAlert("Please fill out all registration fields."); return; }

                var newRegistration = new EventRegistration
                {
                    EventId = hdnRegisterEventId.Value, Name = txtRegName.Text,
                    Age = int.Parse(txtRegAge.Text), PhoneNumber = txtRegPhone.Text
                };

                await _eventService.RegisterForEvent(newRegistration);
                pnlRegisterModal.Visible = false; txtRegName.Text = ""; txtRegAge.Text = ""; txtRegPhone.Text = "";
                ShowAlert("Successfully registered!");
            }
            catch (Exception ex) { ShowAlert("Error during registration: " + ex.Message); }
        }

        protected void btnCloseRegistration_Click(object sender, EventArgs e)
        {
            pnlRegisterModal.Visible = false; txtRegName.Text = ""; txtRegAge.Text = ""; txtRegPhone.Text = "";
        }
        protected async void btnViewRegistrations_Click(object sender, EventArgs e)
        {
            try
            {
                pnlList.Visible = false;
                pnlForm.Visible = false;
                pnlRegistrations.Visible = true;

                var registrations = await _eventService.GetAllRegistrations();
                rptRegistrations.DataSource = registrations;
                rptRegistrations.DataBind();

                divNoRegistrations.Style["display"] = registrations.Count == 0 ? "block" : "none";
            }
            catch (Exception ex) { ShowAlert("Error loading registrations: " + ex.Message); }
        }

        protected void btnBackFromReg_Click(object sender, EventArgs e)
        {
            pnlRegistrations.Visible = false;
            pnlList.Visible = true;
        }

        private void ClearForm()
        {
            txtEventId.Text = ""; txtTitle.Text = ""; txtDescription.Text = ""; txtDate.Text = ""; txtLocation.Text = "";
            hdnCurrentImage.Value = ""; imgPreview.Visible = false;
            if (ddlFormCategory.Items.Count > 0) ddlFormCategory.SelectedIndex = 0;
        }

        private void ShowAlert(string msg) => ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{msg.Replace("'", "")}');", true);
    }
}