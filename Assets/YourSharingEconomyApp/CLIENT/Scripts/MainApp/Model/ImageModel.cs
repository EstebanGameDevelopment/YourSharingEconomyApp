using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * ImageModel
	 * 
	 * Keeps all the information about the image
	 * 
	 * @author Esteban Gallardo
	 */
	public class ImageModel
	{
		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------
		private long m_id;
		private string m_table;
		private long m_idorigin;
		private int m_type;
		private int m_size;
		private byte[] m_data;
		private string m_url = "";

		public long Id
		{
			get { return m_id; }
			set { m_id = value; }
		}
		public string Table
		{
			get { return m_table; }
			set { m_table = value; }
		}
		public long IdOrigin
		{
			get { return m_idorigin; }
			set { m_idorigin = value; }
		}
		public int Type
		{
			get { return m_type; }
			set { m_type = value; }
		}
		public string Url
		{
			get { return m_url; }
			set { m_url = value; }
		}
		public int Size
		{
			get { return m_size; }
			set { m_size = value; }
		}
		public byte[] Data
		{
			get { return m_data; }
			set { m_data = value; }
		}

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public ImageModel()
		{
			m_id = -1;
			m_table = "";
			m_idorigin = -1;
			m_size = 0;
			m_data = null;
			m_type = 0;
			m_url = "";
		}

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public ImageModel(long _id,
						  string _table,
						  long _idorigin,
						  int _type,
						  int _size,
						  byte[] _data,
						  string _url)
		{
			m_id = _id;
			m_table = _table;
			m_idorigin = _idorigin;
			m_type = _type;
			m_size = _size;
			if (_data != null)
			{
				m_data = new byte[_data.Length];
				Array.Copy(_data, m_data, _data.Length);
			}
			m_url = _url;
		}

		// -------------------------------------------
		/* 
		 * Clone
		 */
		public ImageModel Clone()
		{
			return new ImageModel(m_id,
									m_table,
									m_idorigin,
									m_type,
									m_size,
									m_data,
									m_url);
		}

		// -------------------------------------------
		/* 
		 * Copy
		 */
		public void Copy(ImageModel _request)
		{
			m_id = _request.Id;
			m_table = _request.Table;
			m_idorigin = _request.IdOrigin;
			m_size = _request.Size;
			m_data = new byte[_request.Data.Length];
			Array.Copy(_request.Data, m_data, _request.Data.Length);
			m_url = _request.Url;
		}

		// -------------------------------------------
		/* 
		 * CopyData
		 */
		public void CopyData(byte[] _data)
		{
			m_data = new byte[_data.Length];
			Array.Copy(_data, m_data, _data.Length);
		}
	}
}