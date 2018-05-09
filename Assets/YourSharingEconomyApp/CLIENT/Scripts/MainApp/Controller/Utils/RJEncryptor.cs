using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.IO;

/******************************************
 * 
 * RJEncryptor
 * 
 * Encryption class that uses Rijndael algorithm
 * 
 */
public static class RJEncryptor
{
    // -------------------------------------------
    /* 
	 * DecryptString
	 */
    public static string DecryptString(string _textToDecrypt, bool _showLog)
    {
        string sEncryptedString = _textToDecrypt;

        var myRijndael = new RijndaelManaged()
        {
            Padding = PaddingMode.Zeros,
            Mode = CipherMode.CBC,
            KeySize = 256,
            BlockSize = 256
        };

        byte[] key = Encoding.ASCII.GetBytes(ScreenController.KYRJEncryption);
        byte[] IV = Encoding.ASCII.GetBytes(ScreenController.SIVJEncryption);

        var decryptor = myRijndael.CreateDecryptor(key, IV);

        byte[] sEncrypted = Convert.FromBase64String(sEncryptedString);

        byte[] fromEncrypt = new byte[sEncrypted.Length];

        MemoryStream msDecrypt = new MemoryStream(sEncrypted);
        CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

        csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

        string encrytpedText = Encoding.ASCII.GetString(fromEncrypt).Trim('\x0');
        if (_showLog)
        {
            Debug.Log("DecryptString::fromEncrypt.Length=" + fromEncrypt.Length + ";encrytpedText=" + encrytpedText.Length);
        }
        return (encrytpedText);
    }

    // -------------------------------------------
    /* 
	 * Encrypt
	 */
    public static string Encrypt(string _textToEncrypt, bool _showLog)
    {
        var sToEncrypt = _textToEncrypt;

        var myRijndael = new RijndaelManaged()
        {
            Padding = PaddingMode.Zeros,
            Mode = CipherMode.CBC,
            KeySize = 256,
            BlockSize = 256
        };

        var key = Encoding.ASCII.GetBytes(ScreenController.KYRJEncryption);
        var IV = Encoding.ASCII.GetBytes(ScreenController.SIVJEncryption);

        var encryptor = myRijndael.CreateEncryptor(key, IV);

        var msEncrypt = new MemoryStream();
        var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

        var toEncrypt = Encoding.ASCII.GetBytes(sToEncrypt);

        csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
        csEncrypt.FlushFinalBlock();

        var encrypted = msEncrypt.ToArray();

        string encryptedResult = Convert.ToBase64String(encrypted);
        if (_showLog)
        {
            Debug.Log("Encrypt::ORIGINAL[" + _textToEncrypt + "]:encryptedResult[" + encryptedResult + "]::DESCRYPTED[" + DecryptString(encryptedResult, false) + "]");
        }
        return (encryptedResult);
    }


}