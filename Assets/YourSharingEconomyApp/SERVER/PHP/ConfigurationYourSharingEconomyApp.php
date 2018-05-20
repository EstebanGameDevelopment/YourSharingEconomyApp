<?php

	/*
		****CRITICAL****
		
		You should replace:
		
			"$private_key_aes" 
			"$kyRJEncryption" 
			"$ivRJEncryption" 
			
		by values of your own. If you don't do it you are going to be hacked.
	*/
	
	include 'AES.php';

	require 'mail/PHPMailerAutoload.php';

	header('Content-type: text/html; charset=utf-8');
    
	// Connecting, selecting database
    $LINK_DATABASE = mysqli_connect("localhost", "root", "")
       or die("Could not connect");
    // print "Connected successfully<p>";
    mysqli_select_db($LINK_DATABASE, "yoursharingeconomyapp") or die("Database Error::Could not select database)");

	// RESPONSES WILL INCLUDE SPECIAL CHARACTERS BECAUSE THEY CONTAIN USERS' WORDS
	mysqli_query ($LINK_DATABASE, "set character_set_client='utf8'"); 
	mysqli_query ($LINK_DATABASE, "set character_set_results='utf8'"); 
	mysqli_query ($LINK_DATABASE, "set collation_connection='utf8_general_ci'");

	// OFFICIAL NAME OF THE APPLICATION
	$OFFICIAL_NAME_APPLICATION_GLOBAL = " Your Sharing Economy App ";
	
	// ADDRESS OF YOUR SERVER
	$URL_BASE_SERVER = "http://localhost:8080/yoursharingeconomyapp/";
	
	// WILL ENABLE THE EMAIL SYSTEM FOR ACCOUNT CONFIRMATION AND OTHER OPERATIONS
	$ENABLE_EMAIL_SERVER = 0;
	
	// DATABASE ENCRYPTION
	$private_key_aes = "TdLdsQLiaDrSQUpd";
	$blockSize_aes = 128;
	
	// SEPARATOR TOKENS USED IN HTTPS COMMUNICATIONS
	$PARAM_SEPARATOR = "<par>";
	$LINE_SEPARATOR = "<line>";

	// AFTER 10 USERS REPORT A REQUEST AS TOXIC IT WILL BE AUTOMATICALLY FLAGGED
	$TOTAL_REPORT_REQUEST_TO_PENALTY = 10;  

	// AFTER A NUMBER OF REPORTS THE REQUEST WILL SHOW A WARNING IMAGE
	$TOTAL_REPORT_REQUEST_TO_WARN_USERS = 3;  
	
	// NUMBER OF HOURS TO WAIT TO CREATE A NEW PROPOSAL
	$HOURS_TO_WAIT_FOR_NEW_PROPOSAL = 3;
	
	// FREE REQUESTS AVAILABLE TO THE CONSUMER
	$FREE_REQUESTS_AVAILABLE_TO_CONSUMERS = 1;

	// TOTAL NUMBER OF IMAGES THAT CAN BE USED AS A REFERENCE (ALLOWS TO CONTROL THE DATA FOREACH REQUEST)
	$TOTAL_NUMBER_IMAGES_AS_REFERENCE = 3;

    // TOTAL NUMBER OF IMAGES THAT CAN BE USED AS A FINISHED JOB (ALLOWS TO CONTROL THE DATA FOREACH REQUEST)
	$TOTAL_NUMBER_IMAGES_AS_FINISHED = 3;
    
	// TOTAL NUMBER OF IMAGES THAT THE PROVIDER CAN POST TO SHOW HIS EXPERIENCE
	$TOTAL_NUMBER_IMAGES_PROVIDER_EXPERIENCE = 10;
	
    // SIZE TO SCALE THE IMAGES SELECTED BY THE USER
	$SIZE_HEIGHT_ALLOWED_IMAGES = 500;

	// NON-REPLY ADDRESS
	$NON_REPLY_EMAIL_ADDRESS = 'non-reply@YOUR_OWN_DOMAIN.com';

	// SKILLS OF THE PROVIDER
    $PROVIDER_SKILLS = "first;0<skill>second;0<skill>third;0<skill>fourth;0";
	
	// PASSWORD ENCRYPTION COMMS
	$kyRJEncryption = 'sK1rwpD1p+5e#bvt31CK13z77n=ES8jR'; // 32 * 8 = 256 bit key
	$ivRJEncryption = 'A9q2N2haeQybv8#Aq!N9ybc1Cnrx12@y'; // 32 * 8 = 256 bit iv		
	
	 //-------------------------------------------------------------
     //  SpecialCharacters
     //-------------------------------------------------------------
     function SpecialCharacters($text_plain_par)
     {
		$output_text = $text_plain_par;
		$output_text = str_replace('"', '\"', $output_text);
		$output_text = str_replace('\'', '`', $output_text);
		$output_text = str_replace('&', '\&', $output_text);
		
		return $output_text;
	 }

	  //-------------------------------------------------------------
     //  GetCurrentTimestamp
     //-------------------------------------------------------------
	function GetCurrentTimestamp()
    {
		$datebeging = new DateTime('1970-01-01');
		$currDate = new DateTime();
		$diff = $datebeging->diff($currDate);
		$secs=$diff->format('%a') * (60*60*24);  //total days
		$secs+=$diff->format('%h') * (60*60);     //hours
		$secs+=$diff->format('%i') * 60;              //minutes
		$secs+=$diff->format('%s');                     //seconds
		return $secs;
    }
	
	 
	 //-------------------------------------------------------------
     //  EncryptText
     //-------------------------------------------------------------
     function EncryptText($text_plain_par)
     {
		$aes = new AES($text_plain_par, $GLOBALS['private_key_aes'], $GLOBALS['blockSize_aes']);
		$text_encrypted = $aes->encrypt();
		return $text_encrypted;
	 }
  
	 
	 //-------------------------------------------------------------
     //  DecryptText
     //-------------------------------------------------------------
     function DecryptText($text_encrypted_par)
     {
		$aes = new AES($text_encrypted_par, $GLOBALS['private_key_aes'], $GLOBALS['blockSize_aes']);
		$text_decrypted=$aes->decrypt();
		return $text_decrypted;
	 }

	 //-------------------------------------------------------------
     //  rand_string
     //-------------------------------------------------------------
	 function rand_string( $length_par ) 
	 {
		$chars = "abcdefghijklmnopqrstuvwxyz0123456789";
		return substr(str_shuffle($chars),0,$length_par);
	}

	 //-------------------------------------------------------------
     //  IsLittleEndian
     //-------------------------------------------------------------
	function IsLittleEndian() 
	{
		$testint = 0x00FF;
		$p = pack('S', $testint);
		return $testint===current(unpack('v', $p));
	}
	
	 //-------------------------------------------------------------
     //  decryptRJ256
     //-------------------------------------------------------------
	function decryptRJ256($key,$iv,$string_to_decrypt)
	{
		$string_to_decrypt = base64_decode($string_to_decrypt);
		$rtn = mcrypt_decrypt(MCRYPT_RIJNDAEL_256, $key, $string_to_decrypt, MCRYPT_MODE_CBC, $iv);
		$rtn = rtrim($rtn, "\0\4");
		return($rtn);
	}

	 //-------------------------------------------------------------
 	 //  ExistsUser
     //-------------------------------------------------------------
	 function ExistsUser($iduser_par, $currpassword_encrypted_par)
     {
		// DECRIPT THE PASSWORD WITH THE KEY
		$currpassword_encrypted_corrected = str_replace(" ","+",$currpassword_encrypted_par);
		$currpassword_decrypted = decryptRJ256($GLOBALS['kyRJEncryption'],$GLOBALS['ivRJEncryption'],$currpassword_encrypted_corrected);
		$currpassword_encrypted_final = EncryptText($currpassword_decrypted);
	 
		// Performing SQL Consult
		$query_user = "SELECT id,email FROM users WHERE id = $iduser_par AND password = '$currpassword_encrypted_final'";
		$result_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_user) or die("Query Error::ConfigurationYourCollaborativeApp::ExistsUser");
		
		if ($row_user = mysqli_fetch_object($result_user))
		{
			return DecryptText($row_user->email);
		}
		else
		{
			return "";			
		}
	 }

	 //-------------------------------------------------------------
 	 //  SendGlobalEmail
     //-------------------------------------------------------------
	 function SendGlobalEmail($email_from_par, $email_to_par, $content_subject_par, $content_body_par)
     {
		// ++ATTENTION++
		// PHPMailer WILL NOT WORK IN XAMMP, BUT YOU HAVE TO USE IT TO SEND EMAILS TO ANY ADDRESS
		 
		if ($GLOBALS['ENABLE_EMAIL_SERVER'] == 1)
		{
			$mail = new PHPMailer();
			//Enable SMTP debugging
			// 0 = off (for production use)
			// 1 = client messages
			// 2 = client and server messages
			// $mail->SMTPDebug = 2;
			//Ask for HTML-friendly debug output
			// $mail->Debugoutput = 'html';
			
			$mail->isSMTP();
			$mail->CharSet = "UTF-8";
			$mail->SMTPSecure = 'tls';
			$mail->Host = 'smtp.YOUR_OWN_DOMAIN.com';
			$mail->Port = 587;
			$mail->Username = 'YOUR_OWN_EMAIL@YOUR_OWN_DOMAIN.com';
			$mail->Password = 'YOUR_OWN_PASSWORD';
			$mail->SMTPAuth = true;

			$mail->setFrom($email_from_par, $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL']);
			$mail->addAddress($email_to_par);

			$mail->isHTML(true);
			$mail->Subject = $content_subject_par;
			$mail->Body = $content_body_par;

			if (!$mail->send())
			{
			  echo "Mailer Error: " . $mail->ErrorInfo;
			  return false;
			}
			else
			{
			   return true;
			}
		}
		
		return true;
	 }
?>