using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace YourSharingEconomyApp
{

	public interface IHTTPComms
	{
		string UrlRequest { get; }
		WWWForm FormPost { get; }
		int Method { get; }

		string Build(params object[] _list);
		void Response(byte[] _response);
		void Response(string _response);
	}

}