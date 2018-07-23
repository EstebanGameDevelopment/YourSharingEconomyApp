using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YourBitcoinController;
using YourCommonTools;
using YourEthereumController;

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
			PlayerPrefs.SetString(USER_EMAIL_COOCKIE, RJEncryptor.EncryptStringWithKey("vicens@yoursharingeconomyapp.com", MenusScreenController.KYRJEncryption));
			PlayerPrefs.SetString(USER_PASSWORD_COOCKIE, RJEncryptor.EncryptStringWithKey("12345", MenusScreenController.KYRJEncryption));
#if ENABLE_BITCOIN
			PlayerPrefs.SetString(BitCoinController.NETWORK_TEST + BitCoinController.BITCOIN_PRIVATE_KEY_SELECTED, RJEncryptor.EncryptStringWithKey("cUqTycDcKc56GEJBzXrpnJm8Pj8tVMQA6U4bhVRid7tYbb4T5S42", BitCoinController.ENCRYPTION_KEY));
			PlayerPrefs.SetString(BitCoinController.NETWORK_TEST + BitCoinController.BITCOIN_PRIVATE_KEYS, RJEncryptor.EncryptStringWithKey("cUqTycDcKc56GEJBzXrpnJm8Pj8tVMQA6U4bhVRid7tYbb4T5S42" + BitCoinController.SEPARATOR_COMA + "0", BitCoinController.ENCRYPTION_KEY));
#elif ENABLE_ETHEREUM
            PlayerPrefs.SetString(EthereumController.NETWORK_TEST + EthereumController.ETHEREUM_PRIVATE_KEY_SELECTED, RJEncryptor.EncryptStringWithKey("0x1c6c36b151745ed7c93a6e353d0919a98a1b365474136723a743b8d0fa8144f6", EthereumController.ENCRYPTION_KEY));
            PlayerPrefs.SetString(EthereumController.NETWORK_TEST + EthereumController.ETHEREUM_PRIVATE_KEYS, RJEncryptor.EncryptStringWithKey("0x1c6c36b151745ed7c93a6e353d0919a98a1b365474136723a743b8d0fa8144f6" + EthereumController.SEPARATOR_COMA + "0", EthereumController.ENCRYPTION_KEY));
#endif
            PlayerPrefs.SetString(USER_NAME_COOCKIE, "Account Provider");
			PlayerPrefs.SetInt(USER_FACEBOOK_CONNECTED_COOCKIE, 0);
		}


		[MenuItem("YourSharingEconomyTools/User Email Provider 2")]
		private static void UserAccountC()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString(USER_EMAIL_COOCKIE, RJEncryptor.EncryptStringWithKey("sara@yoursharingeconomyapp.com", MenusScreenController.KYRJEncryption));
			PlayerPrefs.SetString(USER_PASSWORD_COOCKIE, RJEncryptor.EncryptStringWithKey("12345", MenusScreenController.KYRJEncryption));
#if ENABLE_BITCOIN
			PlayerPrefs.SetString(BitCoinController.NETWORK_TEST + BitCoinController.BITCOIN_PRIVATE_KEY_SELECTED, RJEncryptor.EncryptStringWithKey("cV3PCdEEABBFBCHP7S3kMnrVVPJSwBcDiJbbHawnzaP3YvGt9pr6", BitCoinController.ENCRYPTION_KEY));
			PlayerPrefs.SetString(BitCoinController.NETWORK_TEST + BitCoinController.BITCOIN_PRIVATE_KEYS, RJEncryptor.EncryptStringWithKey("cV3PCdEEABBFBCHP7S3kMnrVVPJSwBcDiJbbHawnzaP3YvGt9pr6" + BitCoinController.SEPARATOR_COMA + "0", BitCoinController.ENCRYPTION_KEY));
#elif ENABLE_ETHEREUM
            PlayerPrefs.SetString(EthereumController.NETWORK_TEST + EthereumController.ETHEREUM_PRIVATE_KEY_SELECTED, RJEncryptor.EncryptStringWithKey("0x1f30969e98355945c2e1200bbc490443c9a3e71316bddc048e8c8a30b97725f7", EthereumController.ENCRYPTION_KEY));
            PlayerPrefs.SetString(EthereumController.NETWORK_TEST + EthereumController.ETHEREUM_PRIVATE_KEYS, RJEncryptor.EncryptStringWithKey("0x1f30969e98355945c2e1200bbc490443c9a3e71316bddc048e8c8a30b97725f7" + EthereumController.SEPARATOR_COMA + "0", EthereumController.ENCRYPTION_KEY));
