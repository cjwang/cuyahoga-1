using System;
using System.Collections;
using System.Web.UI.WebControls;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using Cuyahoga.Core.Util;
using Cuyahoga.Core.Domain;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;
using Cuyahoga.Modules.Forum;
using Cuyahoga.Modules.Forum.Domain;
using Cuyahoga.Modules.Forum.Utils;
using Cuyahoga.Modules.Forum.Web.UI;

namespace Cuyahoga.Modules.Forum
{
	/// <summary>
	///		Summary description.
	/// </summary>
	public class ForumReplyPost : BaseForumControl
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl Welcome;
		protected System.Web.UI.WebControls.Button btnPreview;
		protected System.Web.UI.WebControls.Button btnPost;
        protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.TextBox txtMessage;
		protected System.Web.UI.WebControls.RequiredFieldValidator rfvMessage;
		protected System.Web.UI.WebControls.Panel pnlForumReplyPost;
		protected System.Web.UI.WebControls.PlaceHolder phTop;
		protected System.Web.UI.WebControls.PlaceHolder phFooter;
		protected System.Web.UI.WebControls.PlaceHolder phForumTop;
		protected System.Web.UI.WebControls.PlaceHolder phForumFooter;
		protected System.Web.UI.WebControls.Literal ltOrigPost;
		protected System.Web.UI.WebControls.Panel pnlPreview;
		protected System.Web.UI.WebControls.Literal ltPreviewPost;
		protected System.Web.UI.WebControls.Label lblPreview;
		protected System.Web.UI.WebControls.Panel pnlOrigPost;
		protected System.Web.UI.WebControls.Label lblOrigTopic;
		protected System.Web.UI.WebControls.Repeater rptSmily;
		protected System.Web.UI.WebControls.Panel pnlSmily;
		protected System.Web.UI.WebControls.Literal ltJsInject;
		protected System.Web.UI.HtmlControls.HtmlInputFile txtAttachment;
		
		#region Private vars
		private ForumForum	_forumForum;
		protected System.Web.UI.WebControls.Literal ltlUploadError;
		protected System.Web.UI.WebControls.Panel pnlUploadError;
		private ForumPost	_origPost;
		#endregion

		private void Page_Load(object sender, System.EventArgs e)
		{
			this._origPost		= base.ForumModule.GetForumPostById(base.ForumModule.CurrentForumPostId);
			this._forumForum	= base.ForumModule.GetForumById(base.ForumModule.CurrentForumId);
			base.ForumModule.CurrentForumCategoryId	= this._forumForum.CategoryId;

            this.BindTopFooter();
            base.LocalizeControls();
            this.ltOrigPost.Text = this._origPost.Message;
			this.BindJS();
			this.BindEmoticon();

			if(!this.IsPostBack)
			{
				if(base.ForumModule.QuotePost == 1)
				{
					this.txtMessage.Text = "[quote]" + TextParser.HtmlToForumCode(this._origPost.Message,base.ForumModule) + "[/quote]";
				}
			}
		}

		private void BindTopFooter()
		{
			ForumTop tForumTop;
			ForumFooter tForumFooter;

			tForumTop = (ForumTop)this.LoadControl("~/Modules/Forum/ForumTop.ascx");
			tForumTop.Module = base.ForumModule;
			this.phForumTop.Controls.Add(tForumTop);

			tForumFooter = (ForumFooter)this.LoadControl("~/Modules/Forum/ForumFooter.ascx");
			tForumFooter.Module	= base.ForumModule;
			this.phForumFooter.Controls.Add(tForumFooter);
		}

		private void BindJS()
		{
			string m = String.Format("{0}",this.txtMessage.ClientID);
			Page.RegisterClientScriptBlock("forumscripts", String.Format("<script language=\"JavaScript\" src=\"{0}/Modules/Forum/forum.js\"></script>\n",UrlHelper.GetSiteUrl()));
		}

