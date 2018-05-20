using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace YourSharingEconomyApp
{

	public class YourSharingEconomyTools
	{

#if UNITY_EDITOR
		public const string USER_EMAIL_COOCKIE = "USER_EMAIL_COOCKIE";
		public const string USER_NAME_COOCKIE = "USER_NAME_COOCKIE";
		public const string USER_PASSWORD_COOCKIE = "USER_PASSWORD_COOCKIE";
		public const string USER_FACEBOOK_CONNECTED_COOCKIE = "USER_FACEBOOK_CONNECTED_COOCKIE";

		/*
		 * You can use the login by email or clear all the PlayerPrefs and login by Facebook. 
		 * For development purposes it's recommended to work only with email because it's way faster.
		 * Remember that when you request the access token you should requests for permision of "email" and "user_friends"
		 * 
		 * https://developers.facebook.com/tools/explorer
		 */
		[MenuItem("YourSharingEconomyTools/Clear PlayerPrefs")]
		private static void NewMenuOption()
		{
			PlayerPrefs.DeleteAll();
			Debug.Log("PlayerPrefs CLEARED!!!");
		}

		/*
		 * In order for you to use this option, first, you will have to register in the system by email (not Facebook).
		 * After the register of the user, you should validate your account with a confirmation email sent to your email account and you are ready to work.
		*/
		[MenuItem("YourSharingEconomyTools/User Email Provider 1")]
		private static void UseAccountA()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString(USER_EMAIL_COOCKIE, RJEncryptor.Encrypt("vicens@yoursharingeconomyapp.com", true));
			PlayerPrefs.SetString(USER_PASSWORD_COOCKIE, RJEncryptor.Encrypt("12345", true));
			PlayerPrefs.SetString(USER_NAME_COOCKIE, "Account Provider");
			PlayerPrefs.SetInt(USER_FACEBOOK_CONNECTED_COOCKIE, 0);
		}


		[MenuItem("YourSharingEconomyTools/User Email Provider 2")]
		private static void UserAccountC()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString(USER_EMAIL_COOCKIE, RJEncryptor.Encrypt("sara@yoursharingeconomyapp.com", true));
			PlayerPrefs.SetString(USER_PASSWORD_COOCKIE, RJEncryptor.Encrypt("12345", true));
			PlayerPrefs.SetString(USER_NAME_COOCKIE, "Account Provider 2");
			PlayerPrefs.SetInt(USER_FACEBOOK_CONNECTED_COOCKIE, 0);
		}

		[MenuItem("YourSharingEconomyTools/User Email Client 1")]
		private static void UseAccountB()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString(USER_EMAIL_COOCKIE, RJEncryptor.Encrypt("ana@yoursharingeconomyapp.com", true));
			PlayerPrefs.SetString(USER_PASSWORD_COOCKIE, RJEncryptor.Encrypt("12345", true));
			PlayerPrefs.SetString(USER_NAME_COOCKIE, "Account Customer 1");
			PlayerPrefs.SetInt(USER_FACEBOOK_CONNECTED_COOCKIE, 0);
		}

		[MenuItem("YourSharingEconomyTools/User Email Client 2")]
		private static void UserAccountD()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString(USER_EMAIL_COOCKIE, RJEncryptor.Encrypt("hector@yoursharingeconomyapp.com", true));
			PlayerPrefs.SetString(USER_PASSWORD_COOCKIE, RJEncryptor.Encrypt("12345", true));
			PlayerPrefs.SetString(USER_NAME_COOCKIE, "Account Provider 2");
			PlayerPrefs.SetInt(USER_FACEBOOK_CONNECTED_COOCKIE, 0);
		}
#endif
	}
}