#endif
            PlayerPrefs.SetString(USER_NAME_COOCKIE, "Account Provider 2");
			PlayerPrefs.SetInt(USER_FACEBOOK_CONNECTED_COOCKIE, 0);
		}

		[MenuItem("YourSharingEconomyTools/User Email Client 1")]
		private static void UseAccountB()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString(USER_EMAIL_COOCKIE, RJEncryptor.EncryptStringWithKey("ana@yoursharingeconomyapp.com", MenusScreenController.KYRJEncryption));
			PlayerPrefs.SetString(USER_PASSWORD_COOCKIE, RJEncryptor.EncryptStringWithKey("12345", MenusScreenController.KYRJEncryption));
#if ENABLE_BITCOIN
			PlayerPrefs.SetString(BitCoinController.NETWORK_TEST + BitCoinController.BITCOIN_PRIVATE_KEY_SELECTED, RJEncryptor.EncryptStringWithKey("cTvjCsEcPrhNtRo8NzGmF7hfoht4ZgKkucsNPwPYcHHdebNtrvVv", BitCoinController.ENCRYPTION_KEY));
			PlayerPrefs.SetString(BitCoinController.NETWORK_TEST + BitCoinController.BITCOIN_PRIVATE_KEYS, RJEncryptor.EncryptStringWithKey("cTvjCsEcPrhNtRo8NzGmF7hfoht4ZgKkucsNPwPYcHHdebNtrvVv" + BitCoinController.SEPARATOR_COMA + "0", BitCoinController.ENCRYPTION_KEY));
#elif ENABLE_ETHEREUM
            PlayerPrefs.SetString(EthereumController.NETWORK_TEST + EthereumController.ETHEREUM_PRIVATE_KEY_SELECTED, RJEncryptor.EncryptStringWithKey("0x5bd8bcbb596f6612bcd06d008d328482d9a986bef6d7a179db7022c35b7b69ae", EthereumController.ENCRYPTION_KEY));
            PlayerPrefs.SetString(EthereumController.NETWORK_TEST + EthereumController.ETHEREUM_PRIVATE_KEYS, RJEncryptor.EncryptStringWithKey("0x5bd8bcbb596f6612bcd06d008d328482d9a986bef6d7a179db7022c35b7b69ae" + EthereumController.SEPARATOR_COMA + "0", EthereumController.ENCRYPTION_KEY));
#endif
            PlayerPrefs.SetString(USER_NAME_COOCKIE, "Account Customer 1");
			PlayerPrefs.SetInt(USER_FACEBOOK_CONNECTED_COOCKIE, 0);
		}

		[MenuItem("YourSharingEconomyTools/User Email Client 2")]
		private static void UserAccountD()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString(USER_EMAIL_COOCKIE, RJEncryptor.EncryptStringWithKey("hector@yoursharingeconomyapp.com", MenusScreenController.KYRJEncryption));
			PlayerPrefs.SetString(USER_PASSWORD_COOCKIE, RJEncryptor.EncryptStringWithKey("12345", MenusScreenController.KYRJEncryption));
#if ENABLE_BITCOIN
			PlayerPrefs.SetString(BitCoinController.NETWORK_TEST + BitCoinController.BITCOIN_PRIVATE_KEY_SELECTED, RJEncryptor.EncryptStringWithKey("cS5NzhNGZ4tjEB666RgGSMo2JYgo9vAdeRRMkscoVvxVTVQXTVXk", BitCoinController.ENCRYPTION_KEY));
			PlayerPrefs.SetString(BitCoinController.NETWORK_TEST + BitCoinController.BITCOIN_PRIVATE_KEYS, RJEncryptor.EncryptStringWithKey("cS5NzhNGZ4tjEB666RgGSMo2JYgo9vAdeRRMkscoVvxVTVQXTVXk" + BitCoinController.SEPARATOR_COMA + "0", BitCoinController.ENCRYPTION_KEY));
#elif ENABLE_ETHEREUM
            PlayerPrefs.SetString(EthereumController.NETWORK_TEST + EthereumController.ETHEREUM_PRIVATE_KEY_SELECTED, RJEncryptor.EncryptStringWithKey("0x2e9942b76cd4b3072dd316c2475c26e55873dbfd16ae6c4f5c03fec7ae61ca61", EthereumController.ENCRYPTION_KEY));
            PlayerPrefs.SetString(EthereumController.NETWORK_TEST + EthereumController.ETHEREUM_PRIVATE_KEYS, RJEncryptor.EncryptStringWithKey("0x2e9942b76cd4b3072dd316c2475c26e55873dbfd16ae6c4f5c03fec7ae61ca61" + EthereumController.SEPARATOR_COMA + "0", EthereumController.ENCRYPTION_KEY));
#endif
            PlayerPrefs.SetString(USER_NAME_COOCKIE, "Account Provider 2");
			PlayerPrefs.SetInt(USER_FACEBOOK_CONNECTED_COOCKIE, 0);
		}
#endif
        }
    }