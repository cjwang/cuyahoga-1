using System;

namespace Cuyahoga.Modules.Forum.Domain
{
	/// <summary>
	/// Summary description for ForumCategory.
	/// </summary>
	public class ForumCategory
	{
		private int _id;
		private int _siteid;
		private int _boardid;
		private string _name;
		private int _sortorder;
		private DateTime	_dateCreated;
		private DateTime	_dateModified;

		#region Properties
		public virtual int Id
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public virtual int SiteId
		{
			get { return this._siteid; }
			set { this._siteid = value; }
		}

		public virtual int BoardId
		{
			get { return this._boardid; }
			set { this._boardid = value; }
		}

		public virtual string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public virtual int SortOrder
		{
			get { return this._sortorder; }
			set { this._sortorder = value; }
		}

		public virtual DateTime DateModified
		{
			get { return this._dateModified; }
			set { this._dateModified = value; }
		}

		public virtual DateTime DateCreated
		{
			get { return this._dateCreated; }
			set { this._dateCreated = value; }
		}

		#endregion
		public ForumCategory()
		{
			//
			// TODO: Add constructor logic here
			//
			this._id	= -1;
			this._dateCreated = DateTime.Now;
		}
	}
}