		private void BindEmoticon()
		{
			this.rptSmily.DataSource = base.ForumModule.GetAllEmoticons();
			this.rptSmily.DataBind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnPreview_Click(object sender, System.EventArgs e)
		{
			this.pnlPreview.Visible = true;
			this.ltPreviewPost.Visible = true;
			this.lblPreview.Visible	= true;
			this.ltPreviewPost.Text	= TextParser.ForumCodeToHtml(this.txtMessage.Text,base.ForumModule);
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
            Response.Redirect(String.Format("{0}/ForumViewPost/{1}/PostId/{2}", UrlHelper.GetUrlFromSection(base.ForumModule.Section), base.ForumModule.CurrentForumId, base.ForumModule.CurrentForumPostId));
        }


		private void btnPost_Click(object sender, System.EventArgs e)
		{
			if(this.Page.IsValid)
			{
				Cuyahoga.Core.Domain.User tUser = this.Page.User.Identity as User;
				HttpPostedFile postedFile = this.txtAttachment.PostedFile;
				ForumFile fFile;

				ForumPost tForumPost	= new ForumPost();
				tForumPost.ForumId		= base.ForumModule.CurrentForumId;
				tForumPost.DateCreated	= DateTime.Now;
				tForumPost.Message		= TextParser.ForumCodeToHtml(this.txtMessage.Text,base.ForumModule);
				tForumPost.Topic		= this._origPost.Topic;
				tForumPost.ReplytoId	= base.ForumModule.OrigForumPostId;
				if(tUser != null)
				{
					tForumPost.UserId		= tUser.Id;
					tForumPost.UserName		= tUser.UserName;
				}
				else
				{
					tForumPost.UserId		= 0;
					tForumPost.UserName		= "Guest";
				}

				base.ForumModule.SaveForumPost(tForumPost);

				// Save attachement
				if(postedFile.ContentLength > 0)
				{
					try
					{
						this.CheckValidFile(this.txtAttachment);
						fFile = this.SaveAttachment(tForumPost,this.txtAttachment);
						tForumPost.AttachmentId = fFile.Id;
						base.ForumModule.SaveForumPost(tForumPost);
					}
					catch(Exception ex)
					{
						this.pnlUploadError.Visible = true;
						this.ltlUploadError.Text	= ex.Message;
						base.ForumModule.DeleteForumPost(tForumPost);
						return;
					}
				}

				// Update number of topics and number of posts
				this._forumForum.NumPosts++;
				base.ForumModule.SaveForum(this._forumForum);

				// Update the original post
				this._origPost.Replies++;
				base.ForumModule.SaveForumPost(this._origPost);
                Response.Redirect(String.Format("{0}/ForumViewPost/{1}/PostId/{2}", UrlHelper.GetUrlFromSection(base.ForumModule.Section), base.ForumModule.CurrentForumId, base.ForumModule.CurrentForumPostId));
			}
		}
		
		public string GetEmoticonIcon(object o)
		{
            ForumEmoticon ei = (ForumEmoticon)o;
            string imagepath = String.Format("{0}/Images/Standard/", this.TemplateSourceDirectory);
            // HARD CODEDE - CHANGEME!!
            string sResult = String.Format("<a href=\"javascript:InsertEmoticon('{0}','{1}');\"><img src=\"{2}{3}\" border=0 alt=\"\"></a>", this.txtMessage.ClientID, ei.TextVersion, imagepath, ei.ImageName);
            return sResult;
        }

		private void CheckValidFile(HtmlInputFile file) 
		{
			if(file.PostedFile==null || file.PostedFile.FileName.Trim().Length==0 || file.PostedFile.ContentLength==0)
				return;

			string filename = file.PostedFile.FileName;
			int pos = filename.LastIndexOfAny(new char[]{'/','\\'});
			if(pos>=0)
				filename = filename.Substring(pos+1);
			pos = filename.LastIndexOf('.');
			if(pos>=0) 
			{
				switch(filename.Substring(pos+1).ToLower()) 
				{
					default:
						break;
					case "asp":
					case "aspx":
					case "ascx":
					case "config":
					case "php":
					case "php3":
					case "js":
					case "vb":
					case "vbs":
						throw new Exception(String.Format(GetText("fileerror"),filename));
				}
			}
		}

		private ForumFile SaveAttachment(ForumPost post, HtmlInputFile file)
		{
			ForumFile fFile = new ForumFile();
			if(file.PostedFile==null || file.PostedFile.FileName.Trim().Length==0 || file.PostedFile.ContentLength==0)
				return null;

			string sUpDir = Server.MapPath(UrlHelper.GetApplicationPath() + "/Modules/Forum/Attach/");
			string filename = file.PostedFile.FileName;

			if(!System.IO.Directory.Exists(sUpDir))
			{
				System.IO.Directory.CreateDirectory(sUpDir);
			}

			int pos = filename.LastIndexOfAny(new char[]{'/','\\'});
			if (pos >= 0)
				filename = filename.Substring(pos+1);

			string newfilename = String.Format("{0}{1}.{2}",sUpDir,post.Id,filename);
			file.PostedFile.SaveAs(newfilename);
			fFile.OrigFileName = filename;
			fFile.ForumFileName = newfilename;
			fFile.FileSize = file.PostedFile.ContentLength;
			fFile.ContentType = file.PostedFile.ContentType;

			try
			{
				base.ForumModule.SaveForumFile(fFile);
			}
			catch(Exception ex)
			{
				throw new Exception("Unable to save file",ex);
			}
			return fFile;
		}
	}
}
